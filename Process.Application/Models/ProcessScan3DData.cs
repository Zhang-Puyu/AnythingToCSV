using System.IO;
using System.Windows.Forms;

namespace Process.Application.Models
{
    public class ProScan3DData : AbstractProcesser
    {
        #region 单例模式
        private static ProScan3DData? instance = null;
        private static readonly object padlock = new object();
        public static ProScan3DData Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProScan3DData();
                    return instance;
                }
            }
        }
        #endregion

        private ProScan3DData() 
        {
            FileType = FileType.Scan3DData;
            FileFilter = "三维扫描数据|*.csv;";

            CanOneToMulti = false;
            CanOneToOne = true;
        }

        public static int Min(in int a, in int b, in int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        protected override void OneToOne(string dataFile, string csvFile)
        {
            using StreamReader reader = new StreamReader(dataFile);

            List<string> X = new List<string>();
            List<string> Y = new List<string>();
            List<string> Z = new List<string>();

            string? line;
            char[] back = ";/n/r".ToCharArray();
            while ((line = reader.ReadLine()) != null)
                if (line.Length > 0) X.AddRange(line.TrimEnd(back).Split(';'));
                else break;
            while ((line = reader.ReadLine()) != null)
                if (line.Length > 0) Y.AddRange(line.TrimEnd(back).Split(';'));
                else break;
            while ((line = reader.ReadLine()) != null)
                if (line.Length > 0) Z.AddRange(line.TrimEnd(back).Split(';'));
                else break;
            reader.Close();


            using StreamWriter writer = new StreamWriter(csvFile);
            writer.WriteLine("x,y,z");

            int l = Min(X.Count, Y.Count, Z.Count);
            for (int i = 0; i < l; i++) writer.WriteLine($"{X[i]},{Y[i]},{Z[i]}");

            writer.Close();

            MessageBox.Show($"读取到了{X.Count}个X，{Y.Count}个Y，{Z.Count}个Z");
        }
    }
}
