using System;
using Microsoft.ML.Data;

namespace TranslateAppWPF.MachineLearning
{
    public class OcrModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }

        public float[] Score { get; set; }
    }
}
