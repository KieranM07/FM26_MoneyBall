namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;

    public class STParser
    {
        public List<Striker> ParseStrikers(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            var rows = table?.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
                return new List<Striker>();

            // Build header -> column index map
            var headerMap = (rows.First()!.SelectNodes(".//th|.//td")
                ?? Enumerable.Empty<HtmlNode>())
                .Select((cell, index) => new
                {
                    Name = cell.InnerText.Trim(),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            var strikers = new List<Striker>();

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null) continue;

                var st = new Striker
                {

                    Inf = GetString(cells, headerMap, "Inf"),
                    Nation = GetString(cells, headerMap, "Nation"),
                    PlayerName = GetString(cells, headerMap, "Player"),
                    TransferValue = ConvertTransferValue(GetString(cells, headerMap, "Transfer Value")),
                    Age = GetInt(cells, headerMap, "Age"),
                    Wage = GetString(cells, headerMap, "Wage"),
                    Club = GetString(cells, headerMap, "Club"),
                    MinutesSinceLastGoal = GetFloat(cells, headerMap, "Minutes Since Last Goal"),
                    Goals = GetInt(cells, headerMap, "Goals"),
                    Off = GetInt(cells, headerMap, "Off"),
                    ShotsOnTarget = GetInt(cells, headerMap, "Shots on Target"),
                    Shot90 = GetFloat(cells, headerMap, "Shot/90"),
                    ShotsOnTargetPercentage = StripPercentage(GetString(cells, headerMap, "Shots on Target Percentage")),
                    GoalsFromOutsideTheBox = GetInt(cells, headerMap, "Goals From Outside The Box"),
                    NPxG90 = GetFloat(cells, headerMap, "NP-xG/90"),
                    Dribblesper90 = GetFloat(cells, headerMap, "Dribbles per 90"),
                    ConvPercentage = StripPercentage(GetString(cells, headerMap, "Conv %")),
                    xG = GetFloat(cells, headerMap, "xG"),
                    PresC90 = GetFloat(cells, headerMap, "Pres C/90"),
                    ShotsOnTargetper90 = GetFloat(cells, headerMap, "Shots on Target per 90"),
                    Shots = GetInt(cells, headerMap, "Shots"),
                    xGShot = GetFloat(cells, headerMap, "xG/shot"),
                    PossessionLostper90 = GetFloat(cells, headerMap, "Possession Lost per 90"),
                    PassCompletionPercentage = StripPercentage(GetString(cells, headerMap, "Pass Completion Percentage")),
                    ClearCutChancesCreated = GetInt(cells, headerMap, "Clear Cut Chances Created"),
                    ChancesCreatedper90 = GetFloat(cells, headerMap, "Chances Created per 90"),
                    Asts90 = GetFloat(cells, headerMap, "Asts/90"),
                    Sprints90 = GetFloat(cells, headerMap, "Sprints/90"),
                    NPxG = GetFloat(cells, headerMap, "NP-xG"),
                    Dist90 = GetFloat(cells, headerMap, "Dist/90"),
                    FoulsAgainst = GetInt(cells, headerMap, "Fouls Against"),
                    Assists = GetInt(cells, headerMap, "Assists"),
                    Goalsper90minutes = GetFloat(cells, headerMap, "Goals per 90 minutes"),
                    MinsGm = GetFloat(cells, headerMap, "Mins/Gl"),

                };

                strikers.Add(st);
            }

            return strikers;
        }

        private int? GetInt(HtmlNodeCollection cells, Dictionary<string, int> map, string key)
        {
            if (!map.ContainsKey(key)) return null;
            var text = cells[map[key]].InnerText.Trim();
            return int.TryParse(text, out var value) ? value : null;
        }

        private string? GetString(HtmlNodeCollection cells, Dictionary<string, int> map, string key)
        {
            if (!map.ContainsKey(key)) return null;
            return cells[map[key]].InnerText.Trim();
        }

        private float? GetFloat(HtmlNodeCollection cells, Dictionary<string, int> map, string key)
        {
            if (!map.ContainsKey(key)) return null;

            var text = cells[map[key]].InnerText.Trim();
            return float.TryParse(text, out var value) ? value : null;
        }

        private static int? ConvertTransferValue(string? transferValue)
        {
            if (string.IsNullOrWhiteSpace(transferValue))
                return null;

            transferValue = transferValue.Trim().ToUpper();

            if (transferValue.Contains('-'))
            {
                var parts = transferValue.Split('-', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    return null;

                int? min = ConvertSingle(parts[0].Trim());
                int? max = ConvertSingle(parts[1].Trim());

                if (min == null || max == null)
                    return null;

                return (min.Value + max.Value) / 2;
            }

            return ConvertSingle(transferValue);
        }

        private static int? ConvertSingle(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove currency symbols, commas and whitespace
            value = value.Trim()
                         .Replace("£", "")
                         .Replace(",", "")
                         .Replace(" ", "");

       
            if (int.TryParse(value, out int plain))
                return plain;

            // Millions
            if (value.EndsWith("M"))
            {
                string num = value[..^1];

                if (double.TryParse(num, out double d))
                    return (int)(d * 1_000_000);
            }

            // Thousands
            if (value.EndsWith("K"))
            {
                string num = value[..^1];

                if (double.TryParse(num, out double d))
                    return (int)(d * 1_000);
            }

            return null;
        }

        private static int? ConvertHeightToInches(string? height)
        {
            if (string.IsNullOrWhiteSpace(height))
                return null;

            // Convert &#39; to ' and &quot; to "
            height = WebUtility.HtmlDecode(height);

            var parts = height.Trim().Split('\'');

            if (parts.Length != 2)
                return null;

            if (!int.TryParse(parts[0].Trim(), out var feet))
                return null;

            var inchesText = parts[1].Trim().Trim('"');

            if (!int.TryParse(inchesText, out var inches))
                return null;

            if (feet < 0 || inches < 0 || inches > 11)
                return null;

            int finalHeight = (feet * 12) + inches;

            return finalHeight;
        }

        private static int? StripPercentage(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (value.EndsWith('%'))
                value = value[..^1].Trim();

            if (!int.TryParse(value, out var percentage))
                return null;

            return percentage;
        }
    }

}
