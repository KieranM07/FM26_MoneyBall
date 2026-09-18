
namespace MoneyBallApp.Models
{

    public class CentreBack: Player
    {
        public int? Height { get; set; }
        public float? TacklesCompletedper90 { get; set; }
        public float? ShtsBlckd90 { get; set; }
        public float? PossessionLostper90 { get; set; }
        public float? HeadersWonper90 { get; set; }
        public float? HeadersLostper90{ get; set; }
        public float? HeadersAttemptedper90{ get; set; }
        public int? Redcards { get; set; }
        public int? YellowCards { get; set; }
        public float? PreC90{ get; set; }
        public float? PresA90 { get; set; }
        public int? KeyTackles { get; set; }
        public float? Clearancesper90 { get; set; }
        public float? PossessionWonper90 { get; set; }
        public float? KeyTacklesper90{  get; set; }
        public float? Interceptionsper90{ get; set;  }
        public float? Blk90 { get; set; }
        public int? TackleCompletionPercentage { get; set; }
        public float? ProgressivePassesper90 { get; set; }
        public float? KeyHeadersper90 { get; set;  }
        public float? HeadersWonPercentage { get; set; }
        public int? FoulsMade { get; set; }
        public float? PassesCompletedper90 { get; set; }
        public float? PassesAttemptedper90 { get; set; }
        public int? PassesAttempted { get; set; }
        public int? PassCompletionPercentage { get; set; }
        public int? MistakesLeadingtoGoals { get; set; }
        public float? MinsGm { get; internal set; }
    }
}
