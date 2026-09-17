namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;


    public class KeeperParser
    {
        public List<Keeper> ParseKeepers(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            var rows = table?.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
                return new List<Keeper>();

            // Build header -> column index map
            var headerMap = (rows.First()!.SelectNodes(".//th|.//td")
                ?? Enumerable.Empty<HtmlNode>())
                .Select((cell, index) => new
                {
                    Name = cell.InnerText.Trim(),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            var keepers = new List<Keeper>();

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null) continue;

                var k = new Keeper
                {
                    Inf = GetString(cells, headerMap, "Inf"),
                    PlayerName = GetString(cells, headerMap, "Player"),
                    Nation = GetString(cells, headerMap, "Nation"),
                    Club = GetString(cells, headerMap, "Club"),
                    ExpectedSavePercentage = GetInt(cells, headerMap, "Expected Save Percentage"),
                    SavePercentage = GetInt(cells, headerMap, "Save Percentage"),
                    xGP = GetFloat(cells, headerMap, "xGP"),
                    SavesTipped = GetInt(cells, headerMap, "Saves Tipped"),
                    MinsGm = GetFloat(cells, headerMap, "Mins/Gm"),
                    Height = ConvertHeightToInches(GetString(cells, headerMap, "Height")),
                    PassesCompleted = GetInt(cells, headerMap, "Passes Completed"),
                    Con90 = GetFloat(cells, headerMap, "Con/90"),
                    Cln90 = GetFloat(cells, headerMap, "Cln/90"),
                    CleanSheets = GetInt(cells, headerMap, "Clean Sheets"),
                    PenaltiesSaved = GetInt(cells, headerMap, "Penalties Saved"),
                    PassesCompletedper90 = GetFloat(cells, headerMap, "Passes Completed per 90"),
                    PenaltiesFaced = GetInt(cells, headerMap, "Penalties Faced"),
                    PenaltiesSavedRatio = GetInt(cells, headerMap, "Penalties Saved Ratio"),
                    PassesAttemptedper90 = GetFloat(cells, headerMap, "Passes Attempted per 90"),
                    PassesAttempted = GetInt(cells, headerMap, "Passes Attempted"),
                    PassCompletionPercentage = StripPercentage(GetString(cells, headerMap, "Pass Completion Percentage")),
                    SavesParried = GetInt(cells, headerMap, "Saves Parried"),
                    SavesHeld = GetInt(cells, headerMap, "Saves Held"),
                    xGP90 = GetFloat(cells, headerMap, "xGP/90"),
                    MistakesLeadingtoGoals = GetInt(cells, headerMap, "Mistakes Leading to Goals"),
                    Age = GetInt(cells, headerMap, "Age"),
                    TransferValue = GetString(cells, headerMap, "Transfer Value"),
                    Wage = GetString(cells, headerMap, "Wage")
                };

                keepers.Add(k);
            }

            return keepers;
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

            return (feet * 12) + inches;
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
