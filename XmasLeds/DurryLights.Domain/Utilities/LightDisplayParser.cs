using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain.Commands;
using Rumr.DurryLights.Domain.Repositories;

namespace Rumr.DurryLights.Domain.Utilities
{
    internal class LightDisplayParser
    {
        private readonly IColourRepository _colourRepository;
        private readonly LightDisplayFactory _lightDisplayFactory;

        public LightDisplayParser(IColourRepository colourRepository)
        {
            _colourRepository = colourRepository;
            _lightDisplayFactory = new LightDisplayFactory();
        }

        public async Task<LightDisplay> ParseAsync(string text)
        {
            text = text.ToLower().Trim();

            var words = SplitWords(text).ToList();

            var lightDisplay = _lightDisplayFactory.Create(words[0]);

            var colours = (await _colourRepository.GetColoursAsync()).ToList();

            foreach (var word in words)
            {
                var matchingColour = colours.Find(c => c.Name == word);

                if (matchingColour != null)
                {
                    lightDisplay.AddColour(matchingColour.HexValue);
                }
            }

            return lightDisplay;
        }

        private static IEnumerable<string> SplitWords(string text)
        {
            text = Regex.Replace(text, "[^a-z ]", string.Empty);

            var words = Regex.Split(text, "\\s+");

            return words;
        }
    }
}
