namespace MoneyBallApp.Models
{

    public class Keeper : Player
    {
        public int? ExpectedSavePercentage { get; set; }
        public int? SavePercentage { get; set; }
        public float? xGP { get; set; }
        public int? SavesTipped { get; set; }
        public float? MinsGm { get; set; }
        public int? Height { get; set; }
        public int? PassesCompleted { get; set; }
        public float? Con90 { get; set; }
        public float? Cln90 { get; set; }
        public int? CleanSheets { get; set; }
        public float? PassesCompletedper90 { get; set; }
        public int? PenaltiesFaced { get; set; }
        public int? PenaltiesSavedRatio { get; set; }
        public int? PenaltiesSaved { get; set; }
        public float? PassesAttemptedper90 { get; set; }
        public int? PassesAttempted {  get; set; }
        public int? PassCompletionPercentage { get; set;  }
        public int? SavesParried { get; set; }

        public int? SavesHeld { get; set; }
        public float? xGP90 { get; set; }

        public int? MistakesLeadingtoGoals { get; set;  }
    }
}
