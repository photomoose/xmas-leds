using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rumr.DurryLights.Domain
{
    public class LightDisplayParser
    {
        private readonly IColourRepository _colourRepository;

        public LightDisplayParser(IColourRepository colourRepository)
        {
            _colourRepository = colourRepository;
        }

        public async Task<LightDisplay> ParseAsync(string text)
        {
            var words = SplitIntoWords(text)
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

            return matchedColours.Any() ? new LightDisplay(matchedColours) : null;
        }

        private static IEnumerable<string> SplitIntoWords(string text)
        {
            return Regex.Split(text, ",\\s*");
        }
    }
}
