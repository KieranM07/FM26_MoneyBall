namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class WFBPredictorService
    {
        private readonly InferenceSession _session;

        public WFBPredictorService(
            [FromKeyedServices("wingfullback")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictWingFullBacks(List<WingFullBack> wingfullbacks)
        {
            // Remove keepers who have half or fewer of their stats as 0
            wingfullbacks.RemoveAll(wingfullback => !HasEnoughStats(wingfullback));

            // Only predict keepers that passed the minimum stats requirement
            foreach (var wingfullback in wingfullbacks)
            {
                wingfullback.PredictedValue = Predict(wingfullback);
            }
        }

        private bool HasEnoughStats(WingFullBack wingfullback)
        {
            var features = GetFeatures(wingfullback);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(WingFullBack wingfullback)
        {
            var features = GetFeatures(wingfullback);

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

        private float[] GetFeatures(WingFullBack wingfullback)
        {
            return new float[]
            {
                wingfullback.Age ?? 0,
                wingfullback.KeyTackles ?? 0,
                wingfullback.PassCompletionPercentage ?? 0,
                wingfullback.PresCPer90 ?? 0,
                wingfullback.MistakesLeadingToGoals ?? 0,
                wingfullback.FoulsMade ?? 0,
                wingfullback.PassesAttemptedPer90 ?? 0,
                wingfullback.ProgressivePassesPer90 ?? 0,
                wingfullback.TackleCompletionPercentage ?? 0,
                wingfullback.ChancesCreatedPer90 ?? 0,
                wingfullback.KeyPassesPer90 ?? 0,
                wingfullback.OpenPlayKeyPassesPer90 ?? 0,
                wingfullback.CrossesAttemptedPer90 ?? 0,
                wingfullback.CrossesCompletedPer90 ?? 0,
                wingfullback.OpenPlayCrossesCompletedPer90 ?? 0,
                wingfullback.OpenPlayCrossesAttemptedPer90 ?? 0,
                wingfullback.AstsPer90 ?? 0,
                wingfullback.XA90 ?? 0,
                wingfullback.SprintsPer90 ?? 0,
                wingfullback.ClearCutChancesCreated ?? 0,
                wingfullback.OpenPlayCrossCompletionPercentage ?? 0,
                wingfullback.PossessionWonPer90 ?? 0,
                wingfullback.PsP ?? 0,
                wingfullback.CrossesCompletedRatio ?? 0,
                wingfullback.TacklesCompletedPer90 ?? 0,
                wingfullback.KeyTacklesPer90 ?? 0,
                wingfullback.PossessionLostPer90 ?? 0,
            };
        }
    }
}