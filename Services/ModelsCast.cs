using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaApi.GameLogic.ModelDTOs;

namespace TamboliyaApi.Services
{
    public static class ModelsCast
    {

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
                RegionOnMap = oracle.RegionOnMap
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
    }
}
