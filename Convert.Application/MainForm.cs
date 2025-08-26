using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Convert.Methods;
using Microsoft.WindowsAPICodePack.Dialogs;
using MessageBox = System.Windows.MessageBox;

namespace Convert.Application
{
    /// <summary>
    /// 文件处理进度状态
    /// </summary>
    internal enum ProgessState
    {
        Waiting, Processing, Finished
    }

    /// <summary>
    /// 导出模式
    /// </summary>
    internal enum ExportModel
    {
        /// <summary>
        /// 导出为单个文件
        /// </summary>
        ToSingle,
        /// <summary>
        /// 分解导出为多个文件
        /// </summary>
        ToMulti
    }

    /// <summary>
    /// csv合并模式
    /// </summary>
    internal enum MergeModel
    {
        /// <summary>
        /// 合并行
        /// </summary>
        MergeRows,
        /// <summary>
        /// 合并列
        /// </summary>
        MergeColumns
    }


    public partial class MainForm : Form
    {
        private AbstractConverter converter = null;
        private AbstractConverter Converter
        {
            get => converter;
            set
            {
                converter = value;
                if (converter != null)
                {
                    converter.StartedEvent = ConvertStartedHandler;
                    converter.FinishedEvent = ConvertFinishedHandler;

                    converter.InfoMsgEvent =
                        msg => Debug.WriteLine(msg);
                    converter.ErrorMsgEvent =
                        msg => MessageBox.Show(msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

                    converter.ReadEncoding = ReadEncoding;
                    converter.WriteEncoding = WriteEncoding;
                }
            }
        }

        private FileType fileType;
        private FileType FileType 
        {
            get => fileType;
            set
            {
                if (fileType != value)
                {
                    _tableFiles.Rows.Clear();
                    fileType = value;
                }
                Converter = fileType.GetConverter();
            }
        }

        public MainForm()
        {
            InitializeComponent();

            #region <导入>菜单
            // 遍历FileType枚举类型, 将类型对应的导入选项添加到<导入>菜单
            foreach (FileType fileType in Enum.GetValues(typeof(FileType)))
            {
                if (fileType != FileType.MergeCsv)
                {
                    var itemImport = new ToolStripMenuItem(fileType.GetDescription());
                    itemImport.Click += ItemImport_Click;
                    itemImport.Tag = fileType;
                    _menuImport.DropDownItems.Add(itemImport);

                    // 添加提示信息
                    itemImport.ToolTipText = string.Join("/", fileType.GetFileExtensions());
                    itemImport.AutoToolTip = true;
                }
            }

            // 添加合并csv项
            var itemImportMergedCsv = new ToolStripMenuItem("合并csv");
            itemImportMergedCsv.Click += ItemImportMergedCsv_Click;
            itemImportMergedCsv.ForeColor = Color.Blue;
            _menuImport.DropDownItems.Add(itemImportMergedCsv);

            // 添加分隔符
            _menuImport.DropDownItems.Add(new ToolStripSeparator());
            // 添加清空菜单项
            var itemClear = new ToolStripMenuItem("清空");
            itemClear.Click += ItemClear_Click;
            _menuImport.DropDownItems.Add(itemClear);

            #endregion

            #region <导出>菜单
            _itemEachToSingle.Tag = ExportModel.ToSingle;
            _itemEachToMulti.Tag  = ExportModel.ToMulti;

            _itemMergeCsvColums.Tag = MergeModel.MergeColumns;
            _itemMergeCsvRows.Tag = MergeModel.MergeRows;
            #endregion

            #region <编码格式>菜单 
            // 列出所有编码格式的名称和代码页
            foreach (var encoding in AbstractConverter.SurpportedEncodings)
            {
                var itemReadEncoding = new ToolStripMenuItem(encoding.EncodingName);
                itemReadEncoding.Tag = encoding;
                itemReadEncoding.Click += ItemReadEncoding_Click;
                _itemReadEncoding.DropDownItems.Add(itemReadEncoding);
                itemReadEncoding.BackColor = 
                    encoding == ReadEncoding ? 
                    System.Drawing.SystemColors.GradientActiveCaption 
                    : System.Drawing.SystemColors.Control;

                var itemWriteEncoding = new ToolStripMenuItem(encoding.EncodingName);
                itemWriteEncoding.Tag = encoding;
                itemWriteEncoding.Click += ItemWriteEncoding_Click;
                _itemWriteEncoding.DropDownItems.Add(itemWriteEncoding);
                itemWriteEncoding.BackColor = 
                    encoding == WriteEncoding ? 
                    System.Drawing.SystemColors.GradientActiveCaption 
                    : System.Drawing.SystemColors.Control;
            }
            #endregion
        }

        #region 修改读/写编码格式
        private Encoding ReadEncoding = Encoding.GetEncoding("GBK");
        private Encoding WriteEncoding = Encoding.UTF8;

        private void ItemReadEncoding_Click(object sender, EventArgs e)
        {
            // 取消所有选项的高亮
            foreach (ToolStripMenuItem menuItem in _itemReadEncoding.DropDownItems)
                menuItem.BackColor = System.Drawing.SystemColors.Control;

            // 高亮该选项
            var item = sender as ToolStripMenuItem;
            ReadEncoding = item.Tag as Encoding;
            item.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
        }

        private void ItemWriteEncoding_Click(object sender, EventArgs e)
        {
            // 取消所有选项的高亮
            foreach (ToolStripMenuItem menuItem in _itemWriteEncoding.DropDownItems)
                menuItem.BackColor = System.Drawing.SystemColors.Control;

            // 高亮该选项
            var item = sender as ToolStripMenuItem;
            WriteEncoding = item.Tag as Encoding;
            item.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
        }
        #endregion

        #region 获取添加的文件
        private IEnumerable<string> Files => _tableFiles.Rows.Cast<DataGridViewRow>()
                    .Select(r => r.Cells[0].Value.ToString());
        #endregion

        #region 文件处理进度事件

        private void ConvertStartedHandler(string filePath)
        {
            for (int i = 0; i < _tableFiles.Rows.Count; ++i)
            {
                if (_tableFiles.Rows[i].Cells[0].Value.ToString() == filePath)
                {
                    BeginInvoke((Action)(() => {
                        _tableFiles.Rows[i].Cells[1].Value = ProgessState.Processing;
                        // 讲这一行背景色设置为黄色
                        _tableFiles.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    }));
                    return;
                }
            }
        }

        private void ConvertFinishedHandler(string filePath)
        {
            for (int i = 0; i < _tableFiles.Rows.Count; ++i)
            {
                if (_tableFiles.Rows[i].Cells[0].Value.ToString() == filePath)
                {
                    BeginInvoke((Action)(() => {
                        _tableFiles.Rows[i].Cells[1].Value = ProgessState.Finished;
                        // 讲这一行背景色设置为绿色
                        _tableFiles.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }));
                    return;
                }
            }
        }

        #endregion

        #region 导入/导出菜单按钮点击事件
        private void ItemImport_Click(object sender, EventArgs e)
        {
            FileType CurrFileType = (FileType)(sender as ToolStripMenuItem).Tag;

            var dialog = CurrFileType.OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileType = CurrFileType;

                foreach (string file in dialog.FileNames)
                    _tableFiles.Rows.Add(new object[] { file, ProgessState.Waiting });
            }
        }

