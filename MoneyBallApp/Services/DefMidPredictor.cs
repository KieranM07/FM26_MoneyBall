namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class DMPredictorService
    {
        private readonly InferenceSession _session;

        public DMPredictorService(
            [FromKeyedServices("defmid")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictDefMids(List<DefMid> defmids)
        {
            // Remove dms who have half or fewer of their stats as 0
            defmids.RemoveAll(defmid => !HasEnoughStats(defmid));

            // Only predict dms that passed the minimum stats requirement
            foreach (var defmid in defmids)
            {
                defmid.PredictedValue = Predict(defmid);
            }
        }

        private bool HasEnoughStats(DefMid defmid)
        {
            var features = GetFeatures(defmid);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(DefMid defmid)
        {
            var features = GetFeatures(defmid);

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

        private float[] GetFeatures(DefMid defmid)
        {
            return new float[]
            {
                defmid.Age ?? 0,
                defmid.Interceptionsper90 ?? 0,
                defmid.Blk90 ?? 0,
                defmid.KeyTackles ?? 0,
                defmid.PassCompletionPercentage ?? 0,
                defmid.YellowCards ?? 0,
                defmid.RedCards ?? 0,
                defmid.TacklesCompletedper90 ?? 0,
                defmid.PresA90 ?? 0,
                defmid.PossessionLostper90 ?? 0,
                defmid.PresC90 ?? 0,
                defmid.KeyPassesper90 ?? 0,
                defmid.PossessionWonper90 ?? 0,
                defmid.TackleCompletionPercentage ?? 0,
                defmid.Dist90 ?? 0,
                defmid.ChancesCreatedper90 ?? 0,
                defmid.ProgressivePassesper90 ?? 0,
                defmid.PsP ?? 0,
                defmid.ClearCutChancesCreated ?? 0,
                defmid.PassesCompletedper90 ?? 0,
                defmid.MistakesLeadingtoGoals ?? 0,
                defmid.OpenPlayKeyPassesper90 ?? 0,
                defmid.KeyTacklesper90 ?? 0,
                defmid.MinsGm ?? 0,
                defmid.FoulsMade ?? 0,
            };
        }
    }
}