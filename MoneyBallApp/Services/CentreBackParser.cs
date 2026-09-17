namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;

    public class CentreBackParser
    {
        public List<CentreBack> ParseCentreBacks(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            var rows = table?.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
                return new List<CentreBack>();

            // Build header -> column index map
            var headerMap = (rows.First()!.SelectNodes(".//th|.//td")
                ?? Enumerable.Empty<HtmlNode>())
                .Select((cell, index) => new
                {
                    Name = cell.InnerText.Trim(),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            var centrebacks = new List<CentreBack>();

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null) continue;

                var cb = new CentreBack
                {

                    Inf = GetString(cells, headerMap, "Inf"),
                    Nation = GetString(cells, headerMap, "Nation"),
                    PlayerName = GetString(cells, headerMap, "Player"),
                    TransferValue = ConvertTransferValue(GetString(cells, headerMap, "Transfer Value")),
                    Age = GetInt(cells, headerMap, "Age"),
                    Wage = GetString(cells, headerMap, "Wage"),
                    Club = GetString(cells, headerMap, "Club"),
                    TacklesCompletedper90 = GetFloat(cells, headerMap, "Tackles Completed per 90"),
                    ShtsBlckd90 = GetFloat(cells, headerMap, "Shts Blckd/90"),
                    PossessionLostper90 = GetFloat(cells, headerMap, "Possession Lost per 90"),
                    HeadersWonper90 = GetFloat(cells, headerMap, "Headers Won per 90"),
                    HeadersLostper90 = GetFloat(cells, headerMap, "Headers Lost per 90"),
                    Height = ConvertHeightToInches(GetString(cells, headerMap, "Height")),
                    HeadersAttemptedper90 = GetFloat(cells, headerMap, "Headers Attempted per 90"),
                    Redcards = GetInt(cells, headerMap, "Red cards"),
                    YellowCards = GetInt(cells, headerMap, "Yellow Cards"),
                    PreC90 = GetFloat(cells, headerMap, "Pres C/90"),
                    PassesCompletedper90 = GetFloat(cells, headerMap, "Passes Completed per 90"),
                    PresA90 = GetFloat(cells, headerMap, "Pres A/90"),
                    KeyTackles = GetInt(cells, headerMap, "Key Tackles"),
                    PassesAttemptedper90 = GetFloat(cells, headerMap, "Passes Attempted per 90"),
                    PassesAttempted = GetInt(cells, headerMap, "Passes Attempted"),
                    PassCompletionPercentage = StripPercentage(GetString(cells, headerMap, "Pass Completion Percentage")),
                    Clearancesper90 = GetFloat(cells, headerMap, "Clearances per 90"),
                    PossessionWonper90 = GetFloat(cells, headerMap, "Possession Won per 90"),
                    KeyTacklesper90 = GetFloat(cells, headerMap, "Key Tackles per 90"),
                    MistakesLeadingtoGoals = GetInt(cells, headerMap, "Mistakes Leading to Goals"),
                    FoulsMade = GetInt(cells, headerMap, "Fouls Made"),
                    Interceptionsper90 = GetFloat(cells, headerMap, "Interceptions per 90"),
                    Blk90 = GetFloat(cells, headerMap, "Blk/90"),
                    TackleCompletionPercentage = GetInt(cells, headerMap, "Tackle Completion Percentage"),
                    ProgressivePassesper90 = GetFloat(cells, headerMap, "Progressive Passes per 90"),
                    KeyHeadersper90 = GetFloat(cells, headerMap, "Key Headers per 90"),
                    HeadersWonPercentage = GetFloat(cells, headerMap, "Headers Won Percentage")
                };

                centrebacks.Add(cb);
            }

            return centrebacks;
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
