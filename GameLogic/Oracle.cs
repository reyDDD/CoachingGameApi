using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using System.Text.RegularExpressions;

namespace TamboliyaApi.GameLogic
{
    public class Oracle
    {
        private const string motivePath = "Cards/Motive.txt";
        private const string qualityOfExperiencePath = "Cards/QualityOfExperience.txt";
        private const string environmentAndCircumstancesPath = "Cards/EnvironmentAndCircumstances.txt";
        private const string chainLinksPath = "Cards/ChainLinks.txt";
        private const string exitPath = "Cards/ExitPath.txt";

        private const string mapEmbodimentPath = "Cards/Map_Embodiment.txt";
        private const string mapDelusionPath = "Cards/Map_Delusion.txt";
        private const string mapOrganizationalPath = "Cards/Map_OrganizationalPath.txt";
        private const string mapInnerHomePath = "Cards/Map_PathInnerHome.txt";
        private const string mapPersonalPath = "Cards/Map_PersonalPath.txt";
        private const string mapMysticalPath = "Cards/Map_MysticalPath.txt";

        private readonly Dodecahedron dodecahedron;

        public string Question { get; private set; } = null!;
        public string Motive { get; private set; } = null!;
        public string QualityOfExperience { get; private set; } = null!;
        public string EnvironmentAndCircumstances { get; private set; } = null!;
        public string ChainLinks { get; private set; } = null!;
        public string ExitPath { get; private set; }
        public int StepOnPath { get; private set; }


        public Oracle(Dodecahedron dodecahedron)
        {
            this.dodecahedron = dodecahedron;
        }

        public async Task Start(string question)
        {
            UserQuestion(question);

            Task[] tasks = new Task[5];

            Task<(string Prophecy, Color Color)> Step1 = Task.Run(async () => await StepWithColor(motivePath));
            Task<(string Prophecy, int Number)> Step2 = Task.Run(async () => await StepWithNumber(qualityOfExperiencePath));
            Task<(string Prophecy, int Number)> Step3 = Task.Run(async () => await StepWithNumber(environmentAndCircumstancesPath, true));
            Task<(string Prophecy, int Number)> Step4 = Task.Run(async () => await StepWithNumber(chainLinksPath));
            Task<(string Prophecy, Color Color)> Step5 = Task.Run(async () => await StepWithColor(exitPath));

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
        }

        private async Task<int> GetStepOnPath(Color color)
        {
            int position = color switch
            {
                Color.Red => (await StepWithNumber(mapOrganizationalPath)).Number,
                Color.Blue => (await StepWithNumber(mapMysticalPath)).Number,
                Color.Green => (await StepWithNumber(mapInnerHomePath)).Number,
                Color.Yellow => (await StepWithNumber(mapPersonalPath)).Number,
                _  => throw new ArgumentException("Color from dodecahedron not get")
            };

            return await Task.FromResult(position);
        }

        public void UserQuestion(string question)
        {
            Question = question;
        }

        private async Task<(string, Color)> StepWithColor(string pathToValues)
        {
            var position = dodecahedron.ThrowBone();

            var rootFolder = Directory.GetCurrentDirectory();
            var path = Path.Combine(rootFolder, pathToValues);
            var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
            string prophecy = prophecies.Where(m => m.StartsWith(position.Color.ToString())).First();
            return (prophecy, position.Color);
        }


        private async Task<(string Prophecy, int Number)> StepWithNumber(string pathToValues, bool additionalThrowBone = false)
        {
            var position = dodecahedron.ThrowBone();
            var rootFolder = Directory.GetCurrentDirectory();
            var pathToValue = pathToValues;

            if (additionalThrowBone && position.Number == 7 && position.Number == 8)
            {
                if (position.Number == 7)
                {
                    pathToValue = mapEmbodimentPath;
                }
                else if (position.Number == 8)
                {
                    pathToValue = mapDelusionPath;
                }
            }

            var path = Path.Combine(rootFolder, pathToValue);
            var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
            string prophecy = prophecies.Where(m => m.StartsWith(position.Number.ToString() + " —")).First();


            var pattern = new Regex(@"^\d{1,2}\s");
            int positionNumber = Convert.ToInt32(pattern.Match(prophecy).Value);
            return (prophecy, positionNumber);
        }
    }
}
