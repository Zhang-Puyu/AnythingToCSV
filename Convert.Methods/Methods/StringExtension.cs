using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convert.Methods
{
    public static class StringExtension
    {
        /// <summary>
        /// 若文件已存在，则重命名
        /// </summary>
        /// <param name="oriFile">原文件</param>
        /// <returns>文件新名称</returns>
        public static string RenameIfFileExists(this string oriFile)
        {
            // 若文件已存在
            if (File.Exists(oriFile))
            {
                string fileName = Path.GetFileNameWithoutExtension(oriFile);
                string ext = Path.GetExtension(oriFile);
                string folder = Path.GetDirectoryName(oriFile);
                int i = 1;
                string newFile = Path.Combine(folder, $"{fileName}({i}){ext}");
                while (File.Exists(newFile))
                {
                    i++;
                    newFile = Path.Combine(folder, $"{fileName}({i}){ext}");
                }
                return newFile;
            }
            return oriFile;
        }

        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        public static bool ContainsChinese(this string str)
        {
            foreach (char c in str)
                if (c >= 0x4e00 && c <= 0x9fbb)
                    return true;
            return false;
        }
    }
}
