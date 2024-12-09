using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;


namespace Process.Application.Models
{
    public class ProSlk : AbstractProcesser
    {
        #region 单例模式
        private static ProSlk? instance = null;
        private static readonly object padlock = new object();
        public static ProSlk Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProSlk();
                    return instance;
                }
            }
        }
        #endregion

        private ProSlk() 
        {
            FileType = FileType.Slk;
            FileFilter = "slk矩阵文件|*.slk;";

            CanOneToMulti = false;
            CanOneToOne = true;
        }

        protected override void OneToOne(string dataFile, string csvFile)
        {
            // 创建Excel应用程序对象
            Debug.WriteLine("Init Excel");
            Excel.Application excelApp = new Excel.Application
            {
                Visible = false // 可以设置为true以显示Excel应用程序界面，方便调试
            };

            try
            {
                // 打开SLK文件
                Debug.WriteLine("Open .slk file");
                Excel.Workbook workbook = excelApp.Workbooks.Open(dataFile);

                // 将SLK文件另存为CSV格式
                workbook.SaveAs(csvFile, Excel.XlFileFormat.xlCSV,
                    Type.Missing, Type.Missing,
                    false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlLocalSessionChanges,
                    Type.Missing, Type.Missing);

                // 关闭工作簿和Excel应用程序
                workbook.Close(false, Type.Missing, Type.Missing);
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                // 释放Excel对象
                if (excelApp != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }
    }
}
