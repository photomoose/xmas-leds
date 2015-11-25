using System;
using System.Threading.Tasks;
using Nest;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.Domain.Models;

namespace DurryLights.ElasticSearch
{
    public class ElasticMetricWriter : IMetricWriter
    {
        private readonly ElasticClient _elasticClient;

        public ElasticMetricWriter(string elasticSearchEndpoint)
        {
            var node = new Uri(elasticSearchEndpoint);
            var settings = new ConnectionSettings(node);

            settings.MapDefaultTypeIndices(d => d.Add(typeof(Metric), "durrylights"));

            _elasticClient = new ElasticClient(settings);
        }

        public async Task SendAsync(Metric metric)
        {
            await _elasticClient.IndexAsync(metric);
        }
    }
}
