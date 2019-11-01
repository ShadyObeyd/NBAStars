namespace NBAStars.App
{
    using System;
    using System.IO;
    using System.Linq;

    using CsvHelper;
    using Newtonsoft.Json;

    public class StartUp
    {
        public static void Main()
        {
            string notEnoughArgumentsMessage = "Input must contain 4 values: input path, maximum years, minimum rating and output path!";

            var input = Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (input.Length < 4)
            {
                Console.WriteLine(notEnoughArgumentsMessage);
                return;
            }

            var inputPath = input[0];
            var maxYears = int.Parse(input[1]);
            var minRating = double.Parse(input[2]);
            var outputPath = input[3];

            ValidateInput(inputPath, maxYears, minRating, outputPath);

            try
            {
                // Json file must be encoded in UTF-8 or deseriazing will not work!!!
                var deserializedPlayers = JsonConvert.DeserializeObject<Player[]>(File.ReadAllText(inputPath));
                var filteredPlayers = FilterPlayers(deserializedPlayers, minRating, maxYears);

                GenerateCsvFile(filteredPlayers, outputPath);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void GenerateCsvFile(PlayerDto[] players, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.Delimiter = ", ";
                csv.WriteRecords(players);
            }
        }

        public static PlayerDto[] FilterPlayers(Player[] players, double minRating, int maxYears)
        {
            return players.Where(p => p.Rating > minRating && DateTime.Now.Year - p.PlayerSince < maxYears)
                          .OrderByDescending(p => p.Rating)
                          .ThenBy(p => p.Name)
                          .Select(p => new PlayerDto
                          {
                              Name = p.Name,
                              Rating = $"{p.Rating:f1}"
                          })
                          .ToArray();
        }

        public static void ValidatePath(string path, string suffix, string message)
        {
            if (string.IsNullOrEmpty(path) || !path.EndsWith(suffix))
            {
                Console.WriteLine(message);
                Environment.Exit(0);
            }
        }

        public static void ValidateInput(string inputPath, int maxYears, double minRating, string outputPath)
        {
            string jsonSuffix = ".json";
            string csvSuffix = ".csv";
            string invalidInputPathMessage = "Provided input path is invaid!";
            string invalidMaxYearsMessage = "Max years value must be larger than 0!";
            string invalidMinRatingMessage = "Minimum Rating value must be larger than 0!";
            string invalidOutputPathMessage = invalidInputPathMessage.Replace("input", "output");

            ValidatePath(inputPath, jsonSuffix, invalidInputPathMessage);

            if (maxYears <= 0 || minRating <= 0)
            {
                if (maxYears > 0)
                {
                    Console.WriteLine(invalidMinRatingMessage);
                }
                else
                {
                    Console.WriteLine(invalidMaxYearsMessage);
                }
                Environment.Exit(0);
            }

            ValidatePath(outputPath, csvSuffix, invalidOutputPathMessage);
        }
    }
}