using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.DurryLights.Domain;

namespace DurryLights.Domain.Tests
{
    public class LightDisplayParserTests
    {
        private IColourRepository _colourRepository;
        private LightDisplayParser _parser;

        [SetUp]
        public void SetUp()
        {
            _colourRepository = Substitute.For<IColourRepository>();
            _parser = new LightDisplayParser(_colourRepository);
        }

        [TestCase("white")]
        public async Task Should_Parse_Valid_Colour(string text)
        {
            var white = new Colour("white", 255, 255, 255);
            GivenTheColourRepositoryReturns(white);
            
            var lightDisplay = await _parser.ParseAsync("white");

            lightDisplay.Should().NotBeNull();
            lightDisplay.Colours.Count.Should().Be(1);
            lightDisplay.Colours[0].Should().Be(white.HexValue);
        }

        [Test]
        public async Task Should_Parse_Multiple_Colours()
        {
            var white = new Colour("white", 255, 255, 255);
            var red = new Colour("red", 255, 0, 0);

            GivenTheColourRepositoryReturns(white, red);

            var lightDisplay = await _parser.ParseAsync("white, red, white");

            lightDisplay.Should().NotBeNull();
            lightDisplay.Colours.Count.Should().Be(3);
            lightDisplay.Colours[0].Should().Be(white.HexValue);
            lightDisplay.Colours[1].Should().Be(red.HexValue);
            lightDisplay.Colours[2].Should().Be(white.HexValue);
        }

        [TestCase("50,100,150", "326496")]
        [TestCase("50, 100, 150", "326496")]
        [TestCase("0, 0, 0", "000000")]
        public async Task Should_Parse_Rgb_Values(string text, string value)
        {
            var lightDisplay = await _parser.ParseAsync(text);

            lightDisplay.Colours.Count.Should().Be(1);
            lightDisplay.Colours[0].Should().Be(value);
        }

        [TestCase("black")]
        [TestCase("BLACK")]
        public async Task Should_Invoke_Colour_Repository_With_Single_Colour(string text)
        {
            await _parser.ParseAsync(text);

            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("black")));
        }

        [TestCase("black, red")]
        [TestCase("black,red")]
        public async Task Should_Invoke_Colour_Repository_With_Multiple_Colours(string text)
        {
            await _parser.ParseAsync(text);

            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("black")));
            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("red")));
        }

        [Test]
        public async Task Should_Invoke_Colour_Repository_With_Distinct_Colours()
        {
            await _parser.ParseAsync("blue, blue, blue");

            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Count() == 1));
        }

        [Test]
        public async Task Should_Invoke_Colour_Repository_Without_Twitter_Username()
        {
            await _parser.ParseAsync("@durrylights black");

            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => !c.Contains("@durrylights")));
            _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("black")));
        }

        [Test]
        public async Task Should_Return_No_Colours()
        {
            GivenTheColourRepositoryReturnsNoMatches();

            var lightDisplay = await _parser.ParseAsync("invalid");

            lightDisplay.Should().BeNull();
        }

        [Test]
        public async Task Should_Order_Colours_By_Request()
        {
            var white = new Colour("white", 255, 255, 255);
            var red = new Colour("red", 255, 0, 0);
            var blue = new Colour("blue", 0, 255, 0);

            GivenTheColourRepositoryReturns(blue, white, red);

            var lightDisplay = await _parser.ParseAsync("white, red, blue");

            lightDisplay.Colours[0].Should().Be(white.HexValue);
            lightDisplay.Colours[1].Should().Be(red.HexValue);
            lightDisplay.Colours[2].Should().Be(blue.HexValue);
        }

        private void GivenTheColourRepositoryReturns(params Colour[] colours)
        {
            _colourRepository
                .FindColoursAsync(Arg.Any<IEnumerable<string>>())
                .Returns(Task.FromResult<IEnumerable<Colour>>(colours));
        }

        private void GivenTheColourRepositoryReturnsNoMatches()
        {
            _colourRepository
                .FindColoursAsync(Arg.Any<IEnumerable<string>>())
                .Returns(Task.FromResult(Enumerable.Empty<Colour>()));
        }
    }
}
