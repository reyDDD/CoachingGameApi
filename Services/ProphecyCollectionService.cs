using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;

namespace TamboliyaApi.Services
{
    public class ProphecyCollectionService
    {
        public Dictionary<Color, List<string>> PropheciesCollection { get; set; } = new();
		private readonly RandomService randomService;

		public ProphecyCollectionService(RandomService randomService)
        {
            InitProphecies();
            this.randomService = randomService;
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
                var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
                PropheciesCollection[color] = (await File.ReadAllLinesAsync(Path.Combine(rootFolder, $"{color}.txt"))).ToList();
            }

            var index = randomService.RandomNumber(0, PropheciesCollection[color].Count() - 1);
            var prophecy = PropheciesCollection[color][index];
            PropheciesCollection[color].RemoveAt(index);
            return prophecy;
        }
    }
}
