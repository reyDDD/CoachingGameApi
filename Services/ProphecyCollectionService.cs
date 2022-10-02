using System;
using System.Text.Json;
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
				string jsonPath = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix, "cards.json");
				using (FileStream jsonLoad = File.Open(jsonPath, FileMode.Open))
				{
					var prophecyCollection = (Dictionary<string, List<string>>?)await JsonSerializer.DeserializeAsync(utf8Json: jsonLoad, returnType: typeof(Dictionary<string, List<string>>));

					PropheciesCollection[color] = prophecyCollection![color.ToString()];
				}
			}

			var index = randomService.RandomNumber(0, PropheciesCollection[color].Count() - 1);
			var prophecy = PropheciesCollection[color][index];
			PropheciesCollection[color].RemoveAt(index);
			return prophecy;
		}
	}
}
