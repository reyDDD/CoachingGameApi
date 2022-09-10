using TamboliyaApi.GameLogic.Models;

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

                    newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = prophecy;
                    newPosition.PositionNumber = 1;
                    break;
                case 2:
                    path = Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath);
                    prophecies = (await File.ReadAllLinesAsync(path)).ToList();
                    prophecy = prophecies.Where(m => m.StartsWith("17 — ")).First();

                    newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = prophecy;
                    newPosition.PositionNumber = 17;
                    break;
                case 3:
                    //ToDo: реализовать ветвь Просветление. Вы можете в любой момент переместить фишку на любое поле карты по собственному выбору.
                    break;
                case 4:
                    //ToDo: реализовать ветвь Время подумать о чем-то другом. Пропустите ход.
                    break;
                case 5:
                    //ToDo: реализовать ветвь Глубокая медитация: пропустите ход.
                    break;
                case 6:
                    var prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapLandOfClarityPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.LandOfClarity;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 7:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapEmbodimentPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.Embodiment;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 8:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapDelusionPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.Delusion;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 9:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapMysticalPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.MysticalPath;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 10:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapOrganizationalPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.OrganizationalPath;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 11:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapInnerHomePath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.InnerHomePath;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
                case 12:
                    prophecyInfo = await Oracle.StepWithNumber(GamePathes.mapPersonalPath, dodecahedron);

                    newPosition.RegionOnMap = RegionOnMap.PersonalPath;
                    newPosition.Description = prophecyInfo.Prophecy;
                    newPosition.PositionNumber = prophecyInfo.Number;
                    break;
            }

            return newPosition;
        }
    }
}
