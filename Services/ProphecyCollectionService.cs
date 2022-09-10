using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using TamboliyaApi.Data;

namespace TamboliyaApi.Services
{
    public class ProphecyCollectionService
    {
        public Dictionary<Color, List<string>> PropheciesCollection { get; set; } = new();

        public ProphecyCollectionService()
        {
            InitProphecies();
        }

        public void InitProphecies()
        {
            PropheciesCollection.Add(Color.Red, new());
            PropheciesCollection.Add(Color.Green, new());
            PropheciesCollection.Add(Color.Blue, new());
            PropheciesCollection.Add(Color.Yellow, new());

        }

        public async Task<string> GetProphecyAsync(Color color)
        {
            if (color == Color.NotSet)
            {
                throw new ArgumentException("Color card not set");
            }

            if (PropheciesCollection[color].Count() == 0)
            {
                var rootFolder = Directory.GetCurrentDirectory();
                PropheciesCollection[color] = (await File.ReadAllLinesAsync(Path.Combine(rootFolder, "Cards", @$"{color}.txt"))).ToList();
            }

            var random = new Random();
            var index = random.Next(0, PropheciesCollection[color].Count());
            var prophecy = PropheciesCollection[color][index];
            PropheciesCollection[color].RemoveAt(index);
            return prophecy;
        }
    }
}
