using System.Runtime.InteropServices;
using TamboliyaApi.Data;

namespace TamboliyaApi.Services
{
    public class ProphecyCollectionService
    {
        public Color Color { get; init; }

        public List<string> Prophecies { get; set; } = new List<string>();

        public ProphecyCollectionService(Color color)
        {
            Color = color;
        }
        public async Task<string> GetProphecyAsync()
        {
            if (Color == Color.NotSet)
            {
                throw new ArgumentException("Color card not set");
            }

            if (Prophecies.Count() == 0)
            {
                var rootFolder = Directory.GetCurrentDirectory();
                Prophecies = (await File.ReadAllLinesAsync(Path.Combine(rootFolder, "Cards", @$"{Color}.txt"))).ToList();
            }

            var random = new Random();
            var index = random.Next(0, Prophecies.Count());
            var prophecy = Prophecies[index];
            Prophecies.RemoveAt(index);
            return prophecy;
        }

        public static ProphecyCollectionService Create(Color color) => new ProphecyCollectionService(color);
    }
}
