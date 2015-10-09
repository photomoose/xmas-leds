using System.Data.Entity;

namespace Rumr.DurryLights.Sql
{
    public class LightsContext : DbContext
    {
        public LightsContext() : base("LightsContext")
        {
            var ensureDllIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public DbSet<Colour> Colours { get; set; } 
    }
}
