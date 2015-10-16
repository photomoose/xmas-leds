using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Rumr.DurryLights.Domain;

namespace DurryLights.Domain.Tests
{
    public class LightDisplayParserSpecs
    {
        [TestFixture]
        public class When_Parsing_String_Containing_Single_Colour
        {
            private IColourRepository _colourRepository;
            private LightDisplayParser _parser;
            private readonly Colour _white = new Colour("white", 255, 255, 255);


            [SetUp]
            public void SetUp()
            {
                _colourRepository = Substitute.For<IColourRepository>();
                _parser = new LightDisplayParser(_colourRepository);

                GivenTheColourRepositoryReturns(_white);
            }

            [Test]
            public async Task Result_Should_Contain_Hex_Value_For_Colour()
            {
                var result = await _parser.ParseAsync("white");

                result.Should().NotBeNull();
                result.Colours.Count.Should().Be(1);
                result.Colours[0].Should().Be("FFFFFF");
            }

            [TestCase("black")]
            [TestCase("BLACK")]
            public async Task Should_Invoke_Colour_Repository_With_Lowercase_Colour_Name(string text)
            {
                await _parser.ParseAsync(text);

                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("black")));
            }

            private void GivenTheColourRepositoryReturns(params Colour[] colours)
            {
                _colourRepository
                    .FindColoursAsync(Arg.Any<IEnumerable<string>>())
                    .Returns(Task.FromResult<IEnumerable<Colour>>(colours));
            }
        }

        [TestFixture]
        public class When_Parsing_String_Containing_Multiple_Colours
        {
            private IColourRepository _colourRepository;
            private LightDisplayParser _parser;
            private readonly Colour _white = new Colour("white", 255, 255, 255);
            private readonly Colour _red = new Colour("red", 255, 0, 0);
            private readonly Colour _blue = new Colour("blue", 0, 0, 255);

            [SetUp]
            public void SetUp()
            {
                _colourRepository = Substitute.For<IColourRepository>();
                _parser = new LightDisplayParser(_colourRepository);

                GivenTheColourRepositoryReturns(_white, _red, _blue);
            }

            [Test]
            public async Task Result_Should_Contain_Hex_Values_For_Each_Colour()
            {
                var lightDisplay = await _parser.ParseAsync("white, red, blue");

                lightDisplay.Colours.Should().Contain("FFFFFF");
                lightDisplay.Colours.Should().Contain("FF0000");
                lightDisplay.Colours.Should().Contain("0000FF");
            }

            [Test]
            public async Task Result_Should_Have_Hex_Values_Ordered_By_Colours_In_String()
            {
                var lightDisplay = await _parser.ParseAsync("red, white, blue");

                lightDisplay.Colours[0].Should().Be("FF0000");
                lightDisplay.Colours[1].Should().Be("FFFFFF");
                lightDisplay.Colours[2].Should().Be("0000FF");
            }

            [Test]
            public async Task Should_Invoke_Colour_Repository_With_Multiple_Colours()
            {
                await _parser.ParseAsync("white, red, blue");

                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("white")));
                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("red")));
                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("blue")));
            }

            [Test]
            public async Task Should_Invoke_Colour_Repository_With_Colours_Containing_Spaces()
            {
                await _parser.ParseAsync("white, dark red, blue");

                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("white")));
                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("dark red")));
                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Contains("blue")));
            }

            [Test]
            public async Task Should_Invoke_Colour_Repository_With_Distinct_Colours()
            {
                await _parser.ParseAsync("blue, blue, blue");

                _colourRepository.Received().FindColoursAsync(Arg.Is<IEnumerable<string>>(c => c.Count() == 1));
            }

            private void GivenTheColourRepositoryReturns(params Colour[] colours)
            {
                _colourRepository
                    .FindColoursAsync(Arg.Any<IEnumerable<string>>())
                    .Returns(Task.FromResult<IEnumerable<Colour>>(colours));
            }
        }

        [TestFixture]
        public class When_Parsing_String_With_No_Matching_Colours
        {
            private IColourRepository _colourRepository;
            private LightDisplayParser _parser;
            
            [SetUp]
            public void SetUp()
            {
                _colourRepository = Substitute.For<IColourRepository>();
                _parser = new LightDisplayParser(_colourRepository);

                GivenTheColourRepositoryReturnsNoMatches();
            }

            [Test]
            public async Task Result_Should_Be_Null()
            {

                var lightDisplay = await _parser.ParseAsync("no matching colours");

                lightDisplay.Should().BeNull();
            }

            private void GivenTheColourRepositoryReturnsNoMatches()
            {
                _colourRepository
                    .FindColoursAsync(Arg.Any<IEnumerable<string>>())
                    .Returns(Task.FromResult(Enumerable.Empty<Colour>()));
            }
        }
    }
}
