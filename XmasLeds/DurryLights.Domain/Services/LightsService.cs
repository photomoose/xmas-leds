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
        private string _lastRequestUser;
        private bool _isScheduleActive;

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

            var scheduledEnqueueTimeUtc = _lastRequestUtc.AddMinutes(1);
            
            if (now > scheduledEnqueueTimeUtc || (request.From == _lastRequestUser && !_isScheduleActive))
            {
                _lastRequestUtc = now;
                _isScheduleActive = false;
                
                await _busPublisher.PublishAsync(lightDisplay);
            }
            else
            {
                _isScheduleActive = true;
                _lastRequestUtc = scheduledEnqueueTimeUtc;

                await _busPublisher.PublishAsync(lightDisplay, scheduledEnqueueTimeUtc);
            }

            _lastRequestUser = request.From;

            metric.IsValidCommand = true;
            metric.DisplayType = lightDisplay.GetType().Name;
            metric.Command = lightDisplay;
            metric.IsScheduled = _isScheduleActive;
            metric.ScheduledAtUtc = _isScheduleActive ? (DateTime?) scheduledEnqueueTimeUtc : null;
            metric.ScheduledDelaySecs = _isScheduleActive ? (double?)(scheduledEnqueueTimeUtc - now).TotalSeconds : null;

            await _metricWriter.SendAsync(metric);
        }
    }
}