using System;
using Microsoft.ML.Data;

namespace TranslateAppWPF.MachineLearning
{
    public class OcrModelInput
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public string Label { get; set; }
    }
}
