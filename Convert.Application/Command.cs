using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Convert.Methods;

namespace Convert.Application
{
    internal static partial class Program
    {
        public static void ConvertUnknownTypeFiles(in IEnumerable<string> args)
        {
            FileType fileType = FileType.Unsurpport;
            List<string> files = new List<string>();

            // 获取文件类型
            foreach (string file in args)
            {
                if (file.FindFileType() != FileType.Unsurpport)
                {
                    fileType = file.FindFileType();
                    break;
                }
            }

            if (fileType != FileType.Unsurpport)
            {
                // 筛选出同类型的文件
                foreach (string file in args)
                    if (file.FindFileType() == fileType)
                        files.Add(file);

                // 根据文件类型选择处理器
                var converter = fileType.GetConverter();
                string folder = Path.GetDirectoryName(files.First());

                // 异步处理文件
                converter.EachToEach(files, folder).Wait();

                // 在处理完成后打印处理的文件名
                MessageBox.Show($"处理完成：\n{string.Join("\n", files.Select(path => Path.GetFileName(path)))}",
                    "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
