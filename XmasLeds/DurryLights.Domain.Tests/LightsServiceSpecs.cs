using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.Domain.Commands;
using Rumr.DurryLights.Domain.Messaging;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Repositories;
using Rumr.DurryLights.Domain.Services;
using Rumr.DurryLights.Domain.Utilities;

namespace DurryLights.Domain.Tests
{
    [TestFixture]
    public class LightsServiceSpecs
    {
        private IBusPublisher _busPublisher;
        private IColourRepository _colourRepository;
        private LightsService _lightsService;
        private IDateTimeProvider _dateTimeProvider;
        private IMetricWriter _metricWriter;

        [SetUp]
        public void SetUp()
        {
            _busPublisher = Substitute.For<IBusPublisher>();
            _colourRepository = Substitute.For<IColourRepository>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _metricWriter = Substitute.For<IMetricWriter>();
            _lightsService = new LightsService(_busPublisher, _colourRepository, _dateTimeProvider, _metricWriter);

            var red = new Colour("red", "ff0000");
            var green = new Colour("green", "00ff00");
            var blue = new Colour("blue", "0000ff");

            GivenTheColourRepositoryReturns(red, green, blue);

            _dateTimeProvider.UtcNow().Returns(new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestCase("red", "ff0000")]
        [TestCase("green.", "00ff00")]
        public async Task Single_Colour_Should_Publish_Default_Display_With_Single_Colour(string text, string hexValue)
        {
            var request = new LightsRequestBuilder()
                .ForText(text)
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<DefaultLightDisplay>(x => x.Colours.Contains(hexValue)));
        }

        [TestCase("red, green, blue")]
        [TestCase("red, green, blue.")]
        [TestCase("red green blue.")]
        [TestCase("RED GREEN BLUE.")]
        public async Task Multiple_Colours_Should_Publish_Default_Display_With_Multiple_Colours(string text)
        {
            var request = new LightsRequestBuilder()
                .ForText(text)
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<DefaultLightDisplay>(x =>
                x.Colours[0] == "ff0000" &&
                x.Colours[1] == "00ff00" &&
                x.Colours[2] == "0000ff"));
        }

