namespace MoneyBallApp.Models
{

    public class Keeper : Player
    {
        public int? ExpectedSavePercentage { get; set; }
        public int? SavePercentage { get; set; }
        public int? xGP { get; set; }
        public int? SavesTipped { get; set; }
        public int? MinsGm { get; set; }
        public int? Height { get; set; }
        public int? PassesCompleted { get; set; }
        public int? Con90 { get; set; }
        public int? Cln90 { get; set; }
        public int? CleanSheets { get; set; }
        public int? PassesCompletedper90 { get; set; }
        public int? PenaltiesFaced { get; set; }
        public int? PenaltiesSavedRatio { get; set; }
        public int? PassesAttemptedper90 { get; set; }
        public int? PassesAttempted {  get; set; }
        public int? PassCompletionPercentage { get; set;  }
        public int? SavesParried { get; set; }

        public int? SavesHeld { get; set; }
        public int? xGP90 { get; set; }

        public int? MistakesLeadingtoGoals { get; set;  }
    }
}
