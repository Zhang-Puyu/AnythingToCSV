using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convert.Methods;
using NCToolBox.Extension;

namespace Convert.NC
{
    public abstract class ConvertNC : AbstractConverter
    {
        protected abstract NCToolBox.Toolpath.AbsractReader Reader { get; }

        public override void SingleToSingle(string oriFile, string csvFile)
        {
            List<double[]> points = new List<double[]>();
            Reader.Read(ref points, oriFile, encoding: ReadEncoding);

            points.ToCSV(csvFile, Reader.Head, encoding: WriteEncoding);
        }
    }
}
