using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Processor.Methods;

namespace Processor.Command
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileType     fileType = FileType.Unsurpport;
            List<string> files    = new List<string>();

            // 获取文件类型
            foreach (string file in args)
            {
                if (file.GetFileType() != FileType.Unsurpport)
                {
                    fileType = file.GetFileType();
                    break;
                }
            }

            if (fileType != FileType.Unsurpport)
            {
                // 筛选出同类型的文件
                foreach (string file in args)
                    if (file.GetFileType() == fileType)
                        files.Add(file);

                // 根据文件类型选择处理器
                var Processor = fileType.ChooseProcesser();
                string folder = Path.GetDirectoryName(files.First());

                // 异步处理文件
                Processor.EachToEach(files, folder).Wait();

                // 在处理完成后打印处理的文件名
                Console.WriteLine($"处理完成：{string.Join("\n", files)}");
            }
        }
    }
}
