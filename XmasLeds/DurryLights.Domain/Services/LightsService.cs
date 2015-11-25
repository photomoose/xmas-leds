using System;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Messaging;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Repositories;
using Rumr.DurryLights.Domain.Utilities;

namespace Rumr.DurryLights.Domain.Services
{
    public class LightsService : ILightsService
    {
        private readonly IBusPublisher _busPublisher;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMetricWriter _metricWriter;
        private readonly LightDisplayParser _lightDisplayParser;
        private DateTime _lastRequestUtc;

        public LightsService(
            IBusPublisher busPublisher, 
            IColourRepository colourRepository, 
            IDateTimeProvider dateTimeProvider,
            IMetricWriter metricWriter)
        {
            _busPublisher = busPublisher;
            _dateTimeProvider = dateTimeProvider;
            _metricWriter = metricWriter;
            _lightDisplayParser = new LightDisplayParser(colourRepository);
        }

        public async Task HandleRequestAsync(LightsRequest request)
        {
            var now = _dateTimeProvider.UtcNow();

            var metric = new Metric
            {
                TimestampUtc = now,
                RequestText = request.Text,
                Source = request.Source,
                From = request.From
            };

            var lightDisplay = await _lightDisplayParser.ParseAsync(request.Text);

            if (lightDisplay.Colours.Count == 0)
            {
                await _metricWriter.SendAsync(metric);

                return;
            }

            var isScheduled = false;
            var scheduledEnqueueTimeUtc = _lastRequestUtc.AddMinutes(1);


            if (now > scheduledEnqueueTimeUtc)
            {
                await _busPublisher.PublishAsync(lightDisplay);
                _lastRequestUtc = now;

            }
            else
            {
                isScheduled = true;
                await _busPublisher.PublishAsync(lightDisplay, scheduledEnqueueTimeUtc);
                _lastRequestUtc = scheduledEnqueueTimeUtc;
            }

            metric.IsValidCommand = true;
            metric.DisplayType = lightDisplay.GetType().Name;
            metric.Command = lightDisplay;
            metric.IsScheduled = isScheduled;
            metric.ScheduledAtUtc = isScheduled ? (DateTime?) scheduledEnqueueTimeUtc : null;
            metric.ScheduledDelaySecs = isScheduled ? (double?)(scheduledEnqueueTimeUtc - now).TotalSeconds : null;

            await _metricWriter.SendAsync(metric);
        }
    }
}