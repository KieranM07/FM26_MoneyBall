namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class CBPredictorService
    {
        private readonly InferenceSession _session;

        public CBPredictorService(
            [FromKeyedServices("centreback")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictCentreBacks(List<CentreBack> centrebacks)
        {
            // Remove cbs who have half or fewer of their stats as 0
            centrebacks.RemoveAll(centreback => !HasEnoughStats(centreback));

            // Only predict cbs that passed the minimum stats requirement
            foreach (var centreback in centrebacks)
            {
                centreback.PredictedValue = Predict(centreback);
            }
        }

        private bool HasEnoughStats(CentreBack centreback)
        {
            var features = GetFeatures(centreback);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(CentreBack centreback)
        {
            var features = GetFeatures(centreback);

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

        private float[] GetFeatures(CentreBack centreback)
        {
            return new float[]
            {
                centreback.Age ?? 0,
                centreback.MinsGm ?? 0,
                centreback.TacklesCompletedper90 ?? 0,
                centreback.ShtsBlckd90 ?? 0,
                centreback.PossessionLostper90 ?? 0,
                centreback.HeadersWonper90 ?? 0,
                centreback.HeadersLostper90 ?? 0,
                centreback.Height ?? 0,
                centreback.HeadersAttemptedper90 ?? 0,
                centreback.Redcards ?? 0,
                centreback.YellowCards ?? 0,
                centreback.PreC90 ?? 0,
                centreback.PassesCompletedper90 ?? 0,
                centreback.PresA90 ?? 0,
                centreback.KeyTackles ?? 0,
                centreback.PassesAttemptedper90 ?? 0,
                centreback.PassesAttempted ?? 0,
                centreback.PassCompletionPercentage ?? 0,
                centreback.Clearancesper90 ?? 0,
                centreback.PossessionWonper90 ?? 0,
                centreback.KeyTacklesper90 ?? 0,
                centreback.MistakesLeadingtoGoals ?? 0,
                centreback.FoulsMade ?? 0,
                centreback.Interceptionsper90 ?? 0,
                centreback.Blk90 ?? 0,
                centreback.TackleCompletionPercentage ?? 0,
                centreback.ProgressivePassesper90 ?? 0,
                centreback.KeyHeadersper90 ?? 0,
                centreback.HeadersWonPercentage ?? 0
            };
        }
    }
}