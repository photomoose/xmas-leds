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
            var matches = Regex.Match(text, "(\\d{1,3}), ?(\\d{1,3}), ?(\\d{1,3})", RegexOptions.Compiled);

            if (matches.Success)
            {
                var colour = new Colour(
                    "Custom",
                    byte.Parse(matches.Groups[1].Value),
                    byte.Parse(matches.Groups[2].Value),
                    byte.Parse(matches.Groups[3].Value));

                return new LightDisplay(new[] {colour});
            }
            else
            {
                text = StripTwitterUsernames(text);

                var potentialColours = SplitIntoWords(text)
                                        .Select(c => c.ToLower())
                                        .ToList();
                
                var matchingColours = (await _colourRepository.FindColoursAsync(potentialColours.Distinct())).ToDictionary(c => c.Name);

                var matchedColours = new List<Colour>();
                
                foreach (var potentialColour in potentialColours)
                {
                    if (matchingColours.ContainsKey(potentialColour))
                    {
                        matchedColours.Add(matchingColours[potentialColour]);
                    }
                }

                if (matchedColours.Any())
                {
                    return new LightDisplay(matchedColours);
                }
                else
                {
                    return null;
                }
            }
        }

        private string StripTwitterUsernames(string text)
        {
            return Regex.Replace(text, "@(\\w){1,15}\\s+", string.Empty);
        }

        private static IEnumerable<string> SplitIntoWords(string text)
        {
            return Regex.Split(text, ",\\s*");
        }

        private static bool IsNotATwitterUsername(string text)
        {
            return !text.StartsWith("@");
        }
    }
}
