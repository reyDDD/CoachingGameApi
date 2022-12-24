using System.Collections.ObjectModel;
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
			Coordinates coordinates = regionOnMap switch
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
					1 => new Coordinates(700, 415),
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
					1 => new Coordinates(335, 415),
					2 => new Coordinates(357, 166),
					3 => new Coordinates(418, 228),
					4 => new Coordinates(388, 340),
					5 => new Coordinates(452, 360),
					6 => new Coordinates(523, 383),
					7 => new Coordinates(732, 383),
					8 => new Coordinates(780, 333),
					9 => new Coordinates(635, 208),
					10 => new Coordinates(713, 234),
					11 => new Coordinates(930, 228),
					12 => new Coordinates(885, 295),
					13 => new Coordinates(790, 250),
					14 => new Coordinates(690, 65),
					15 => new Coordinates(890, 170),
					16 => new Coordinates(780, 38),
					17 => new Coordinates(960, 425),
					18 => new Coordinates(960, 480),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.Delusion => stepOnPath switch
				{
					1 => new Coordinates(174, 490),
					2 => new Coordinates(202, 448),
					3 => new Coordinates(204, 380),
					4 => new Coordinates(180, 302),
					5 => new Coordinates(201, 253),
					6 => new Coordinates(208, 205),
					7 => new Coordinates(85, 159),
					8 => new Coordinates(77, 116),
					9 => new Coordinates(311, 153),
					10 => new Coordinates(340, 111),
					11 => new Coordinates(210, 130),
					12 => new Coordinates(208, 88),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.InnerHomePath => stepOnPath switch
				{
					1 => new Coordinates(157, 241),
					2 => new Coordinates(154, 225),
					3 => new Coordinates(155, 210),
					4 => new Coordinates(149, 166),
					5 => new Coordinates(180, 166),
					6 => new Coordinates(210, 166),
					7 => new Coordinates(240, 166),
					8 => new Coordinates(270, 166),
					9 => new Coordinates(165, 105),
					10 => new Coordinates(163, 90),
					11 => new Coordinates(165, 80),
					12 => new Coordinates(206, 52),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.PersonalPath => stepOnPath switch
				{
					1 => new Coordinates(384, 338),
					2 => new Coordinates(396, 369),
					3 => new Coordinates(356, 383),
					4 => new Coordinates(347, 340),
					5 => new Coordinates(385, 305),
					6 => new Coordinates(430, 330),
					7 => new Coordinates(424, 386),
					8 => new Coordinates(370, 417),
					9 => new Coordinates(307, 383),
					10 => new Coordinates(311, 316),
					11 => new Coordinates(366, 265),
					12 => new Coordinates(420, 260),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				RegionOnMap.OrganizationalPath => stepOnPath switch
				{
					1 => new Coordinates(70, 462),
					2 => new Coordinates(65, 444),
					3 => new Coordinates(63, 431),
					4 => new Coordinates(68, 420),
					5 => new Coordinates(78, 402),
					6 => new Coordinates(67, 388),
					7 => new Coordinates(64, 374),
					8 => new Coordinates(72, 353),
					9 => new Coordinates(77, 341),
					10 => new Coordinates(75, 328),
					11 => new Coordinates(79, 314),
					12 => new Coordinates(64, 259),
					_ => throw new ArgumentException("stepOnPath is not correct")
				},
				_ => throw new ArgumentException("branch is unknown")
			};

			return new Coordinates(coordinates.x - 14, coordinates.y - 29);
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
				RegionOnMap = newGame.Oracle.RegionOnMap,

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
				Created = DateTime.UtcNow,
				UserId = newGame.CreatorId
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
				Created = newGame.Created,
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


		public static LogsDTOModel GameLogsToLogsDTOModel(this IEnumerable<GameChatLog> gameLogs, int gameId, int userId)
		{
			var model = new LogsDTOModel
			{
				GameId = gameId,
				UserId = userId,
				Messages = new HashSet<string>()
			};

			foreach (var item in gameLogs)
			{
				model.Messages.Add(item.Message);
			} 
			return model;
		}
	}
}
