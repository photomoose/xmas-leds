using System.ComponentModel.DataAnnotations;

namespace Rumr.DurryLights.Sql
{
    public class Colour
    {
        [Key]
        public string Name { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }
        
        public byte B { get; set; }
    }
}
