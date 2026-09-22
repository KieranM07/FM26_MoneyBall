namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class STPredictorService
    {
        private readonly InferenceSession _session;

        public STPredictorService(
            [FromKeyedServices("striker")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictStrikers(List<Striker> strikers)
        {
            // Remove Strikers who have half or fewer of their stats as 0
            strikers.RemoveAll(striker => !HasEnoughStats(striker));

            // Only predict sts that passed the minimum stats requirement
            foreach (var striker in strikers)
            {
                striker.PredictedValue = Predict(striker);
            }
        }

        private bool HasEnoughStats(Striker striker)
        {
            var features = GetFeatures(striker);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(Striker striker)
        {
            var features = GetFeatures(striker);

            var tensor = new DenseTensor<float>(
                features,
                new[] { 1, features.Length }
            );

            var inputs = new[]
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };

            using var results = _session.Run(inputs);

            return results
                .First()
                .AsEnumerable<float>()
                .First();
        }

        private float[] GetFeatures(Striker striker)
        {
            return new float[]
            {
                striker.Age ?? 0,
                striker.MinutesSinceLastGoal ?? 0,
                striker.Goals ?? 0,
                striker.Off ?? 0,
                striker.ShotsOnTarget ?? 0,
                striker.Shot90 ?? 0,
                striker.ShotsOnTargetPercentage ?? 0,
                striker.GoalsFromOutsideTheBox ?? 0,
                striker.NPxG90 ?? 0,
                striker.Dribblesper90 ?? 0,
                striker.ConvPercentage ?? 0,
                striker.xG ?? 0,
                striker.PresC90 ?? 0,
                striker.ShotsOnTargetper90 ?? 0,
                striker.Shots ?? 0,
                striker.xGShot ?? 0,
                striker.PossessionLostper90 ?? 0,
                striker.PassCompletionPercentage ?? 0,
                striker.ClearCutChancesCreated ?? 0,
                striker.ChancesCreatedper90 ?? 0,
                striker.Asts90 ?? 0,
                striker.Sprints90 ?? 0,
                striker.NPxG ?? 0,
                striker.Dist90 ?? 0,
                striker.FoulsAgainst ?? 0,
                striker.Assists ?? 0,
                striker.Goalsper90minutes ?? 0,
                striker.MinsGm ?? 0
            };
        }
    }
}