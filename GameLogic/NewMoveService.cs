using System.IO;
using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic
{
    public class NewMoveService
    {
        private readonly Dodecahedron dodecahedron;
        private const string finishMessage = "Почни гру з початку, з новим питанням";
        private const string selectEmbodimentMessage = "Вибери один зі шляхів самореалізації або перейди в будь-яке інше місце на карті"; //TODO: Переход в любое место на карте не реализован. Сделать!

        private readonly LogService logService;

        public NewMoveService(Dodecahedron dodecahedron, LogService logService)
        {
            this.dodecahedron = dodecahedron;
            this.logService = logService;
        }

        public async Task<ActualPositionOnMap> MakeMoveAsync(NewGame newGame, Game game)
        {
            ActualPositionOnMap actualPosition = game.ActualPosition.ActualPositionOnMapToDTO();

            return actualPosition.RegionOnMap switch
            {
                RegionOnMap.OrganizationalPath => await NewPositionStepOnStep(game.ActualPosition.PositionNumber,
                game, GamePathes.mapOrganizationalPath, RegionOnMap.OrganizationalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.NameYourIsland),

                RegionOnMap.PersonalPath => await NewPositionStepOnStep(game.ActualPosition.PositionNumber,
                game, GamePathes.mapPersonalPath, RegionOnMap.PersonalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.WhoSees),

                RegionOnMap.InnerHomePath => await NewPositionStepOnStep(game.ActualPosition.PositionNumber,
                game, GamePathes.mapInnerHomePath, RegionOnMap.InnerHomePath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.WhatAmIDoingHere),

                RegionOnMap.MysticalPath => await NewPositionStepOnStep(game.ActualPosition.PositionNumber,
                game, GamePathes.mapMysticalPath, RegionOnMap.MysticalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.WhoTeaches),


                RegionOnMap.Delusion => await NewPositionStepOnStep(game.ActualPosition.PositionNumber,
                game, GamePathes.mapDelusionPath, RegionOnMap.Delusion, RegionOnMap.MysticalPath,
                (int)MysticalPath.FulfillmentOfDesires),

                RegionOnMap.Embodiment => await NewStepOnEmbodiment(game.ActualPosition.PositionNumber),

                RegionOnMap.LandOfClarity => await NewStepOnLandOfClarity(game.ActualPosition.PositionNumber, newGame, game),

                _ => throw new ArgumentException("Region on the map is not right")
            };
        }

        private async Task<ActualPositionOnMap> NewStepOnLandOfClarity(int positionNumber,
            NewGame newGame, Game game)
        {
            string rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
            string path = Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!;
            ActualPositionOnMap actualPositionOnMap = new()
            {
                RegionOnMap = RegionOnMap.LandOfClarity
            };

            if (newGame.ActualPositionsForSelect.Count() > 0)
            {
                for (int i = 0; i < newGame.ActualPositionsForSelect.Count(); i++)
                {
                    if (newGame.ActualPositionsForSelect[i].IsSelected!.Value)
                    {
                        actualPositionOnMap.PositionNumber = newGame.ActualPositionsForSelect[i].PositionNumber;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(newGame.ActualPositionsForSelect[i].PositionNumber + " —"));
                        break;
                    }
                }
                newGame.ActualPositionsForSelect = new();
            }
            else
            {
                if ((LandOfClarity)positionNumber == LandOfClarity.Gatekeeper ||
                    (LandOfClarity)positionNumber == LandOfClarity.WhoYourPartner)
                {
                    var position = dodecahedron.ThrowBone();
                    if (position.Number % 2 == 0)
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.DifferenceGoodAndEvil;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    }
                    else
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhereForceComeIn;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    }
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhereForceComeIn)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhoYourPartner;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.DifferenceGoodAndEvil)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsRiverOfLife;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsRiverOfLife)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsConsciousness;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsConsciousness)
                {
                    var position = dodecahedron.ThrowBone();
                    if (position.Number % 2 == 0)
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.YourRefuge;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    }
                    else
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameShadowOfDeath;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    }
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameShadowOfDeath)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.RestartGame;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    newGame.IsFinished = true;
                    logService.AddRecord(game, finishMessage);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.YourRefuge)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhoTeaches;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoTeaches)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourIsland)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBoat;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBoat)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoSees)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBridge;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBridge)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsSerenity)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.HermitMystic;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhereLoveComeFrom)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.PilgrimWanderer;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoResponsibleForTeachings)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.TeacherFriend;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhatAmIDoingHere)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath);
                    await AddActualPositionOnLandOfClarityToList(newGame, path);

                    actualPositionOnMap.PositionNumber = game.ActualPosition.PositionNumber;
                    actualPositionOnMap.RegionOnMap = game.ActualPosition.RegionOnMap;
                    actualPositionOnMap.Description = game.ActualPosition.Description;

                    logService.AddRecord(game, selectEmbodimentMessage);
                }
            }


            return actualPositionOnMap;
        }

        private async Task<ActualPositionOnMap> NewStepOnEmbodiment(int positionNumber)
        {
            string rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
            string path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
            ActualPositionOnMap actualPositionOnMap = new()
            {
                RegionOnMap = RegionOnMap.Embodiment
            };

            if ((Embodiment)positionNumber == Embodiment.HermitMystic)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.Moonbeam;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.PilgrimWanderer)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.Sunshine;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.TeacherFriend)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.RainbowBeam;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.Moonbeam ||
                (Embodiment)positionNumber == Embodiment.Sunshine ||
                (Embodiment)positionNumber == Embodiment.RainbowBeam)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.MyGift;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.MyGift)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.MyWay;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.MyWay)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.Wisdom;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.Wisdom)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.EngagementLimit;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.EngagementLimit)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.WhereGoWhoUnderstandEverything;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.WhereGoWhoUnderstandEverything)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.TruthAndFalsehood;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            else if ((Embodiment)positionNumber == Embodiment.TruthAndFalsehood)
            {
                actualPositionOnMap.RegionOnMap = RegionOnMap.Delusion;
                actualPositionOnMap.PositionNumber = (int)Delusion.TruthAndFalsehood;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
            }
            return actualPositionOnMap!;
        }

        private async Task<ActualPositionOnMap> NewPositionStepOnStep(int positionNumber, Game game,
            string pathToProphecy, RegionOnMap actualRegion, RegionOnMap nextStepRegion,
            int stepPositionNumberOnNextRegionMap = 1)
        {
            string rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
            string path = Path.Combine(rootFolder, pathToProphecy)!;
            ActualPositionOnMap actualPositionOnMap = new()
            {
                RegionOnMap = actualRegion
            };

            if (positionNumber > 0 && positionNumber < 12 && nextStepRegion != RegionOnMap.Embodiment)
            {
                actualPositionOnMap.PositionNumber = positionNumber + 1;

                string prophecy = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
				int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
				var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

				actualPositionOnMap.Description = textMessage;
            }
            else if (positionNumber == 12 && nextStepRegion != RegionOnMap.Embodiment)
            {
                path = game.ActualPosition.RegionOnMap switch
                {
                    RegionOnMap.OrganizationalPath => Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!,
                    RegionOnMap.PersonalPath => Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!,
                    RegionOnMap.MysticalPath => Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!,
                    RegionOnMap.Delusion => Path.Combine(rootFolder, GamePathes.mapMysticalPath)!,
                    RegionOnMap.InnerHomePath => Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!,
                    RegionOnMap.Embodiment => Path.Combine(rootFolder, GamePathes.mapDelusionPath)!,
                    _ => throw new ArgumentException("RegionOnMap isn't correct")
                };


                actualPositionOnMap.RegionOnMap = nextStepRegion;
                actualPositionOnMap.PositionNumber = (int)stepPositionNumberOnNextRegionMap;

                string prophecy = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
				int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
				var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

				actualPositionOnMap.Description = textMessage;
            }

            return actualPositionOnMap!;
        }


        private async Task AddActualPositionOnLandOfClarityToList(NewGame game, string path)
        {

			string prophecy = (await File.ReadAllLinesAsync(path))
					.First(line => line.StartsWith((int)Embodiment.TeacherFriend + " —"));
			int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();
			game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)Embodiment.TeacherFriend,
                RegionOnMap = RegionOnMap.Embodiment,
                Description = textMessage
			});


			prophecy = (await File.ReadAllLinesAsync(path))
					.First(line => line.StartsWith((int)Embodiment.PilgrimWanderer + " —"));
			separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();
			game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)Embodiment.PilgrimWanderer,
                RegionOnMap = RegionOnMap.Embodiment,
                Description = textMessage
			});


			prophecy = (await File.ReadAllLinesAsync(path))
					.First(line => line.StartsWith((int)Embodiment.HermitMystic + " —"));
			separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();
			game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)Embodiment.HermitMystic,
                RegionOnMap = RegionOnMap.Embodiment,
                Description = textMessage
			});
        }
    }
}
