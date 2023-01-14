using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using System.Text.RegularExpressions;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.GameLogic
{
	public class Oracle
	{
		private readonly Dodecahedron dodecahedron;

		
		public string Question { get; private set; } = null!;
		public string Motive { get; private set; } = null!;
		public string QualityOfExperience { get; private set; } = null!;
		public string EnvironmentAndCircumstances { get; private set; } = null!;
		public string ChainLinks { get; private set; } = null!;
		public string ExitPath { get; private set; } = null!;
		public int StepOnPath { get; private set; }
		public RegionOnMap RegionOnMap { get; private set; }

        public int GameId { get; set; }

        public Oracle(Dodecahedron dodecahedron)
		{
			this.dodecahedron = dodecahedron;
		}

        public void UserQuestion(string question)
        {
            Question = question;
        }

        public async Task Start(string question)
		{
			UserQuestion(question);

			Task[] tasks = new Task[5];

			Task<(string Prophecy, Color Color)> Step1 = Task.Run(async () => await StepWithColor(GamePathes.motivePath));
			Task<(string Prophecy, int Number)> Step2 = Task.Run(async () => await StepWithNumber(GamePathes.qualityOfExperiencePath));
			Task<(string Prophecy, int Number)> Step3 = Task.Run(async () => await StepWithNumber(GamePathes.environmentAndCircumstancesPath, true));
			Task<(string Prophecy, int Number)> Step4 = Task.Run(async () => await StepWithNumber(GamePathes.chainLinksPath));
			Task<(string Prophecy, Color Color)> Step5 = Task.Run(async () => await StepWithColor(GamePathes.exitPath));

			tasks[0] = Step1;
			tasks[1] = Step2;
			tasks[2] = Step3;
			tasks[3] = Step4;
			tasks[4] = Step5;

			Task.WaitAll(tasks);

			Motive = Step1!.Result.Prophecy;
			QualityOfExperience = Step2!.Result.Prophecy;
			EnvironmentAndCircumstances = Step3!.Result.Prophecy;
			ChainLinks = Step4!.Result.Prophecy;
			ExitPath = Step5!.Result.Prophecy;
			StepOnPath = await GetStepOnPath(Step5!.Result.Color);
			RegionOnMap = GetRegion(Step5!.Result.Color);
		}

		private async Task<int> GetStepOnPath(Color color)
		{
			int position = color switch
			{
				Color.Red => (await StepWithNumber(GamePathes.mapOrganizationalPath)).Number,
				Color.Blue => (await StepWithNumber(GamePathes.mapMysticalPath)).Number,
				Color.Green => (await StepWithNumber(GamePathes.mapInnerHomePath)).Number,
				Color.Yellow => (await StepWithNumber(GamePathes.mapPersonalPath)).Number,
				_ => throw new ArgumentException("Color from dodecahedron not get")
			};

			return await Task.FromResult(position);
		}

		

		private async Task<(string, Color)> StepWithColor(string pathToValues)
		{
			var position = dodecahedron.ThrowBone();

			var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
			var path = Path.Combine(rootFolder, pathToValues);
			var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
			string prophecy = prophecies.Where(m => m.StartsWith(position.Color.ToString())).First();

			int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

			return (textMessage, position.Color);
		}


		private async Task<(string Prophecy, int Number)> StepWithNumber(string pathToValues, bool additionalThrowBone = false)
		{
			var position = dodecahedron.ThrowBone();
			var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);
			var pathToValue = pathToValues;

			if (additionalThrowBone && position.Number == 7 && position.Number == 8)
			{
				if (position.Number == 7)
				{
					pathToValue = GamePathes.mapEmbodimentPath;
				}
				else if (position.Number == 8)
				{
					pathToValue = GamePathes.mapDelusionPath;
				}
			}

			var path = Path.Combine(rootFolder, pathToValue);
			var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
			string prophecy = prophecies.Where(m => m.StartsWith(position.Number.ToString() + " —")).First();


			var pattern = new Regex(@"^\d{1,2}\s");
			int positionNumber = Convert.ToInt32(pattern.Match(prophecy).Value);

			int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

			return (textMessage, positionNumber);
		}

		public static async Task<(string Prophecy, int Number)> StepWithNumber(string pathToValue,
			Dodecahedron dodecahedron)
		{
			var position = dodecahedron.ThrowBone();
			var rootFolder = Path.Combine(Directory.GetCurrentDirectory()!, GamePathes.Prefix);

			var path = Path.Combine(rootFolder, pathToValue);
			var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
			string prophecy = prophecies.Where(m => m.StartsWith(position.Number.ToString() + " —")).First();


			var pattern = new Regex(@"^\d{1,2}\s");
			int positionNumber = Convert.ToInt32(pattern.Match(prophecy).Value);

			int separatorIndex = (prophecy.IndexOf('—')) != -1 ? prophecy.IndexOf('—') + 2 : 0;
			var textMessage = prophecy.AsSpan()[separatorIndex..(prophecy.Length - 1)].ToString();

			return (textMessage, positionNumber);
		}

		private RegionOnMap GetRegion(Color color)
		{
			return color switch
			{
				Color.Red => RegionOnMap.OrganizationalPath,
				Color.Green => RegionOnMap.InnerHomePath,
				Color.Yellow => RegionOnMap.PersonalPath,
				Color.Blue => RegionOnMap.MysticalPath,
				_ => throw new ArgumentException("Argument is not right", color.ToString())
			};
		}
	}
}