        [Test]
        public async Task Unknown_Colours_Should_Be_Silently_Ignored()
        {
            var request = new LightsRequestBuilder()
                .ForText("blue, purple, yellow, red")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<DefaultLightDisplay>(x =>
                x.Colours[0] == "0000ff" &&
                x.Colours[1] == "ff0000"));
        }

        [Test]
        public async Task No_Recognised_Colours_Should_Not_Publish_Command()
        {
            var request = new LightsRequestBuilder()
                .ForText("unknown")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.DidNotReceive().PublishAsync(Arg.Any<DefaultLightDisplay>());
        }

        [Test]
        public async Task Fade_Should_Publish_Fade()
        {
            var request = new LightsRequestBuilder()
                .ForText("fade red, blue")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<FadingInOutLightDisplay>(x =>
                x.Colours[0] == "ff0000" &&
                x.Colours[1] == "0000ff"));
        }

        [Test]
        public async Task Strobe_Should_Publish_Strobe()
        {
            var request = new LightsRequestBuilder()
                .ForText("strobe red, green")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<StrobeLightDisplay>(x =>
                x.Colours[0] == "ff0000" &&
                x.Colours[1] == "00ff00"));
        }

        [Test]
        public async Task Flash_Should_Publish_Flash()
        {
            var request = new LightsRequestBuilder()
                .ForText("flash red, green")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<FlashingLightDisplay>(x =>
                x.Colours[0] == "ff0000" &&
                x.Colours[1] == "00ff00" &&
                x.Interval == 500));
        }

        [Test]
        public async Task Cycle_Should_Publish_Cycle()
        {
            var request = new LightsRequestBuilder()
                .ForText("cycle red, green")
                .Build();

            await _lightsService.HandleRequestAsync(request);

            _busPublisher.Received().PublishAsync(Arg.Is<CyclingLightDisplay>(x =>
                x.Colours[0] == "ff0000" &&
                x.Colours[1] == "00ff00"));
        }

        [Test]
        public async Task Multiple_Requests_For_Different_Users_Should_Be_Scheduled()
        {
            var request1 = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var request2 = new LightsRequestBuilder()
                .ForText("blue")
                .From("@durrylights")
                .Build();

            var now = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var scheduledTime = now.AddMinutes(1);
            _dateTimeProvider.UtcNow().Returns(now);

            await _lightsService.HandleRequestAsync(request1);
            await _lightsService.HandleRequestAsync(request2);

            _busPublisher.Received().PublishAsync(Arg.Is<DefaultLightDisplay>(x =>
                x.Colours[0] == "0000ff"), scheduledTime);
        }

        [Test]
        public async Task Multiple_Requests_For_Same_User_Should_Not_Be_Scheduled()
        {
            var request1 = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var request2 = new LightsRequestBuilder()
                .ForText("blue")
                .From("@photomoose")
                .Build();

            await _lightsService.HandleRequestAsync(request1);
            await _lightsService.HandleRequestAsync(request2);

            _busPublisher.DidNotReceive().PublishAsync(Arg.Is<DefaultLightDisplay>(x =>
                x.Colours[0] == "0000ff"), Arg.Any<DateTime>());
        }

        [Test]
        public async Task Multiple_Requests_For_Same_User_After_Existing_Schedule_Should_Be_Scheduled()
        {
            var request1 = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var request2 = new LightsRequestBuilder()
                .ForText("blue")
                .From("@rumr")
                .Build();

            var request3 = new LightsRequestBuilder()
                .ForText("green")
                .From("@rumr")
                .Build();

            await _lightsService.HandleRequestAsync(request1);
            await _lightsService.HandleRequestAsync(request2);
            await _lightsService.HandleRequestAsync(request3);

            _busPublisher.Received().PublishAsync(Arg.Is<DefaultLightDisplay>(x =>
                x.Colours[0] == "00ff00"), Arg.Any<DateTime>());
        }

        [Test]
        public async Task Valid_Request_Should_Send_Metric_To_Elastic_Search()
        {
            var request = new LightsRequestBuilder()
                .ForText("red, blue, green")
                .FromSource("twitter")
                .From("@photomoose")
                .Build();

            Metric metric = null;
            _metricWriter.SendAsync(Arg.Do<Metric>(a => metric = a));

            var now = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow().Returns(now);

            await _lightsService.HandleRequestAsync(request);

            metric.IsValidCommand.Should().BeTrue();
            metric.TimestampUtc.Should().Be(now);
            metric.RequestText.Should().Be("red, blue, green");
            metric.Source.Should().Be("twitter");
            metric.From.Should().Be("@photomoose");
            metric.DisplayType.Should().Be("DefaultLightDisplay");
            metric.Command.Colours[0].Should().Be("ff0000");
            metric.Command.Colours[1].Should().Be("0000ff");
            metric.Command.Colours[2].Should().Be("00ff00");
            metric.ScheduledAtUtc.Should().NotHaveValue();
            metric.ScheduledDelaySecs.Should().NotHaveValue();
        }

        [Test]
        public async Task Invalid_Request_Should_Send_Metric_To_Elastic_Search()
        {
            var request = new LightsRequestBuilder()
                .ForText("no valid colours")
                .FromSource("twitter")
                .From("@photomoose")
                .Build();

            Metric metric = null;
            _metricWriter.SendAsync(Arg.Do<Metric>(a => metric = a));

            var now = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            _dateTimeProvider.UtcNow().Returns(now);

            await _lightsService.HandleRequestAsync(request);

            metric.IsValidCommand.Should().BeFalse();
            metric.TimestampUtc.Should().Be(now);
            metric.RequestText.Should().Be("no valid colours");
            metric.Source.Should().Be("twitter");
            metric.From.Should().Be("@photomoose");
            metric.DisplayType.Should().BeNull();
            metric.Command.Should().BeNull();
            metric.ScheduledAtUtc.Should().NotHaveValue();
            metric.ScheduledDelaySecs.Should().NotHaveValue();
        }

        [Test]
        public async Task Scheduled_Requests_Should_Include_Schedule_In_Metric()
        {
            var request1 = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var request2 = new LightsRequestBuilder()
                .ForText("blue")
                .From("@durrylights")
                .Build();

            var now = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var scheduledTime = now.AddMinutes(1);

            _dateTimeProvider.UtcNow().Returns(now);
            await _lightsService.HandleRequestAsync(request1);

            Metric metric = null;
            _metricWriter.SendAsync(Arg.Do<Metric>(a => metric = a));

            _dateTimeProvider.UtcNow().Returns(now.AddSeconds(10));
            await _lightsService.HandleRequestAsync(request2);

            metric.IsScheduled.Should().BeTrue();
            metric.ScheduledAtUtc.Should().Be(scheduledTime);
            metric.ScheduledDelaySecs.Should().Be(60 - 10);
        }

        [Test]
        public async Task Given_Request_Is_Scheduled_Then_Service_Should_Return_Scheduled_Time()
        {
            var request1 = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var request2 = new LightsRequestBuilder()
                .ForText("blue")
                .From("@durrylights")
                .Build();

            var now = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var scheduledTime = now.AddMinutes(1);
            _dateTimeProvider.UtcNow().Returns(now);

            await _lightsService.HandleRequestAsync(request1);
            var result = await _lightsService.HandleRequestAsync(request2);

            result.IsSuccess.Should().BeTrue();
            result.IsScheduled.Should().BeTrue();
            result.ScheduledForUtc.Should().Be(scheduledTime);
        }

        [Test]
        public async Task Given_Request_Is_Not_Scheduled_Then_Service_Should_Return_Success()
        {
            var request = new LightsRequestBuilder()
                .ForText("red")
                .From("@photomoose")
                .Build();

            var result = await _lightsService.HandleRequestAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.IsScheduled.Should().BeFalse();
            result.ScheduledForUtc.Should().NotHaveValue();
        }

        private void GivenTheColourRepositoryReturns(params Colour[] colours)
        {
            _colourRepository.GetColoursAsync().Returns(Task.FromResult((IEnumerable<Colour>)colours));
        }
    }
}