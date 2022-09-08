using System.IO;
using TamboliyaApi.GameLogic.Models;

namespace TamboliyaApi.GameLogic
{
    public class NewMoveService
    {
        private readonly Dodecahedron dodecahedron;
        private const string finishMessage = "Почни гру з початку, з новим питанням";
        private const string selectEmbodimentMessage = "Вибери один зі шляхів самореалізації або перейди в будь-яке інше місце на карті"; //TODO: Переход в любое место на карте не реализован. Сделать!

        public NewMoveService(Dodecahedron dodecahedron)
        {
            this.dodecahedron = dodecahedron;
        }

        public async Task<ActualPositionOnMap> MakeMoveAsync(ActualPositionOnMap actualPosition, NewGame game)
        {
            return actualPosition.RegionOnMap switch
            {
                RegionOnMap.OrganizationalPath => await NewPositionStepOnStep(actualPosition.PositionNumber,
                game, GamePathes.mapOrganizationalPath, RegionOnMap.OrganizationalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.NameYourIsland),
                
                RegionOnMap.PersonalPath => await NewPositionStepOnStep(actualPosition.PositionNumber,
                game, GamePathes.mapPersonalPath, RegionOnMap.PersonalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.WhoSees),
                
                RegionOnMap.InnerHomePath => await NewPositionStepOnStep(actualPosition.PositionNumber,
                game, GamePathes.mapInnerHomePath, RegionOnMap.InnerHomePath, RegionOnMap.LandOfClarity, 
                (int)LandOfClarity.WhatAmIDoingHere),

                RegionOnMap.MysticalPath => await NewPositionStepOnStep(actualPosition.PositionNumber, 
                game, GamePathes.mapMysticalPath, RegionOnMap.MysticalPath, RegionOnMap.LandOfClarity,
                (int)LandOfClarity.WhoTeaches),
                

                RegionOnMap.Delusion => await NewPositionStepOnStep(actualPosition.PositionNumber, game,
                GamePathes.mapDelusionPath, RegionOnMap.Delusion, RegionOnMap.MysticalPath, 
                (int)MysticalPath.FulfillmentOfDesires),

                RegionOnMap.Embodiment => await NewStepOnEmbodiment(actualPosition.PositionNumber, game),
                
                RegionOnMap.LandOfClarity => await NewStepOnLandOfClarity(actualPosition.PositionNumber, game),
                
                _ => throw new ArgumentException("Region on the map is not right")
            };
        }

