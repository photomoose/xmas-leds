using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Repositories;

namespace Rumr.DurryLights.Sql
{
    public class ColourRepository : IColourRepository
    {
        private readonly string _connectionString;

        public ColourRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Colour>> GetColoursAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("colours");

            var blob = container.GetBlockBlobReference("colours.txt");

            var contents = await blob.DownloadTextAsync();

            var colours = new List<Colour>();

            using (var sr = new StringReader(contents))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    colours.Add(new Colour(parts[0], parts[1]));
                }
            }

            return colours;
        }
    }
}