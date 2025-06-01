using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Convert.Methods.NC
{
    #region 数组扩展方法
    internal static class ArrayExtension
    {
        // 拷贝数组
        public static T[] Copy<T>(this T[] array)
        {
            //if (array == null) return null;
            T[] newArray = new T[array.Length];
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }

        // 判断两个数组是否所有元素都相等
        public static bool IsEqual<T>(this T[] array1, T[] array2)
        {
            //if (array1 == null || array2 == null) return false;
            //if (array1.Length != array2.Length) return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i])) return false;
            }
            return true;
        }
    }
    #endregion

    public abstract class ConvertNC : AbstractConverter
    {
        public override void SingleToSingle(string oriFile, string tarFile)
        {
            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                using (StreamWriter writer = new StreamWriter(tarFile, false, WriteEncoding))
                {
                    writer.WriteLine(Header);

                    string line = null;
                    float[] lastPoint = new float[Dimension];
                    while ((line = reader.ReadLine()) != null)
                    {
                        float[] currPoint = lastPoint.Copy();
                        ParaseLine(line, ref currPoint);
                        if (!currPoint.IsEqual(lastPoint))
                            writer.WriteLine(string.Join(",", currPoint));
                        lastPoint = currPoint.Copy();
                    }
                    writer.Close();
                }
                reader.Close();
            }
        }

        /// <summary>
        /// 表头
        /// </summary>
        protected abstract string Header { get; }
        /// <summary>
        /// 刀位点维度
        /// </summary>
        protected abstract int Dimension { get; }

        protected abstract void ParaseLine(in string line, ref float[] point);
    }
}
