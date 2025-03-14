using System;
using System.Collections.Generic;
using System.IO;

namespace Convert.Methods
{
    /// <summary>
    /// scanCONTROL 3D扫描数据处理器
    /// </summary>
    public class ConvertScan3DData : AbstractConverter
    {
        #region 单例模式
        private static ConvertScan3DData instance = null;
        private static readonly object padlock = new object();
        public static ConvertScan3DData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertScan3DData();
                    return instance;
                }
            }
        }

        private ConvertScan3DData() { }
        #endregion

        internal static int Min(in int a, in int b, in int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        public override void SingleToSingle(string oriFile, string tarFile)
        {
            List<string> X = new List<string>();
            List<string> Y = new List<string>();
            List<string> Z = new List<string>();

            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                string line = null;
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
            }

            using (StreamWriter writer = new StreamWriter(tarFile, false, WriteEncoding))
            {
                writer.WriteLine("x,y,z");
                int l = Min(X.Count, Y.Count, Z.Count);
                for (int i = 0; i < l; i++) 
                    writer.WriteLine($"{X[i]},{Y[i]},{Z[i]}");

                writer.Close();
            }

            InfoMsgEvent?.Invoke($"在{Path.GetFileName(oriFile)}中读取到了{X.Count}个X，{Y.Count}个Y，{Z.Count}个Z");
        }
    }
}
