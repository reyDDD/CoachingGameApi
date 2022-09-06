using TamboliyaApi.Data;
using TamboliyaApi.GameLogic.Models;
using System.Text.RegularExpressions;

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


        public Oracle(Dodecahedron dodecahedron)
        {
            this.dodecahedron = dodecahedron;
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
        }

        private async Task<int> GetStepOnPath(Color color)
        {
            int position = color switch
            {
                Color.Red => (await StepWithNumber(GamePathes.mapOrganizationalPath)).Number,
                Color.Blue => (await StepWithNumber(GamePathes.mapMysticalPath)).Number,
                Color.Green => (await StepWithNumber(GamePathes.mapInnerHomePath)).Number,
                Color.Yellow => (await StepWithNumber(GamePathes.mapPersonalPath)).Number,
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
            return (prophecy, positionNumber);
        }

        public static async Task<(string Prophecy, int Number)> StepWithNumber(string pathToValue,
            Dodecahedron dodecahedron)
        {
            var position = dodecahedron.ThrowBone();
            var rootFolder = Directory.GetCurrentDirectory();

            var path = Path.Combine(rootFolder, pathToValue);
            var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
            string prophecy = prophecies.Where(m => m.StartsWith(position.Number.ToString() + " —")).First();


            var pattern = new Regex(@"^\d{1,2}\s");
            int positionNumber = Convert.ToInt32(pattern.Match(prophecy).Value);
            return (prophecy, positionNumber);
        }
    }
}
