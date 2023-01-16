using AutoMapper;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InitialGameData, OracleDTO>()
                .ForMember(u => u.PathToImage, opt => opt.MapFrom(x => GameExtensions.GetPath(x.RegionOnMap, x.StepOnPath)))
                .ForMember(u => u.Coordinates, opt => opt.MapFrom(x => GameExtensions.GetCoordinates(x.RegionOnMap, x.StepOnPath)));

            CreateMap<ActualPositionOnMap, ActualPositionOnTheMap>();
            CreateMap<ActualPositionOnTheMap, ActualPositionOnMap>();
            CreateMap<GameChatLog, GameChatLogDTO>();
            CreateMap<NewGame, Game>()
                .ForMember(u => u.ActualPosition, opt => opt.MapFrom(x => x.ActualPosition))
                .ForMember(u => u.CreatorGuid, opt => opt.MapFrom(x => x.CreatorId))
                .ForMember(u => u.IsFinished, opt => opt.MapFrom(x => false))

                .ForMember(u => u.InitialGameData, opt => opt.MapFrom(x => new InitialGameData()))
                .ForPath(u => u.InitialGameData.Question, opt => opt.MapFrom(x => x.Oracle.Question))
                .ForPath(u => u.InitialGameData.Motive, opt => opt.MapFrom(x => x.Oracle.Motive))
                .ForPath(u => u.InitialGameData.QualityOfExperience, opt => opt.MapFrom(x => x.Oracle.QualityOfExperience))
                .ForPath(u => u.InitialGameData.EnvironmentAndCircumstances, opt => opt.MapFrom(x => x.Oracle.EnvironmentAndCircumstances))
                .ForPath(u => u.InitialGameData.ChainLinks, opt => opt.MapFrom(x => x.Oracle.ChainLinks))
                .ForPath(u => u.InitialGameData.ExitPath, opt => opt.MapFrom(x => x.Oracle.ExitPath))
                .ForPath(u => u.InitialGameData.StepOnPath, opt => opt.MapFrom(x => x.Oracle.StepOnPath))
                .ForPath(u => u.InitialGameData.RegionOnMap, opt => opt.MapFrom(x => x.Oracle.RegionOnMap));

            CreateMap<Game, GameDTO>()
                .ForMember(u => u.GameId, opt => opt.MapFrom(x => x.Id))
                .ForMember(u => u.Created, opt => opt.MapFrom(x => x.DateBeginning))
                .ForMember(u => u.ActualPosition, opt => opt.MapFrom(x => x.ActualPosition))
                .ForMember(u => u.Oracle, opt => opt.MapFrom(x => x.InitialGameData))
                .ForMember(u => u.PathToImage, opt => opt.MapFrom(x => GameExtensions.GetPath(x.ActualPosition.RegionOnMap, x.ActualPosition.PositionNumber)))
                .ForMember(u => u.Coordinates, opt => opt.MapFrom(x => GameExtensions.GetCoordinates(x.ActualPosition.RegionOnMap, x.ActualPosition.PositionNumber)));

        }
    }
}
