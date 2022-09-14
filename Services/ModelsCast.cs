using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Services
{
	public static class ModelsCast
	{
		private static string prefixImages = "images/";

		public static string GetPath(RegionOnMap regionOnMap, int stepOnPath)
		{
			string path = regionOnMap switch
			{
				RegionOnMap.MysticalPath => stepOnPath switch
				{
					> 0 and <= 8 => GamePathes.MysticalPath_1_8,
					> 8 and <= 12 => GamePathes.MysticalPath_9_12,
				},
				RegionOnMap.Embodiment => GamePathes.EmbodimentPath,
				RegionOnMap.LandOfClarity => GamePathes.LandOfClarityPath,
				RegionOnMap.Delusion => GamePathes.DelusionPath,
				RegionOnMap.InnerHomePath => GamePathes.IllusionPath,
				RegionOnMap.PersonalPath => GamePathes.IllusionPath,
				RegionOnMap.OrganizationalPath => GamePathes.IllusionPath,
				_ => throw new ArgumentException("branch is unknown")
			};

			return prefixImages + path;
		}


		public static Coordinates GetCoordinates(RegionOnMap regionOnMap, int stepOnPath)
		{
			return regionOnMap switch
			{
				RegionOnMap.MysticalPath => stepOnPath switch
				{
					1 => new Coordinates(542, 465),
					2 => new Coordinates(520, 375),
					3 => new Coordinates(530, 320),
					4 => new Coordinates(538, 273),
					5 => new Coordinates(551, 199),
					6 => new Coordinates(521, 149),
					7 => new Coordinates(537, 92),
					8 => new Coordinates(532, 26),
					9 => new Coordinates(393, 186),
					10 => new Coordinates(538, 284),
					11 => new Coordinates(535, 150),
					12 => new Coordinates(532, 60),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.Embodiment => stepOnPath switch
				{
					1 => new Coordinates(700, 409),
					2 => new Coordinates(745, 331),
					3 => new Coordinates(811, 413),
					4 => new Coordinates(815, 344),
					5 => new Coordinates(912, 401),
					6 => new Coordinates(870, 325),
					7 => new Coordinates(814, 224),
					8 => new Coordinates(816, 176),
					9 => new Coordinates(818, 144),
					10 => new Coordinates(815, 112),
					11 => new Coordinates(818, 51),
					12 => new Coordinates(823, 12),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.LandOfClarity => stepOnPath switch
				{
					1 => new Coordinates(394, 484),
					2 => new Coordinates(417, 201),
					3 => new Coordinates(484, 269),
					4 => new Coordinates(457, 391),
					5 => new Coordinates(526, 416),
					6 => new Coordinates(609, 446),
					7 => new Coordinates(842, 445),
					8 => new Coordinates(909, 383),
					9 => new Coordinates(740, 237),
					10 => new Coordinates(829, 268),
					11 => new Coordinates(1077, 257),
					12 => new Coordinates(11024, 338),
					13 => new Coordinates(916, 290),
					14 => new Coordinates(799, 75),
					15 => new Coordinates(1024, 190),
					16 => new Coordinates(906, 39),
					17 => new Coordinates(1119, 501),
					18 => new Coordinates(1110, 568),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.Delusion => stepOnPath switch
				{
					1 => new Coordinates(175, 568),
					2 => new Coordinates(235, 524),
					3 => new Coordinates(238, 449),
					4 => new Coordinates(202, 355),
					5 => new Coordinates(230, 293),
					6 => new Coordinates(242, 238),
					7 => new Coordinates(102, 184),
					8 => new Coordinates(93, 134),
					9 => new Coordinates(366, 176),
					10 => new Coordinates(388, 119),
					11 => new Coordinates(252, 150),
					12 => new Coordinates(244, 96),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.InnerHomePath => stepOnPath switch
				{
					1 => new Coordinates(178, 280),
					2 => new Coordinates(178, 261),
					3 => new Coordinates(178, 241),
					4 => new Coordinates(174, 152),
					5 => new Coordinates(209, 152),
					6 => new Coordinates(244, 152),
					7 => new Coordinates(276, 152),
					8 => new Coordinates(309, 152),
					9 => new Coordinates(189, 123),
					10 => new Coordinates(192, 104),
					11 => new Coordinates(196, 93),
					12 => new Coordinates(243, 72),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.PersonalPath => stepOnPath switch
				{
					1 => new Coordinates(438, 388),
					2 => new Coordinates(460, 420),
					3 => new Coordinates(416, 446),
					4 => new Coordinates(400, 390),
					5 => new Coordinates(450, 360),
					6 => new Coordinates(500, 380),
					7 => new Coordinates(496, 448),
					8 => new Coordinates(430, 484),
					9 => new Coordinates(360, 450),
					10 => new Coordinates(370, 370),
					11 => new Coordinates(425, 307),
					12 => new Coordinates(479, 298),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.OrganizationalPath => stepOnPath switch
				{
					1 => new Coordinates(59, 531),
					2 => new Coordinates(51, 511),
					3 => new Coordinates(39, 492),
					4 => new Coordinates(46, 480),
					5 => new Coordinates(28, 455),
					6 => new Coordinates(30, 445),
					7 => new Coordinates(19, 426),
					8 => new Coordinates(32, 410),
					9 => new Coordinates(32, 393),
					10 => new Coordinates(23, 376),
					11 => new Coordinates(31, 364),
					12 => new Coordinates(64, 305),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				_ => throw new ArgumentException("branch is unknown")
			};
		}

		public static OracleDTO InitialGameDataToOracleDTO(this InitialGameData oracle)
		{
			OracleDTO oracleDTO = new()
			{
				GameId = oracle.GameId,
				Question = oracle.Question,
				Motive = oracle.Motive,
				QualityOfExperience = oracle.QualityOfExperience,
				EnvironmentAndCircumstances = oracle.EnvironmentAndCircumstances,
				ChainLinks = oracle.ChainLinks,
				ExitPath = oracle.ExitPath,
				StepOnPath = oracle.StepOnPath,
				RegionOnMap = oracle.RegionOnMap,
				PathToImage = GetPath(oracle.RegionOnMap, oracle.StepOnPath),
				Coordinates = GetCoordinates(oracle.RegionOnMap, oracle.StepOnPath)
			};

			return oracleDTO;
		}




		public static Game NewGameToGame(this NewGame newGame)
		{
			InitialGameData oracle = new()
			{
				Question = newGame.Oracle.Question,
				Motive = newGame.Oracle.Motive,
				QualityOfExperience = newGame.Oracle.QualityOfExperience,
				EnvironmentAndCircumstances = newGame.Oracle.EnvironmentAndCircumstances,
				ChainLinks = newGame.Oracle.ChainLinks,
				ExitPath = newGame.Oracle.ExitPath,
				StepOnPath = newGame.Oracle.StepOnPath,
				RegionOnMap = newGame.Oracle.RegionOnMap
			};

			Game game = new()
			{
				InitialGameData = oracle,
				IsFinished = false,
				ActualPosition = new()
				{
					Description = newGame.ActualPosition.Description,
					PositionNumber = newGame.ActualPosition.PositionNumber,
					RegionOnMap = newGame.ActualPosition.RegionOnMap
				},

			};
			return game;
		}


		public static GameDTO GameToGameDTO(this Game newGame,
			List<ActualPositionOnMap>? actualPositionsForSelect = null)
		{
			var listPositionForSelect = actualPositionsForSelect ?? new();

			GameDTO game = new()
			{
				GameId = newGame.Id,
				IsFinished = newGame.IsFinished,
				ActualPosition = new()
				{
					Description = newGame.ActualPosition.Description,
					PositionNumber = newGame.ActualPosition.PositionNumber,
					RegionOnMap = newGame.ActualPosition.RegionOnMap,
					IsSelected = null
				},
				Oracle = InitialGameDataToOracleDTO(newGame.InitialGameData),
				ActualPositionsForSelect = listPositionForSelect,
				PathToImage = GetPath(newGame.ActualPosition.RegionOnMap, newGame.ActualPosition.PositionNumber),
				Coordinates = GetCoordinates(newGame.ActualPosition.RegionOnMap, newGame.ActualPosition.PositionNumber)
			};
			return game;
		}


		public static ActualPositionOnMap ActualPositionOnMapToDTO(this ActualPositionOnTheMap game)
		{
			ActualPositionOnMap oracleDTO = new()
			{
				RegionOnMap = game.RegionOnMap,
				Description = game.Description,
				PositionNumber = game.PositionNumber
			};

			return oracleDTO;
		}
	}
}
