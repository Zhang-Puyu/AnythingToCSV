using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Process.Application.Models
{
    public enum FileType
    {
        Slk, Csv, JDReport, CMMReport, MonNCCode, NCCode, Scan3DData, DWData, Undefine
    }

    public abstract class AbstractProcesser
    {
        public FileType FileType { get; protected set; } = FileType.Undefine;
        public string FileFilter { get; protected set; } = string.Empty;

        public OpenFileDialog NewOpenFileDialog() => new OpenFileDialog()
        {
            Title = "请选择要添加的数据文件",
            Multiselect = CanMultiSelect,
            Filter = this.FileFilter
        };

        // 能分解的文件就不能多选
        public bool CanMultiSelect => !CanOneToMulti;

        /// <summary>
        /// 单个文件能否输出为多个csv
        /// </summary>
        public bool CanOneToMulti { get; protected set; } = true;
        /// <summary>
        /// 单个文件能否输出为一个csv
        /// </summary>
        public bool CanOneToOne { get; protected set; } = false;

        public static void MergeCsv(in IEnumerable<string> oriFiles, string tarFile, bool hasHead = false)
        {
            using StreamWriter writer = new StreamWriter(tarFile);

            if (hasHead)
            {
                using StreamReader reader = new StreamReader(oriFiles.First());
                writer.WriteLine(reader.ReadLine());
                reader.Close();
            }

            string? line = null;
            foreach(string file in oriFiles)
            {
                using StreamReader reader = new StreamReader(file);
                if (hasHead) reader.ReadLine();
                while((line = reader.ReadLine()) != null)
                    writer.WriteLine(line);
                reader.Close();
            }

            writer.Close();
        }

        protected virtual void OneToOne(string dataFile, string csvFile)
        {
            MessageBox.Show("这功能还没做，自己合并csv吧");
        }
        protected virtual void OneToMulti(string dataFile, string folder)
        {
            MessageBox.Show("这功能还没做，自己拆分csv吧");
        }

        public virtual void EachOneToEachOne(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, file =>
            {
                string tarFile = Path.Combine(tarFolder, Path.GetFileNameWithoutExtension(file) + ".csv");
                OneToOne(file, tarFile);
            });
        }
        public virtual void EachOneToEachMulti(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, file =>
            {
                OneToMulti(file, tarFolder);
            });
        }
    }
}