        private async Task<ActualPositionOnMap> NewStepOnLandOfClarity(int positionNumber, NewGame game)
        {
            string rootFolder = Directory.GetCurrentDirectory()!;
            string path = Path.Combine(rootFolder, GamePathes.mapLandOfClarityPath)!;
            ActualPositionOnMap actualPositionOnMap = new()
            {
                RegionOnMap = RegionOnMap.LandOfClarity
            };

            if (game.ActualPositionsForSelect.Count() > 0)
            {
                for (int i = 0; i < game.ActualPositionsForSelect.Count(); i++)
                {
                    if (game.ActualPositionsForSelect[i].IsSelected)
                    {
                        actualPositionOnMap.PositionNumber = game.ActualPositionsForSelect[i].PositionNumber;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(game.ActualPositionsForSelect[i].PositionNumber + " —"));
                        break;
                    }
                }
                game.ActualPositionsForSelect = new();
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
                        game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    }
                    else
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhereForceComeIn;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                        game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    }
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhereForceComeIn)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhoYourPartner;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.DifferenceGoodAndEvil)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsRiverOfLife;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsRiverOfLife)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsConsciousness;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsConsciousness)
                {
                    var position = dodecahedron.ThrowBone();
                    if (position.Number % 2 == 0)
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.YourRefuge;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                        game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    }
                    else
                    {
                        actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameShadowOfDeath;
                        actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                            .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                        game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    }
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameShadowOfDeath)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.RestartGame;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.IsFinished = true;
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    game.PromptMessages.Enqueue(finishMessage);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.YourRefuge)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhoTeaches;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoTeaches)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    await AddActualPositionOnLandOfClarityToList(game, path);
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    game.PromptMessages.Enqueue(selectEmbodimentMessage);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourIsland)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBoat;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBoat)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    await AddActualPositionOnLandOfClarityToList(game, path);
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    game.PromptMessages.Enqueue(selectEmbodimentMessage);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoSees)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBridge;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBridge)
                {
                    actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    await AddActualPositionOnLandOfClarityToList(game, path);
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                    game.PromptMessages.Enqueue(selectEmbodimentMessage);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsSerenity)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.HermitMystic;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhereLoveComeFrom)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.PilgrimWanderer;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
                else if ((LandOfClarity)positionNumber == LandOfClarity.WhoResponsibleForTeachings)
                {
                    path = Path.Combine(rootFolder, GamePathes.mapEmbodimentPath)!;
                    actualPositionOnMap.PositionNumber = (int)Embodiment.TeacherFriend;
                    actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
                    actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                        .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                    game.PromptMessages.Enqueue(actualPositionOnMap.Description);
                }
            }

            
            return actualPositionOnMap;
        }

        private async Task<ActualPositionOnMap> NewStepOnEmbodiment(int positionNumber, NewGame game)
        {
            string rootFolder = Directory.GetCurrentDirectory()!;
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
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.PilgrimWanderer)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.Sunshine;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.TeacherFriend)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.RainbowBeam;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.Moonbeam ||
                (Embodiment)positionNumber == Embodiment.Sunshine ||
                (Embodiment)positionNumber == Embodiment.RainbowBeam)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.MyGift;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.MyGift)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.MyWay;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.MyWay)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.Wisdom;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.Wisdom)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.EngagementLimit;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.EngagementLimit)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.WhereGoWhoUnderstandEverything;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.WhereGoWhoUnderstandEverything)
            {
                actualPositionOnMap.PositionNumber = (int)Embodiment.TruthAndFalsehood;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if ((Embodiment)positionNumber == Embodiment.TruthAndFalsehood)
            {
                actualPositionOnMap.RegionOnMap = RegionOnMap.Delusion;
                actualPositionOnMap.PositionNumber = (int)Delusion.TruthAndFalsehood;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            return actualPositionOnMap!;
        }

        private async Task<ActualPositionOnMap> NewPositionStepOnStep(int positionNumber, NewGame game, 
            string pathToProphecy, RegionOnMap actualRegion, RegionOnMap nextStepRegion,
            int stepPositionNumberOnNextRegionMap = 1)
        {
            string rootFolder = Directory.GetCurrentDirectory()!;
            string path = Path.Combine(rootFolder, pathToProphecy)!;
            ActualPositionOnMap actualPositionOnMap = new()
            {
                RegionOnMap = actualRegion
            };

            if (positionNumber > 0 && positionNumber < 12)
            {
                actualPositionOnMap.PositionNumber = positionNumber + 1;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }
            else if (positionNumber == 12)
            {
                actualPositionOnMap.RegionOnMap = nextStepRegion;
                actualPositionOnMap.PositionNumber = (int)stepPositionNumberOnNextRegionMap;
                actualPositionOnMap.Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith(actualPositionOnMap.PositionNumber + " —"));
                game.PromptMessages.Enqueue(actualPositionOnMap.Description);
            }

            return actualPositionOnMap!;
        }



        private Task<ActualPositionOnMap> NewStepOnInnerHome(int positionNumber)
        {
            throw new NotImplementedException();
        }

        

        private Task<ActualPositionOnMap> NewStepOnPersonal(int positionNumber)
        {
            throw new NotImplementedException();
        }

        private Task<ActualPositionOnMap> NewStepOnOrganization(int positionNumber)
        {
            throw new NotImplementedException();
        }


        private async Task AddActualPositionOnLandOfClarityToList(NewGame game, string path)
        {
            game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)LandOfClarity.WhoResponsibleForTeachings,
                RegionOnMap = RegionOnMap.LandOfClarity,
                Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith((int)LandOfClarity.WhoResponsibleForTeachings + " —"))
            });

            game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)LandOfClarity.WhereLoveComeFrom,
                RegionOnMap = RegionOnMap.LandOfClarity,
                Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith((int)LandOfClarity.WhereLoveComeFrom + " —"))
            });

            game.ActualPositionsForSelect.Add(new()
            {
                PositionNumber = (int)LandOfClarity.WhatIsSerenity,
                RegionOnMap = RegionOnMap.LandOfClarity,
                Description = (await File.ReadAllLinesAsync(path))
                    .First(line => line.StartsWith((int)LandOfClarity.WhatIsSerenity + " —"))
            });
        }
    }
}
