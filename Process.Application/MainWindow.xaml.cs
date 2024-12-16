using System.Text;
using System.Windows;
using System.IO;
using Process.Application.ViewModels;
using System.CodeDom;

namespace Process.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string? dropFile;
            List<string> files = new List<string>();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                int count = ((Array)e.Data.GetData(DataFormats.FileDrop)).Length;

                for (int i = 0; i < count; i++)
                {
                    dropFile = (e.Data.GetData(DataFormats.FileDrop) as Array)?.GetValue(i)?.ToString();
                    if (dropFile != null && Path.GetExtension(dropFile).ToLower() == ".slk")
                    {
                        files.Add(dropFile);
                    }
                }
            }
        }
    }
}