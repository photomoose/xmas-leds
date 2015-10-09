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

        [Test]
        public async Task Should_Parse_Valid_Colour()
        {
            var white = new Colour("white", 255, 255, 255);
            GivenTheColourRepositoryReturns(white);
            
            var lightDisplay = await _parser.ParseAsync("white");

            lightDisplay.Should().NotBeNull();
            lightDisplay.Colour.Should().Be(white);
        }

        [TestCase("50,100,150")]
        [TestCase("50, 100, 150")]
        public async Task Should_Parse_Rgb_Values(string text)
        {
            var lightDisplay = await _parser.ParseAsync(text);

            lightDisplay.Colour.Should().NotBeNull();
            lightDisplay.Colour.R.Should().Be(50);
            lightDisplay.Colour.G.Should().Be(100);
            lightDisplay.Colour.B.Should().Be(150);
            lightDisplay.Colour.Name.Should().Be("Custom");
        }

        [Test]
        public async Task Should_Invoke_Colour_Repository_With_Correct_Args()
        {
            await _parser.ParseAsync("black");

            _colourRepository.Received().FindColourAsync("black");
        }

        [Test]
        public async Task Should_Return_Null()
        {
            GivenTheColourRepositoryReturns(null);

            var lightDisplay = await _parser.ParseAsync("invalid");

            lightDisplay.Should().BeNull();
        }

        private void GivenTheColourRepositoryReturns(Colour colour)
        {
            _colourRepository
                .FindColourAsync(Arg.Any<string>())
                .Returns(Task.FromResult(colour));
        }
    }
}
