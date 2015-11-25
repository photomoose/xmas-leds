using Rumr.DurryLights.Domain;
using Rumr.DurryLights.Domain.Models;

namespace DurryLights.Domain.Tests
{
    public class LightsRequestBuilder
    {
        private string _from = "unit-test";
        private string _source = "test-suite";
        private string _text = "blue";

        public LightsRequest Build()
        {
            return new LightsRequest
            {
                From = _from,
                Source = _source,
                Text = _text
            };
        }

        public LightsRequestBuilder ForText(string text)
        {
            _text = text;
            return this;
        }

        public LightsRequestBuilder From(string from)
        {
            _from = from;
            return this;
        }

        public LightsRequestBuilder FromSource(string source)
        {
            _source = source;
            return this;
        }
    }
}