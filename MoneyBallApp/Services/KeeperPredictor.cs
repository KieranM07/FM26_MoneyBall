namespace MoneyBallApp.Services
{
    using HtmlAgilityPack;
    using MoneyBallApp.Models;
    using System.Net;
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    public class KeeperPredictorService
    {
        private readonly InferenceSession _session;

        public KeeperPredictorService([FromKeyedServices("goalkeeper")] InferenceSession session)
        {
            _session = session;
        }

        public void PredictKeepers(List<Keeper> keepers)
        {
            foreach (var keeper in keepers)
            {
                var prediction = Predict(keeper);

                keeper.PredictedValue = prediction;
            }
        }

        private float Predict(Keeper keeper)
        {
            var features = new float[]
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
            keeper.Age?? 0
            };

            var tensor = new DenseTensor<float>(
                features,
                new[] { 1, features.Length }
            );

            var inputs = new[]
            {
            NamedOnnxValue.CreateFromTensor("input", tensor)
        };

            using var results = _session.Run(inputs);

            return results.First().AsEnumerable<float>().First();
        }
    }
}
