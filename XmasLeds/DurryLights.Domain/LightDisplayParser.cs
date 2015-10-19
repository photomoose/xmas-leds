using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rumr.DurryLights.Domain.LightDisplays;

namespace Rumr.DurryLights.Domain
{
    public class LightDisplayParser
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
            var firstSpace = text.IndexOf(" ", StringComparison.InvariantCulture);
            var firstWord = firstSpace > 0 ? text.Substring(0, firstSpace) : text;

            var lightDisplay = _lightDisplayFactory.Create(firstWord);

            if (!(lightDisplay is DefaultLightDisplay))
            {
                text = text.Substring(firstSpace);
            }

            var colours = await FindMatchingColours(text);
            lightDisplay.AddColours(colours);

            return lightDisplay.Colours.Count > 0 ? lightDisplay : null;
        }

        private async Task<List<Colour>> FindMatchingColours(string text)
        {
            var words = SplitIntoWords(text.Trim())
                .Select(c => c.ToLower())
                .ToList();

            var matchingColours = (await _colourRepository.FindColoursAsync(words.Distinct())).ToDictionary(c => c.Name);

            var matchedColours = new List<Colour>();

            foreach (var potentialColour in words)
            {
                if (matchingColours.ContainsKey(potentialColour))
                {
                    matchedColours.Add(matchingColours[potentialColour]);
                }
            }

            return matchedColours;
        }

        private static IEnumerable<string> SplitIntoWords(string text)
        {
            return Regex.Split(text, ",\\s*");
        }
    }
}
