using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Process.Application.Models;

namespace Process.Application.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportEachToEachCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportEachToMultiCommand))]
        private ObservableCollection<string> filePathList = new ObservableCollection<string>();
        [ObservableProperty]
        private FileType fileType = FileType.Undefine;

        private AbstractProcesser? Processer = null;

        #region 选择文件

        private void ChooseFile()
        {
            if (Processer != null)
            {
                if (FileType != Processer.FileType)
                {
                    FilePathList.Clear();
                    FileType = Processer.FileType;
                }
                var dialog = Processer.NewOpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in dialog.FileNames)
                    {
                        FilePathList.Add(file);
                    }
                }
            }
        }

        [RelayCommand]
        private void ChooseSlk()
        {
            Processer = ProSlk.Instance;
            ChooseFile();
        }

        [RelayCommand]
        private void ChooseNCCode()
        {
            Processer = ProNCCode.Instance;
            ChooseFile();
        }
        [RelayCommand]
        private void ChooseMonNCCode()
        {
            Processer = ProMonNCCode.Instance;
            ChooseFile();
        }
        [RelayCommand]
        private void ChooseJDReport()
        {
            Processer = ProJDReport.Instance;
            ChooseFile();
        }
        [RelayCommand]
        private void ChooseCMMRxport()
        {
            Processer = ProCMMReport.Instance;
            ChooseFile();
        }
        [RelayCommand]
        private void ChooseScan3DData()
        {
            Processer = ProScan3DData.Instance;
            ChooseFile();
        }
        [RelayCommand]
        private void ChooseDWData()
        {
            Processer = ProcessDWData.Instance;
            ChooseFile();
        }

        #endregion

        #region 清空文件    

        [RelayCommand]
        private void Clear()
        {
            FilePathList.Clear();
            FileType = FileType.Undefine;
            Processer = null;
        }

        #endregion

        #region 执行转换

        private bool CanExport => FilePathList.Count > 0;

        [RelayCommand(CanExecute = nameof(CanExport))]
        private void ExportEachToEach()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Processer?.EachOneToEachOne(FilePathList, dialog.SelectedPath);
                System.Diagnostics.Process.Start("Explorer.exe", dialog.SelectedPath);
                MessageBox.Show("转换结束");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private void ExportEachToMulti()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Processer?.EachOneToEachMulti(FilePathList, dialog.SelectedPath);
                System.Diagnostics.Process.Start("Explorer.exe", dialog.SelectedPath);
                MessageBox.Show("转换结束");
            }   
        }

        #endregion
    }
}
