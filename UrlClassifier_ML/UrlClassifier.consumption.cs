using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
namespace UrlClassifier_ML
{
    public partial class UrlClassifier
    {
        /// <summary>
        /// model input class for UrlClassifier.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [ColumnName(@"url")]
            public string Url { get; set; }

            [ColumnName(@"type")]
            public string Type { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for UrlClassifier.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName("PredictedLabel")]
            public string Prediction { get; set; }

            public float[] Score { get; set; }
        }

        #endregion

        private static readonly string MLNetModelPath = Path.GetFullPath("C:\\Users\\nikos\\Desktop\\UrlShortener\\UrlClassifier_ML\\UrlClassifier.zip");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}
