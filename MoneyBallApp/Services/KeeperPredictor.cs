namespace MoneyBallApp.Services
{
    using MoneyBallApp.Models;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class KeeperPredictorService
    {
        private readonly InferenceSession _session;

        public KeeperPredictorService(
            [FromKeyedServices("goalkeeper")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictKeepers(List<Keeper> keepers)
        {
            // Remove keepers who have half or fewer of their stats as 0
            keepers.RemoveAll(keeper => !HasEnoughStats(keeper));

            // Only predict keepers that passed the minimum stats requirement
            foreach (var keeper in keepers)
            {
                keeper.PredictedValue = Predict(keeper);
            }
        }

        private bool HasEnoughStats(Keeper keeper)
        {
            var features = GetFeatures(keeper);

            int nonZeroStats = features.Count(x => x != 0);
            return nonZeroStats > features.Length / 2;
        }

        private float Predict(Keeper keeper)
        {
            var features = GetFeatures(keeper);

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

        private float[] GetFeatures(Keeper keeper)
        {
            return new float[]
            {
                keeper.ExpectedSavePercentage ?? 0,
                keeper.SavePercentage ?? 0,
                keeper.xGP ?? 0,
                keeper.SavesTipped ?? 0,
                keeper.MinsGm ?? 0,
                keeper.Height ?? 0,
                keeper.PassesCompleted ?? 0,
                keeper.Con90 ?? 0,
                keeper.Cln90 ?? 0,
                keeper.CleanSheets ?? 0,
                keeper.PassesCompletedper90 ?? 0,
                keeper.PenaltiesFaced ?? 0,
                keeper.PenaltiesSavedRatio ?? 0,
                keeper.PenaltiesSaved ?? 0,
                keeper.PassesAttemptedper90 ?? 0,
                keeper.PassesAttempted ?? 0,
                keeper.PassCompletionPercentage ?? 0,
                keeper.SavesParried ?? 0,
                keeper.SavesHeld ?? 0,
                keeper.xGP90 ?? 0,
                keeper.MistakesLeadingtoGoals ?? 0,
                keeper.Age ?? 0
            };
        }
    }
}