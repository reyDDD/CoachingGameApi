using TamboliyaApi.GameLogic.Models;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.GameLogic
{
    public class ChooseRandomActionService
    {
        private readonly Dodecahedron dodecahedron;


        public ChooseRandomActionService(Dodecahedron dodecahedron)
        {
            this.dodecahedron = dodecahedron;
        }
        public async Task<ActualPositionOnMap> ChooseAsync()
        {
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
            var sideOfBone = dodecahedron.ThrowBone();
            ActualPositionOnMap newPosition = new();

            switch (sideOfBone.Number)
            {
                case 1:
                    var path = Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath);
                    var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
                    string prophecy = prophecies.Where(m => m.StartsWith("1 — ")).First();

					int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
					var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = 1;
                    break;
                case 2:
                    path = Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath);
                    prophecies = (await File.ReadAllLinesAsync(path)).ToList();
                    prophecy = prophecies.Where(m => m.StartsWith("17 — ")).First();

					separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = 17;
                    break;
                case 3:
                    //ToDo: реализовать ветвь Просветление. Вы можете в любой момент переместить фишку на любое поле карты по собственному выбору.
                    //break;
                case 4:
                    //ToDo: реализовать ветвь Время подумать о чем-то другом. Пропустите ход.
                    //break;
                case 5:
                    //ToDo: реализовать ветвь Глубокая медитация: пропустите ход.
                    //break;
                case 6:
                    var prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapLandOfClarityPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 7:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapEmbodimentPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.Embodiment;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 8:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapDelusionPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.Delusion;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 9:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapMysticalPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.MysticalPath;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 10:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapOrganizationalPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.OrganizationalPath;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 11:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapInnerHomePath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.InnerHomePath;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 12:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapPersonalPath, dodecahedron);

					separatorIndex = (prophecyInfo.Prophecy.IndexOf('—')) != -1 ? prophecyInfo.Prophecy.IndexOf('—') + 2 : 0;
					textMessage = prophecyInfo.Prophecy.AsSpan()[separatorIndex..(prophecyInfo.Prophecy.Length - 1)].ToString();

					newPosition.RegionOnMap = RegionOnMap.PersonalPath;
                    newPosition.Description = textMessage;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
            }

            return newPosition;
        }
    }
}
