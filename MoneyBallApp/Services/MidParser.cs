namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;

    public class MidParser
    {
        public List<Mid> ParseMids(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            var rows = table?.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
                return new List<Mid>();

            // Build header -> column index map
            var headerMap = (rows.First()!.SelectNodes(".//th|.//td")
                ?? Enumerable.Empty<HtmlNode>())
                .Select((cell, index) => new
                {
                    Name = cell.InnerText.Trim(),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            var mids = new List<Mid>();

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null) continue;

                var m = new Mid
                {

                    Inf = GetString(cells, headerMap, "Inf"),
                    Nation = GetString(cells, headerMap, "Nation"),
                    PlayerName = GetString(cells, headerMap, "Player"),
                    TransferValue = ConvertTransferValue(GetString(cells, headerMap, "Transfer Value")),
                    Age = GetInt(cells, headerMap, "Age"),
                    Wage = GetString(cells, headerMap, "Wage"),
                    Club = GetString(cells, headerMap, "Club"),
                    FoulsMade = GetInt(cells, headerMap, "Fouls Made"),
                    Dist90 = GetFloat(cells, headerMap, "Dist/90"),
                    KeyPassesper90 = GetFloat(cells, headerMap, "Key Passes per 90"),
                    CrossesCompletedper90 = GetFloat(cells, headerMap, "Crosses Completed per 90"),
                    PsP = GetFloat(cells, headerMap, "PsP"),
                    TacklesCompletedper90 = GetFloat(cells, headerMap, "Tackles Completed per 90"),
                    ConvPercentage = StripPercentage(GetString(cells, headerMap, "Conv %")),
                    FreeKickShots = GetInt(cells, headerMap, "Free Kick Shots"),
                    MinsGm = GetFloat(cells, headerMap, "Mins/Gm"),
                    RedCards = GetInt(cells, headerMap, "Red cards"),
                    YellowCards = GetInt(cells, headerMap, "Yellow Cards"),
                    OpenPlayCrossesCompletedper90 = GetFloat(cells, headerMap, "Open Play Crosses Completed per 90"),
                    ClearCutChancesCreated = GetInt(cells, headerMap, "Clear Cut Chances Created"),
                    PassCompletionPercentage = StripPercentage(GetString(cells, headerMap, "Pass Completion Percentage")),
                    Asts90 = GetFloat(cells, headerMap, "Asts/90"),
                    KeyTacklesper90 = GetFloat(cells, headerMap, "Key Tackles per 90"),
                    PresC90 = GetFloat(cells, headerMap, "Pres C/90"),
                    PresA90 = GetFloat(cells, headerMap, "Pres A/90"),
                    GoalsFromOutsideTheBox = GetInt(cells, headerMap, "Goals From Outside The Box"),
                    Goalsper90minutes = GetFloat(cells, headerMap, "Goals per 90 minutes"),
                    ShotsFromOutsideTheBoxPer90minutes = GetFloat(cells, headerMap, "Shots From Outside The Box Per 90 minutes"),
                    Dribblesper90 = GetFloat(cells, headerMap, "Dribbles per 90"),
                    ProgressivePassesper90 = GetFloat(cells, headerMap, "Progressive Passes per 90"),
                    ChancesCreatedper90 = GetFloat(cells, headerMap, "Chances Created per 90"),
                    PassesCompletedper90 = GetFloat(cells, headerMap, "Passes Completed per 90"),
                    OpenPlayKeyPassesper90 = GetFloat(cells, headerMap, "Open Play Key Passes per 90"),
                    PassesAttemptedper90 = GetFloat(cells, headerMap, "Passes Attempted per 90"),
                    Sprints90 = GetFloat(cells, headerMap, "Sprints/90"),
                    PossessionLostper90 = GetFloat(cells, headerMap, "Possession Lost per 90"),
                    MistakesLeadingtoGoals = GetInt(cells, headerMap, "Mistakes Leading to Goals"),

                };

                mids.Add(m);
            }

            return mids;
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
