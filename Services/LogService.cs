using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;

namespace TamboliyaApi.Services
{
    public class LogService
    {
        private readonly UnitOfWork unitOfWork;

        public LogService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async void AddOracle(Game game, NewGame gameService)
        {
            var logQuestion = new GameLog
            {
                Message = "Запит гравця - " + game.InitialGameData.Question,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logQuestion);

            var logMotive = new GameLog
            {
                Message = "Мотив - " + game.InitialGameData.Motive,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logMotive);

            var logQualityOfExperience = new GameLog
            {
                Message = "Якість досвіду - " + game.InitialGameData.QualityOfExperience,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logQualityOfExperience);

            var logEnvironmentAndCircumstances = new GameLog
            {
                Message = "Оточення і обставини - " + game.InitialGameData.EnvironmentAndCircumstances,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logEnvironmentAndCircumstances);

            var logChainLinks = new GameLog
            {
                Message = "Ланки ланцюга - " + game.InitialGameData.ChainLinks,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logChainLinks);

            var logExitPath = new GameLog
            {
                Message = "Шлях входу - " + game.InitialGameData.ExitPath,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logExitPath);

            string path = gameService.GetPathToCards(game.InitialGameData.RegionOnMap);
            string positionDescription = await gameService.GetProphecyDescriptionAsync(path, game.InitialGameData.StepOnPath);

			var logStepOnPath = new GameLog
            {
                Message = "Позиція на шляху - " + positionDescription,
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logStepOnPath);

        }

        public void AddRecord(Game game)
        {
            var logMessage = new GameLog
            {
                Message = $"Позиція на шляху {game.ActualPosition.RegionOnMap} -  {game.ActualPosition.Description}",
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logMessage);
        }

        public void AddRecord(Game game, string message)
        {
            var logMessage = new GameLog
            {
                Message = $"Подія на шляху {game.ActualPosition.RegionOnMap} -  {message}",
                UserId = game.UserId,
                Game = game
            };
            unitOfWork.GameLog.Insert(logMessage);
        }

    }
}
