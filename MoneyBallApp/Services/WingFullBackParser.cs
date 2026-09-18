namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;

    public class WingFullBackParser
    {
        public List<WingFullBack> ParseWingFullBacks(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            var rows = table?.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
                return new List<WingFullBack>();

            // Build header -> column index map
            var headerMap = (rows.First()!.SelectNodes(".//th|.//td")
                ?? Enumerable.Empty<HtmlNode>())
                .Select((cell, index) => new
                {
                    Name = cell.InnerText.Trim(),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            var wingfullbacks = new List<WingFullBack>();

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null) continue;

                var wfb = new WingFullBack
                {

                    Inf = GetString(cells, headerMap, "Inf"),
                    Nation = GetString(cells, headerMap, "Nation"),
                    PlayerName = GetString(cells, headerMap, "Player"),
                    TransferValue = ConvertTransferValue(GetString(cells, headerMap, "Transfer Value")),
                    Age = GetInt(cells, headerMap, "Age"),
                    Wage = GetString(cells, headerMap, "Wage"),
                    Club = GetString(cells, headerMap, "Club"),
                    CrossesAttemptedPer90 = GetFloat(cells, headerMap, "Crosses Attempted per 90"),
                    AstsPer90 = GetFloat(cells, headerMap, "Asts/90"),
                    FoulsMade = GetInt(cells, headerMap, "Fouls Made"),
                    ChancesCreatedPer90 = GetFloat(cells, headerMap, "Chances Created per 90"),
                    OpenPlayCrossCompletionPercentage = GetInt(cells, headerMap, "Open Play Cross Completion Percentage"),
                    PossessionWonPer90 = GetFloat(cells, headerMap, "Possession Won per 90"),
                    TackleCompletionPercentage = GetInt(cells, headerMap, "Tackle Completion Percentage"),
                    XA90 = GetFloat(cells, headerMap, "xA/90"),
                    KeyTackles = GetInt(cells, headerMap, "Key Tackles"),
                    ClearCutChancesCreated = GetInt(cells, headerMap, "Clear Cut Chances Created"),
                    PassesAttemptedPer90 = GetFloat(cells, headerMap, "Passes Attempted per 90"),
                    PsP = GetInt(cells, headerMap, "PsP"),
                    PassCompletionPercentage = GetInt(cells, headerMap, "Pass Completion Percentage"),
                    SprintsPer90 = GetFloat(cells, headerMap, "Sprints/90"),
                    CrossesCompletedRatio = GetInt(cells, headerMap, "Crosses Completed Ratio"),
                    PresCPer90 = GetFloat(cells, headerMap, "Pres C/90"),
                    PossessionLostPer90 = GetFloat(cells, headerMap, "Possession Lost per 90"),
                    TacklesCompletedPer90 = GetFloat(cells, headerMap, "Tackles Completed per 90"),
                    KeyTacklesPer90 = GetFloat(cells, headerMap, "Key Tackles per 90"),
                    KeyPassesPer90 = GetFloat(cells, headerMap, "Key Passes per 90"),
                    OpenPlayKeyPassesPer90 = GetFloat(cells, headerMap, "Open Play Key Passes per 90"),
                    ProgressivePassesPer90 = GetFloat(cells, headerMap, "Progressive Passes per 90"),
                    OpenPlayCrossesCompletedPer90 = GetFloat(cells, headerMap, "Open Play Crosses Completed per 90"),
                    OpenPlayCrossesAttemptedPer90 = GetFloat(cells, headerMap, "Open Play Crosses Attempted per 90"),
                    CrossesCompletedPer90 = GetFloat(cells, headerMap, "Crosses Completed per 90"),
                    MistakesLeadingToGoals = GetInt(cells, headerMap, "Mistakes Leading to Goals"),
                };

                wingfullbacks.Add(wfb);
            }

            return wingfullbacks;
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
