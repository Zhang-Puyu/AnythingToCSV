using System;
using System.Security.Principal;
using Microsoft.Win32;

namespace Processor.Methods
{
    public class Register
    {
        /// <summary>
        /// 判断当前程序是否以管理员权限运行
        /// </summary>
        public static bool IsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 将应用程序注册到右键菜单
        /// </summary>
        /// <param name="regKeyName">注册表项名称</param>
        /// <param name="cmdName">右键菜单命令名称</param>
        /// <param name="exePath">应用程序路径</param>
        /// <param name="fileExtension">文件扩展名</param>
        public static void RegisterToContextMenu(string regKeyName, string cmdName, string exePath, string fileExtension = "*")
        {
            try
            {
                // 创建注册表项
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"{fileExtension}\\shell\\{regKeyName}"))
                {
                    if (key != null)
                    {
                        key.SetValue("", cmdName);
                    }
                }

                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"{fileExtension}\\shell\\{regKeyName}\\command"))
                {
                    if (key != null)
                    {
                        key.SetValue("", $"\"{exePath}\" \"%1\"");
                    }
                }
                Console.WriteLine("已成功添加到右键菜单。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加到右键菜单时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 从右键菜单中移除应用程序
        /// </summary>
        /// <param name="regKeyName">注册表项名称</param>
        /// <param name="fileExtension">文件扩展名</param>
        public static void RemoveFromContextMenu(string regKeyName, string fileExtension = "*")
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree($"{fileExtension}\\shell\\{regKeyName}");
                Console.WriteLine("已成功从右键菜单中移除。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"移除右键菜单时出错: {ex.Message}");
            }
        }
    }
}
