namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class MPredictorService
    {
        private readonly InferenceSession _session;

        public MPredictorService(
            [FromKeyedServices("mid")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictMids(List<Mid> mids)
        {
            // Remove ms who have half or fewer of their stats as 0
            mids.RemoveAll(mid => !HasEnoughStats(mid));

            // Only predict ms that passed the minimum stats requirement
            foreach (var mid in mids)
            {
                mid.PredictedValue = Predict(mid);
            }
        }

        private bool HasEnoughStats(Mid mid)
        {
            var features = GetFeatures(mid);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(Mid mid)
        {
            var features = GetFeatures(mid);

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

        private float[] GetFeatures(Mid mid)
        {
            return new float[]
            {
                mid.Age ?? 0,
                mid.FoulsMade ?? 0,
                mid.Dist90 ?? 0,
                mid.KeyPassesper90 ?? 0,
                mid.CrossesCompletedper90 ?? 0,
                mid.PsP ?? 0,
                mid.TacklesCompletedper90 ?? 0,
                mid.ConvPercentage ?? 0,
                mid.FreeKickShots ?? 0,
                mid.MinsGm ?? 0,
                mid.RedCards ?? 0,
                mid.YellowCards ?? 0,
                mid.OpenPlayCrossesCompletedper90 ?? 0,
                mid.ClearCutChancesCreated ?? 0,
                mid.PassCompletionPercentage ?? 0,
                mid.Asts90 ?? 0,
                mid.KeyTacklesper90 ?? 0,
                mid.PresC90 ?? 0,
                mid.PresA90 ?? 0,
                mid.GoalsFromOutsideTheBox ?? 0,
                mid.Goalsper90minutes ?? 0,
                mid.ShotsFromOutsideTheBoxPer90minutes ?? 0,
                mid.Dribblesper90 ?? 0,
                mid.ProgressivePassesper90 ?? 0,
                mid.ChancesCreatedper90 ?? 0,
                mid.PassesCompletedper90 ?? 0,
                mid.OpenPlayKeyPassesper90 ?? 0,
                mid.PassesAttemptedper90 ?? 0,
                mid.Sprints90 ?? 0,
                mid.PossessionLostper90 ?? 0,
                mid.MistakesLeadingtoGoals ?? 0
            };
        }
    }
}