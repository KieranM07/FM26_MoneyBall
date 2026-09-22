
namespace MoneyBallApp.Models
{

    public class Striker: Player
    {
        public float? MinutesSinceLastGoal { get; set; }
        public int? Goals { get; set; }
        public int? Off { get; set; }
        public int? ShotsOnTarget { get; set; }
        public float? Shot90 { get; set; }
        public float? ShotsOnTargetPercentage { get; set; }
        public int? GoalsFromOutsideTheBox { get; set; }
        public float? NPxG90 { get; set; }
        public float? Dribblesper90 { get; set; }
        public float? ConvPercentage { get; set; }
        public float? xG { get; set; }
        public float? PresC90 { get; set; }
        public float? ShotsOnTargetper90 { get; set; }
        public int? Shots { get; set; }
        public float? xGShot { get; set; }
        public float? PossessionLostper90 { get; set; }
        public int? PassCompletionPercentage { get; set; }
        public int? ClearCutChancesCreated { get; set; }
        public float? ChancesCreatedper90 { get; set; }
        public float? Asts90 { get; set; }
        public float? Sprints90 { get; set; }
        public float? NPxG { get; set; }
        public float? Dist90 { get; set; }
        public int? FoulsAgainst { get; set; }
        public int? Assists { get; set; }
        public float? Goalsper90minutes { get; set; }
        public float? MinsGm { get; set; }
    }
}
