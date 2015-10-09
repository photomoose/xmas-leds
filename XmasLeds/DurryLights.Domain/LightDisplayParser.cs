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
            Colour colour;
            var matches = Regex.Match(text, "(\\d{1,3}), ?(\\d{1,3}), ?(\\d{1,3})", RegexOptions.Compiled);

            if (matches.Success)
            {
                colour = new Colour(
                    "Custom",
                    byte.Parse(matches.Groups[1].Value),
                    byte.Parse(matches.Groups[2].Value),
                    byte.Parse(matches.Groups[3].Value));
            }
            else
            {
                colour = await _colourRepository.FindColourAsync(text);
            }

            return colour != null ? new LightDisplay(colour) : null;
        }
    }
}
