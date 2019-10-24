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
            var input = Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            var inputPath = input[0];
            var maxYears = int.Parse(input[1]);
            var minRating = double.Parse(input[2]);
            var outputPath = input[3];
            

            // Json file must be encoded in UTF-8 or deseriazing will not work!!!
            var deserializedPlayers = JsonConvert.DeserializeObject<Player[]>(File.ReadAllText(inputPath));

            var filteredPlayers = FilterPlayers(deserializedPlayers, minRating, maxYears);

            GenerateCsvFile(filteredPlayers, outputPath);
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
                          .Select(p => new PlayerDto
                          {
                              Name = p.Name,
                              Rating = $"{p.Rating:f1}"
                          })
                          .ToArray();
        }
    }
}