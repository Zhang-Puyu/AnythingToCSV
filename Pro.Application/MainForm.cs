using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Processor.Methods;
using MessageBox = System.Windows.MessageBox;

namespace Processor.Application
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

    public partial class MainForm : Form
    {
        private AbstractProcessor   Processor    { get; set; } = null;
        private FileType            LastFileType { get; set; } = FileType.Unsurpport;

        public MainForm()
        {
            InitializeComponent();

            #region <导入>菜单
            // 遍历FileType枚举类型, 将类型对应的导入选项添加到<导入>菜单
            foreach (FileType fileType in Enum.GetValues(typeof(FileType)))
            {
                if (fileType.ChooseProcesser() != null)
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
            #endregion

            #region <注册>菜单
            foreach (FileType fileType in Enum.GetValues(typeof(FileType)))
            {
                if (fileType!= FileType.Unsurpport)
                {
                    var itemRegister = new ToolStripMenuItem(fileType.GetDescription());
                    var itemUnregister = new ToolStripMenuItem(fileType.GetDescription());
                    itemRegister.Click += ItemRegister_Click;
                    itemUnregister.Click += ItemUnregister_Click;
                    itemRegister.Tag = fileType;
                    itemUnregister.Tag = fileType;

                    // 添加提示信息
                    itemRegister.ToolTipText = string.Join("/", fileType.GetFileExtensions());
                    itemRegister.AutoToolTip = true;
                    itemUnregister.ToolTipText = string.Join("/", fileType.GetFileExtensions());
                    itemUnregister.AutoToolTip = true;

                    _itemRegister.DropDownItems.Add(itemRegister);
                    _itemUnregister.DropDownItems.Add(itemUnregister);
                }
            }

            _itemRegister.DropDownItems.Add(new ToolStripSeparator());
            _itemUnregister.DropDownItems.Add(new ToolStripSeparator());

            var itemRegisterAny = new ToolStripMenuItem("注册到任意文件类型的右键菜单");
            itemRegisterAny.Click += ItemRegister_Click;
            _itemRegister.DropDownItems.Add(itemRegisterAny);

            var itemUnregisterAny = new ToolStripMenuItem("注册到任意文件类型的右键菜单");
            itemUnregisterAny.Click += ItemUnregister_Click;
            _itemUnregister.DropDownItems.Add(itemUnregisterAny);

            Register.InfoMsgEvent += msg => MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            Register.ErrorMsgEvent += msg => MessageBox.Show(msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

            #endregion
        }

        #region 文件处理进度事件

        private void ProcessStartedHandler(string filePath)
        {
            for (int i = 0; i < _tableFiles.Rows.Count; ++i)
            {
                if (_tableFiles.Rows[i].Cells[0].Value.ToString() == filePath)
                {
                    BeginInvoke((Action)(() => _tableFiles.Rows[i].Cells[1].Value = ProgessState.Processing));
                    return;
                }
            }
        }

        private void ProcessFinishedHandler(string filePath)
        {
            for (int i = 0; i < _tableFiles.Rows.Count; ++i)
            {
                if (_tableFiles.Rows[i].Cells[0].Value.ToString() == filePath)
                {
                    BeginInvoke((Action)(() => _tableFiles.Rows[i].Cells[1].Value = ProgessState.Finished));
                    return;
                }
            }
        }

        #endregion

        #region 添加未知类型文件
        public void AddUnknownTypeFiles(in IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (file.GetFileType() != FileType.Unsurpport)
                {
                    FileType CurrFileType = file.GetFileType();
                    if (LastFileType != CurrFileType)
                    {
                        _tableFiles.Rows.Clear();

                        Processor = CurrFileType.ChooseProcesser();

                        Processor.ProcessStartedEvent = ProcessStartedHandler;
                        Processor.ProcessFinishedEvent = ProcessFinishedHandler;

                        Processor.InfoMsgEvent = 
                            msg => Debug.WriteLine(msg);
                        Processor.ErrorMsgEvent = 
                            msg => MessageBox.Show(msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

                        _itemEachToMulti.Enabled = Processor.CanSingleToMulti;
                        _itemEachToSingle.Enabled = Processor.CanSingleToSingle;

                        LastFileType = CurrFileType;
                    }
                    break;
                }
            }
            if (LastFileType != FileType.Unsurpport)
                foreach (string file in files)
                {
                    if (file.GetFileType() == LastFileType)
                        _tableFiles.Rows.Add(new object[] { file, ProgessState.Waiting });
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
                if (LastFileType != CurrFileType)
                {
                    _tableFiles.Rows.Clear();

                    Processor = CurrFileType.ChooseProcesser();

                    Processor.ProcessStartedEvent  = ProcessStartedHandler;
                    Processor.ProcessFinishedEvent = ProcessFinishedHandler;

                    Processor.InfoMsgEvent = msg => Debug.WriteLine(msg);
                    Processor.ErrorMsgEvent = msg => MessageBox.Show(msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

                    _itemEachToMulti.Enabled = Processor.CanSingleToMulti;
                    _itemEachToSingle.Enabled = Processor.CanSingleToSingle;

                    LastFileType = CurrFileType;
                }

                foreach (string file in dialog.FileNames)
                {
                    _tableFiles.Rows.Add(new object[] { file, ProgessState.Waiting });
                }
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
                Task task = null;
                IEnumerable<string> files 
                    = _tableFiles.Rows.Cast<DataGridViewRow>()
                    .Select(r => r.Cells[0].Value.ToString());

                switch ((ExportModel)(sender as ToolStripMenuItem).Tag)
                {
                    case ExportModel.ToSingle:
                        task = Processor.EachToEach(files, dialog.FileName);
                        break;
                    case ExportModel.ToMulti:
                        task = Processor.EachToMulti(files, dialog.FileName);
                        break;
                }

                if (task != null)
                {
                    _menuImport.Enabled = false;
                    _menuExport.Enabled = false;

                    task.ContinueWith(t =>
                    {
                        _menuImport.Enabled = true;
                        _menuExport.Enabled = true;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void ItemClear_Click(object sender, EventArgs e)
        {
            _tableFiles.Rows.Clear();
            Processor = null;
        }

        #endregion

        #region 拖入文件

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                AddUnknownTypeFiles((string[])e.Data.GetData(DataFormats.FileDrop)); 
        }

        #endregion

        #region DataGridView显示设置

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

        #region 将程序注册到右键菜单
        /// <summary>
        /// 注册表项名称
        /// </summary>
        private static readonly string KeyName = "ProcesseToCsv";
        /// <summary>
        /// 右键菜单中命令名称
        /// </summary>
        private static readonly string CmdName = "转换为csv";
        /// <summary>
        /// 程序路径
        /// </summary>
        private static string ExePath => System.Windows.Forms.Application.ExecutablePath;
        /// <summary>
        /// 尝试获取管理员权限
        /// </summary>
        private void TryGetAdminAuthority()
        {
            if (!Register.HasAdminAuthority())
            {
                MessageBox.Show("需要管理员权限，请软件重启后重新操作", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // 重新启动应用程序并请求管理员权限
                var processInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = System.Windows.Forms.Application.ExecutablePath,
                    Verb = "runas" // 请求管理员权限
                };
                try
                {
                    System.Diagnostics.Process.Start(processInfo);
                }
                catch (Exception) // 用户拒绝了管理员权限请求
                {
                    MessageBox.Show("注册表修改失败，无管理员权限", "错误", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                System.Windows.Forms.Application.Exit();
            }
        }

        /// <summary>
        /// 将程序注册到右键菜单(需要管理员权限)
        /// </summary>
        private void ItemRegister_Click(object sender, EventArgs e)
        {
            TryGetAdminAuthority();

            object tag = (FileType)((sender as ToolStripMenuItem).Tag);
            if (tag != null)
                foreach (string extension in ((FileType)tag).GetFileExtensions())
                    Register.AddToContextMenu(KeyName, CmdName, ExePath, extension);
            else
                Register.AddToContextMenu(KeyName, CmdName, ExePath);
        }

        /// <summary>
        /// 从右键菜单移除(需要管理员权限)
        /// </summary>
        private void ItemUnregister_Click(object sender, EventArgs e)
        {
            TryGetAdminAuthority();

            object tag = (sender as ToolStripMenuItem).Tag;
            if (tag != null)
                foreach (string extension in ((FileType)tag).GetFileExtensions())
                    Register.RemoveFromContextMenu(KeyName, extension);
            else
                Register.RemoveFromContextMenu(KeyName);
        }
        #endregion
    }
}
