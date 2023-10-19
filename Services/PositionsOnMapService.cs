using System.Text.Json;
using TamboliyaApi.GameLogic;
using TamboliyaApi.GameLogic.Models;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Services
{
	public class PositionsOnMapService
	{
		public Dictionary<string, List<string>> PositionsOnMapCollection { get; private set; } = null!;


		public PositionsOnMapService()
		{
			Task.Run(InitProphecies).Wait();
		}

		private async Task InitProphecies()
		{
			string jsonPath = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix, "maps.json");
			using (FileStream jsonLoad = File.Open(jsonPath, FileMode.Open))
			{
				var positionsOnMapCollection = (Dictionary<string, List<string>>?)await JsonSerializer.DeserializeAsync(utf8Json: jsonLoad, returnType: typeof(Dictionary<string, List<string>>));

				PositionsOnMapCollection = positionsOnMapCollection ?? new Dictionary<string, List<string>>();
			}
		}

		public string GetPositionDescription(RegionOnMap regionOnMap, int positionStartWith)
		{
			if (regionOnMap < RegionOnMap.Delusion || regionOnMap > RegionOnMap.PersonalPath || positionStartWith == default)
			{
				throw new ArgumentException("Type of region or position value not set");
			}

			string regionName = regionOnMap switch  {
				RegionOnMap.Delusion => "Map_Delusion",
				RegionOnMap.InnerHomePath => "Map_PathInnerHome",
				RegionOnMap.OrganizationalPath => "Map_OrganizationalPath",
				RegionOnMap.PersonalPath => "Map_PersonalPath",
				RegionOnMap.MysticalPath => "Map_MysticalPath",
				RegionOnMap.LandOfClarity => "Map_LandOfClarity",
				RegionOnMap.Embodiment => "Map_Embodiment",
				_ => throw new ArgumentException("regionOnMap isn't correct")
			};

			var description = PositionsOnMapCollection[regionName]
				.First(l => l.StartsWith(positionStartWith + " —"));

			int separatorIndex = (description.IndexOf('—')) != -1 ? description.IndexOf('—') + 2 : 0;
			var textMessage = description.AsSpan()[separatorIndex..(description.Length - 1)].ToString();

			return textMessage;


            //TODO: ошибка при броске кости, когда игрок находится в зоне личностного пути. Нет названия новой позиции на земле ясности, а описание обрезано. Пример лога:
//            Позиція на шляху LandOfClarity -Почти никогда вашим партнером не будет другой человек.Чаще это причина, по которой Вам нужны другие люди, или по которой вы отвергаете и
//Подія на шляху PersonalPath - Гравець зробив новий хід.Зона на карті NotSet, номер нової позиції - , спосіб пересування -RandomAction
//Позиція на шляху - Возвращение.Место, откуда вы пришли, все еще там и будет там, когда вы закончите свое приключение здесь
//Шлях входу - Личностный путь.Путь вашего личностного роста, развития

        }
	}
}