        private void ItemExport_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = "请选择要保存路径",
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Task task = Task.Run(() =>
                {
                    switch ((ExportModel)(sender as ToolStripMenuItem).Tag)
                    {
                        case ExportModel.ToSingle:
                            task = Converter.EachToEach(Files, dialog.FileName);
                            break;
                        case ExportModel.ToMulti:
                            task = Converter.EachToMulti(Files, dialog.FileName);
                            break;
                    }
                });

                if (task != null)
                {
                    _menuStrip.Enabled = false;
                    task.ContinueWith(t =>
                    {
                        _menuStrip.Enabled = true;
                        // 打开所在文件夹
                        Process.Start("explorer.exe", dialog.FileName);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void ItemClear_Click(object sender, EventArgs e)
        {
            _tableFiles.Rows.Clear();

            Converter = null;
        }

        #endregion

        #region DataGridView自动添加行号

        /// <summary>
        /// DataGridView自动添加行号
        /// </summary>
        private void TableFiles_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIndex = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBunds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIndex, new Font("Microsoft YaHei UI", 9), 
                SystemBrushes.ControlText, headerBunds, centerFormat);
        }

        #endregion

        #region 合并csv
        private void ItemImportMergedCsv_Click(object sender, EventArgs e)
        {
            FileType = FileType.MergeCsv;
            var dialog = new OpenFileDialog()
            {
                Title = "请选择要添加的数据文件",
                Filter = FileType.GetFileFilter(),
                Multiselect = true,
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                    _tableFiles.Rows.Add(new object[] { file, ProgessState.Waiting });
            }
        }

        private void ItemExportMergedCsv_Click(object sender, EventArgs e)
        {
            var dialog = fileType.SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 弹出一个对话框, 询问是否跳过第一行
                bool skipFirstRow = 
                    MessageBox.Show("文件第一行是否为表头?", 
                    "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

                string tarFile = dialog.FileName.RenameIfFileExists();

                Task task = Task.Run(() => 
                {
                    switch ((MergeModel)(sender as ToolStripMenuItem).Tag)
                    {
                        case MergeModel.MergeColumns:
                            (Converter as ConvertMergeCSV).MergeCsvAsColumn(Files, dialog.FileName, skipFirstRow);
                            break;
                        case MergeModel.MergeRows:
                            (Converter as ConvertMergeCSV).MergeCsvAsRow(Files, dialog.FileName, skipFirstRow);
                            break;
                    }
                });

                // 将所有文件的状态设置为处理中
                for (int i = 0; i < _tableFiles.Rows.Count; ++i)
                {
                    _tableFiles.Rows[i].Cells[1].Value = ProgessState.Processing;
                    _tableFiles.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
                // 禁用菜单
                _menuStrip.Enabled = false;
                // 处理完成后的操作
                task.ContinueWith(t =>
                {
                    // 启用菜单
                    _menuStrip.Enabled = true;
                    // 将所有文件的状态设置为已完成
                    for (int i = 0; i < _tableFiles.Rows.Count; ++i)
                    {
                        _tableFiles.Rows[i].Cells[1].Value = ProgessState.Finished;
                        _tableFiles.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                    }
                    // 打开所在文件夹
                    Process.Start("explorer.exe", tarFile);
                }, TaskScheduler.FromCurrentSynchronizationContext()); 
            }
        }
        #endregion

        #region 增/删行事件
        private void RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_tableFiles.Rows.Count > 0)
            {
                _itemEachToMulti.Enabled = Converter.CanSingleToMulti;
                _itemEachToSingle.Enabled = Converter.CanSingleToSingle;

                _itemMergeCsvRows.Enabled = Converter is ConvertMergeCSV;
                _itemMergeCsvColums.Enabled = Converter is ConvertMergeCSV;
            }
        }

        private void RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_tableFiles.Rows.Count == 0)
            {
                _itemEachToMulti.Enabled = false;
                _itemEachToSingle.Enabled = false;

                _itemMergeCsvRows.Enabled = false;
                _itemMergeCsvColums.Enabled = false;
            }
        }
        #endregion
    }
}
