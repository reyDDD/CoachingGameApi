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
        public string GetProphecy()
        {
            if (Color == Color.NotSet)
            {
                throw new ArgumentException("Color card not set");
            }

            if (Prophecies.Count() == 0)
            {
                var rootFolder = Directory.GetCurrentDirectory();
                Prophecies = File.ReadAllLines(Path.Combine(rootFolder, "Cards", @$"{Color}.txt")).ToList();
            }

            var random = new Random();
            var index = random.Next(0, Prophecies.Count());
            return Prophecies[index];
        }

        public static ProphecyCollectionService Create(Color color) => new ProphecyCollectionService(color);
    }
}
