using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateAppWPF.MachineLearning
{
    public class OcrDataLoader
    {
        private readonly MLContext _mlContext;
        private readonly string _dataPath;

        public OcrDataLoader(string dataPath)
        {
            _mlContext = new MLContext();
            _dataPath = dataPath;
        }

        public IDataView LoadData()
        {
            // Load the data
            IDataView dataView = _mlContext.Data.LoadFromTextFile<OcrModelInput>(
                path: _dataPath,
                hasHeader: true,
                separatorChar: ','
            );

            return dataView;
        }

        public IDataView PrepareData(IDataView dataView)
        {
            // Data processing pipeline
            var dataProcessPipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText("Features", nameof(OcrModelInput.Text)));

            // Apply transformations
            IDataView transformedData = dataProcessPipeline.Fit(dataView).Transform(dataView);

            return transformedData;
        }
    }
}
