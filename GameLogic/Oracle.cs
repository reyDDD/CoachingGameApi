using TamboliyaApi.GameLogic.Models;

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

        public void Start(string question)
        {
            UserQuestion(question);

            Task[] tasks = new Task[5];

            Task<string> Step1 = Task.Run(async () => await StepWithColor(motivePath));
            Task<string> Step2 = Task.Run(async () => await StepWithNumber(qualityOfExperiencePath));
            Task<string> Step3 = Task.Run(async () => await StepWithNumber(environmentAndCircumstancesPath, true));
            Task<string> Step4 = Task.Run(async () => await StepWithNumber(chainLinksPath));
            Task<string> Step5 = Task.Run(async () => await StepWithColor(exitPath));

            tasks[0] = Step1;
            tasks[1] = Step2;
            tasks[2] = Step3;
            tasks[3] = Step4;
            tasks[4] = Step5;

            Task.WaitAll(tasks);

            Motive = Step1!.Result;
            QualityOfExperience = Step2!.Result;
            EnvironmentAndCircumstances = Step3!.Result;
            ChainLinks = Step4!.Result;
            ExitPath = Step5!.Result;
        }


        public void UserQuestion(string question)
        {
            Question = question;
        }

        private async Task<string> StepWithColor(string pathToValues)
        {
            var position = dodecahedron.ThrowBone();

            var rootFolder = Directory.GetCurrentDirectory();
            var path = Path.Combine(rootFolder, pathToValues);
            var prophecies = (await File.ReadAllLinesAsync(path)).ToList();
            return prophecies.Where(m => m.StartsWith(position.Color.ToString())).First();
        }


        private async Task<string> StepWithNumber(string pathToValues, bool additionalThrowBone = false)
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
            return prophecies.Where(m => m.StartsWith(position.Number.ToString() + " —")).First();
        }
    }
}
