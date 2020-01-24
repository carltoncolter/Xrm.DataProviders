﻿using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using CustomDataProviders.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace CustomDataProviders.AADODataProvider
{
    public class DataService : IDataService
    {
        private readonly DataSource _dataSource;
        private readonly IOrganizationService _service;

        public DataService(IOrganizationService service, Entity dataSource)
        {
            _service = service;
            _dataSource = new DataSource(dataSource);
        }

        public EntityCollection GetEntities(QueryExpression query)
        {
            var metadata = _service.GetEntityMetadata(query.EntityName);

            var visitor = new ODataQueryExpressionVisitor(metadata);
            visitor.Visit(query);

            var url = $"{_dataSource.Resource}/{metadata.ExternalCollectionName}?{visitor.QueryString}";

            var records = GetResponse<Records>(GetRequest("GET", url, query.PageInfo?.Count ?? 0));

            var entities = new EntityCollection
            {
                MoreRecords = !string.IsNullOrEmpty(records.NextLink),
                TotalRecordCountLimitExceeded = false,
                TotalRecordCount = records.Count
            };
            if (!string.IsNullOrEmpty(records.NextLink))
            {
                var regex = new Regex("pagingcookie='(.*?)'");
                var match = regex.Match(WebUtility.UrlDecode(records.NextLink));
                if (match.Success) entities.PagingCookie = match.Groups[1].Value;
            }

            foreach (var record in records.Value) entities.Entities.Add(record.ToEntity(metadata));

            return entities;
        }

        public Entity GetEntity(EntityReference reference)
        {
            var metadata = _service.GetEntityMetadata(reference.LogicalName);

            var url = $"{_dataSource.Resource}/{metadata.ExternalCollectionName}({reference.Id.ToString("D")})";

            var record = GetResponse<Record>(GetRequest("GET", url));

            return record.ToEntity(metadata);
        }

        public Token GetToken()
        {
            var uri = "https://login.microsoftonline.com/common/oauth2/token";

            var parameters = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _dataSource.ClientId,
                ["resource"] = _dataSource.Resource,
                ["username"] = _dataSource.Username,
                ["password"] = _dataSource.Password
            };

            var request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.Write(parameters);

            return GetResponse<Token>(request);
        }

        private HttpWebRequest GetRequest(string method, string url, int? pageSize = null)
        {
            var token = GetToken();

            var request = WebRequest.CreateHttp($"{token.Resource}{url}");
            request.Method = method;
            request.Accept = "application/json; odata.metadata=none;";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
            request.Headers.Add("OData-MaxVersion", "4.0");
            request.Headers.Add("OData-Version", "4.0");
            if (pageSize != null) request.Headers.Add("Prefer", $"odata.maxpagesize={pageSize}");

            // TODO: Handle URL's over 2048 characters.

            return request;
        }

        private TResponse GetResponse<TResponse>(HttpWebRequest request)
        {
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var settings = new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };
                var serializer = new DataContractJsonSerializer(typeof(TResponse), settings);
                return (TResponse) serializer.ReadObject(stream);
            }
        }
    }
}