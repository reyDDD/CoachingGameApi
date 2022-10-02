using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.Services;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.GameLogic
{
	public class NewMoveService
	{
		private readonly Dodecahedron dodecahedron;
		private readonly PositionsOnMapService positionsOnMapService;
		private const string finishMessage = "Почни гру з початку, з новим питанням";
		private const string selectEmbodimentMessage = "Вибери один зі шляхів самореалізації або перейди в будь-яке інше місце на карті"; //TODO: Переход в любое место на карте не реализован. Сделать!

		private readonly LogService logService;

		public NewMoveService(Dodecahedron dodecahedron, LogService logService, PositionsOnMapService positionsOnMapService)
		{
			this.dodecahedron = dodecahedron;
			this.logService = logService;
			this.positionsOnMapService = positionsOnMapService;
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

				RegionOnMap.Embodiment => NewStepOnEmbodiment(game.ActualPosition.PositionNumber),

				RegionOnMap.LandOfClarity => NewStepOnLandOfClarity(game.ActualPosition.PositionNumber, newGame, game),

				_ => throw new ArgumentException("Region on the map is not right")
			};
		}

		private ActualPositionOnMap NewStepOnLandOfClarity(int positionNumber,
			NewGame newGame, Game game)
		{
			RegionOnMap regionOnMapForNewPosition = RegionOnMap.LandOfClarity;
			ActualPositionOnMap actualPositionOnMap = new()
			{
				RegionOnMap = regionOnMapForNewPosition
			};

			if (newGame.ActualPositionsForSelect.Count() > 0)
			{
				for (int i = 0; i < newGame.ActualPositionsForSelect.Count(); i++)
				{
					if (newGame.ActualPositionsForSelect[i].IsSelected!.Value)
					{
						actualPositionOnMap.PositionNumber = newGame.ActualPositionsForSelect[i].PositionNumber;

						actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(actualPositionOnMap.RegionOnMap, actualPositionOnMap.PositionNumber);
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
					}
					else
					{
						actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhereForceComeIn;
					}
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhereForceComeIn)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.DifferenceGoodAndEvil;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.DifferenceGoodAndEvil)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsRiverOfLife;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsRiverOfLife)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatIsConsciousness;
				}
				if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsConsciousness)
				{
					var position = dodecahedron.ThrowBone();
					if (position.Number % 2 == 0)
					{
						actualPositionOnMap.PositionNumber = (int)LandOfClarity.YourRefuge;
					}
					else
					{
						actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameShadowOfDeath;
					}
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.NameShadowOfDeath)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.RestartGame;
					newGame.IsFinished = true;
					logService.AddRecord(game, finishMessage);
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.YourRefuge)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhoTeaches;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourIsland)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBoat;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhoSees)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.NameYourBridge;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhoTeaches)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
					AddActualPositionOnLandOfClarityToList(newGame);
					logService.AddRecord(game, selectEmbodimentMessage);
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBoat)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
					AddActualPositionOnLandOfClarityToList(newGame);
					logService.AddRecord(game, selectEmbodimentMessage);
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.NameYourBridge)
				{
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
					AddActualPositionOnLandOfClarityToList(newGame);
					logService.AddRecord(game, selectEmbodimentMessage);
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhatIsSerenity)
				{
					regionOnMapForNewPosition = RegionOnMap.Embodiment;
					actualPositionOnMap.PositionNumber = (int)Embodiment.HermitMystic;
					actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhereLoveComeFrom)
				{
					regionOnMapForNewPosition = RegionOnMap.Embodiment;
					actualPositionOnMap.PositionNumber = (int)Embodiment.PilgrimWanderer;
					actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhoResponsibleForTeachings)
				{
					regionOnMapForNewPosition = RegionOnMap.Embodiment;
					actualPositionOnMap.PositionNumber = (int)Embodiment.TeacherFriend;
					actualPositionOnMap.RegionOnMap = RegionOnMap.Embodiment;
				}
				else if ((LandOfClarity)positionNumber == LandOfClarity.WhatAmIDoingHere)
				{
					AddActualPositionOnLandOfClarityToList(newGame);
					actualPositionOnMap.PositionNumber = (int)LandOfClarity.WhatAmIDoingHere;
					logService.AddRecord(game, selectEmbodimentMessage);
				}

				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			return actualPositionOnMap;
		}

		private ActualPositionOnMap NewStepOnEmbodiment(int positionNumber)
		{
			RegionOnMap regionOnMapForNewPosition = RegionOnMap.Embodiment;
			ActualPositionOnMap actualPositionOnMap = new()
			{
				RegionOnMap = regionOnMapForNewPosition
			};

			if ((Embodiment)positionNumber == Embodiment.HermitMystic)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.Moonbeam;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.PilgrimWanderer)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.Sunshine;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.TeacherFriend)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.RainbowBeam;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.Moonbeam ||
				(Embodiment)positionNumber == Embodiment.Sunshine ||
				(Embodiment)positionNumber == Embodiment.RainbowBeam)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.MyGift;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.MyGift)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.MyWay;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.MyWay)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.Wisdom;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.Wisdom)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.EngagementLimit;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.EngagementLimit)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.WhereGoWhoUnderstandEverything;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.WhereGoWhoUnderstandEverything)
			{
				actualPositionOnMap.PositionNumber = (int)Embodiment.TruthAndFalsehood;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if ((Embodiment)positionNumber == Embodiment.TruthAndFalsehood)
			{
				regionOnMapForNewPosition = RegionOnMap.Delusion;
				actualPositionOnMap.PositionNumber = (int)Delusion.TruthAndFalsehood;
				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			return actualPositionOnMap!;
		}

		private async Task<ActualPositionOnMap> NewPositionStepOnStep(int positionNumber, Game game,
			string pathToProphecy, RegionOnMap actualRegion, RegionOnMap nextStepRegion,
			int stepPositionNumberOnNextRegionMap = 1)
		{
			RegionOnMap regionOnMapForNewPosition = actualRegion;

			ActualPositionOnMap actualPositionOnMap = new()
			{
				RegionOnMap = regionOnMapForNewPosition
			};

			if (positionNumber > 0 && positionNumber < 12 && nextStepRegion != RegionOnMap.Embodiment)
			{
				actualPositionOnMap.PositionNumber = positionNumber + 1;

				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}
			else if (positionNumber == 12 && nextStepRegion != RegionOnMap.Embodiment)
			{
				regionOnMapForNewPosition = game.ActualPosition.RegionOnMap switch
				{
					RegionOnMap.OrganizationalPath => RegionOnMap.LandOfClarity,
					RegionOnMap.PersonalPath => RegionOnMap.LandOfClarity,
					RegionOnMap.MysticalPath => RegionOnMap.LandOfClarity,
					RegionOnMap.Delusion => RegionOnMap.MysticalPath,
					RegionOnMap.InnerHomePath => RegionOnMap.LandOfClarity,
					RegionOnMap.Embodiment => RegionOnMap.Delusion,
					_ => throw new ArgumentException("RegionOnMap isn't correct")
				};


				actualPositionOnMap.RegionOnMap = nextStepRegion;
				actualPositionOnMap.PositionNumber = (int)stepPositionNumberOnNextRegionMap;

				actualPositionOnMap.Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, actualPositionOnMap.PositionNumber);
			}

			return actualPositionOnMap!;
		}


		private void AddActualPositionOnLandOfClarityToList(NewGame game)
		{
			RegionOnMap regionOnMapForNewPosition = RegionOnMap.LandOfClarity;

			game.ActualPositionsForSelect.Add(new()
			{
				PositionNumber = (int)LandOfClarity.WhatIsSerenity,
				RegionOnMap = RegionOnMap.LandOfClarity,
				Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, (int)LandOfClarity.WhatIsSerenity)
			});

			game.ActualPositionsForSelect.Add(new()
			{
				PositionNumber = (int)LandOfClarity.WhereLoveComeFrom,
				RegionOnMap = RegionOnMap.LandOfClarity,
				Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, (int)LandOfClarity.WhereLoveComeFrom)
			});

			game.ActualPositionsForSelect.Add(new()
			{
				PositionNumber = (int)LandOfClarity.WhoResponsibleForTeachings,
				RegionOnMap = RegionOnMap.LandOfClarity,
				Description = positionsOnMapService.GetPositionDescription(regionOnMapForNewPosition, (int)LandOfClarity.WhoResponsibleForTeachings)
			});
		}
	}
}
