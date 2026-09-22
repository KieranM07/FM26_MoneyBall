namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class WPredictorService
    {
        private readonly InferenceSession _session;

        public WPredictorService(
            [FromKeyedServices("winger")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictWingers(List<Winger> wingers)
        {
            // Remove wingers who have half or fewer of their stats as 0
            wingers.RemoveAll(winger => !HasEnoughStats(winger));

            // Only predict wingers that passed the minimum stats requirement
            foreach (var winger in wingers)
            {
                winger.PredictedValue = Predict(winger);
            }
        }

        private bool HasEnoughStats(Winger winger)
        {
            var features = GetFeatures(winger);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(Winger winger)
        {
            var features = GetFeatures(winger);

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

        private float[] GetFeatures(Winger winger)
        {
            return new float[]
            {
                winger.Age ?? 0,
                winger.NPxG90 ?? 0,
                winger.Dribblesper90 ?? 0,
                winger.KeyPassesper90 ?? 0,
                winger.ConvPercentage ?? 0,
                winger.KeyTacklesper90 ?? 0,
                winger.TackleCompletionPercentage ?? 0,
                winger.Interceptionsper90 ?? 0,
                winger.xG ?? 0,
                winger.PresC90 ?? 0,
                winger.ShotsOnTargetper90 ?? 0,
                winger.Shots ?? 0,
                winger.xGperShot ?? 0,
                winger.ProgressivePassesper90 ?? 0,
                winger.PossessionLostper90 ?? 0,
                winger.PassCompletionPercentage ?? 0,
                winger.OpenPlayKeyPassesper90 ?? 0,
                winger.ClearCutChancesCreated ?? 0,
                winger.ChancesCreatedper90 ?? 0,
                winger.Asts90 ?? 0,
                winger.Sprints90 ?? 0,
                winger.NPxG ?? 0,
                winger.Dist90 ?? 0,
                winger.FoulsMade ?? 0,
                winger.Assists ?? 0,
                winger.Goalsper90minutes ?? 0,
                winger.CrossesCompletedRatio ?? 0,
                winger.MinsGm ?? 0,
                winger.OpenPlayCrossesCompletedper90 ?? 0,
                winger.OpenPlayCrossesAttemptedper90 ?? 0,
                winger.OpenPlayCrossCompletionPercentage ?? 0,
                winger.CrossesCompletedper90 ?? 0,
                winger.CrossesAttemptedper90 ?? 0
            };
        }
    }
}