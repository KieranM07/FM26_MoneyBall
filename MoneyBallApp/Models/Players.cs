namespace MoneyBallApp.Models
{
    public abstract class Player
    {
        public string? Inf { get; set; }
        public string? PlayerName { get; set; }
        public string? Nation { get; set; }
        public int? TransferValue { get; set; }
        public int? Age { get; set; }
        public string? Wage { get; set; }

        public string? Club { get; set; }

        public float? PredictedValue {  get; set; }
    }
}
