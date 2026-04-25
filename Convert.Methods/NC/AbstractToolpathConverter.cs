using System.Collections.Generic;
using MachKit.Data;
using MachKit.NC.Toolpath;

using Convert.Methods.Converters;

namespace Convert.Methods.NC
{
    public abstract class AbstractToolpathConverter : AbstractConverter
    {
        protected abstract AbstractToolpathReader Reader { get; }

        public override void ConvertSingleToSingle(string oriFile, string csvFile)
        {
            List<double[]> points = new List<double[]>();
            Reader.Read(ref points, oriFile, encoding: ReadEncoding);

            CSVFileIO Writer = new CSVFileIO(csvFile)
            {
                Heads = Reader.Heads,
                Encoding = WriteEncoding
            };
			Writer.WriteCSV(points);
        }
    }
}
