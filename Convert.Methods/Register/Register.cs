using System;
using System.Security.Principal;
using Microsoft.Win32;

namespace Convert.Methods
{
    public class Register
    {
        # region 判断当前程序是否以管理员权限运行
        /// <summary>
        /// 判断当前程序是否以管理员权限运行
        /// </summary>
        public static bool HasAdminAuthority()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion

        #region 将应用程序注册到任意文件类型的右键菜单
        /// <summary>
        /// 将应用程序注册到右键菜单
        /// </summary>
        /// <param name="regKeyName">注册表项名称</param>
        /// <param name="cmdName">右键菜单命令名称</param>
        /// <param name="exePath">应用程序路径</param>
        /// <param name="fileExtension">文件扩展名</param>
        public static void AddToContextMenu(string regKeyName, string cmdName, string exePath)
        {
            try
            {
                // 创建注册表项
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"*\\shell\\{regKeyName}"))
                {
                    // 设置右键菜单项名称
                    key?.SetValue("", cmdName);
                    // 设置右键菜单项图标
                    key?.SetValue("Icon", $"{exePath},0");
                }
                // 设置执行的命令
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"*\\shell\\{regKeyName}\\command"))
                {
                    key?.SetValue("", $"\"{exePath}\" \"%1\"");
                }
                InfoMsgEvent?.Invoke("已成功添加到右键菜单。");
            }
            catch (Exception ex)
            {
                ErrorMsgEvent?.Invoke($"添加到右键菜单时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 从右键菜单中移除应用程序
        /// </summary>
        /// <param name="regKeyName">注册表项名称</param>
        /// <param name="fileExtension">文件扩展名</param>
        public static void RemoveFromContextMenu(string regKeyName)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree($"*\\shell\\{regKeyName}");
                InfoMsgEvent?.Invoke("已成功从右键菜单中移除。");
            }
            catch (Exception ex)
            {
                ErrorMsgEvent?.Invoke($"移除右键菜单时出错: {ex.Message}");
            }
        }
        #endregion

        /// <summary>
        /// 错误消息事件
        /// </summary>
        public static Action<string> ErrorMsgEvent;
        /// <summary>
        /// 信息消息事件
        /// </summary>
        public static Action<string> InfoMsgEvent;

        #region 将应用程序注册到指定文件类型的右键菜单
        public static void AddToContextMenu(string regKeyName, string cmdName, string exePath, string fileExtention)
        {
            try
            {
                // 创建注册表项
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"SystemFileAssociations\\{fileExtention}\\shell\\{regKeyName}"))
                {
                    // 设置右键菜单项名称
                    key?.SetValue("", cmdName);
                    // 设置右键菜单项图标
                    key?.SetValue("Icon", $"{exePath},0");
                }
                // 设置执行的命令
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"SystemFileAssociations\\{fileExtention}\\shell\\{regKeyName}\\command"))
                {
                    key?.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                InfoMsgEvent?.Invoke("已成功添加到右键菜单。");
            }
            catch (Exception ex)
            {
                ErrorMsgEvent?.Invoke($"添加到右键菜单时出错: {ex.Message}");
            }
        }

        public static void RemoveFromContextMenu(string regKeyName, string fileExtention)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree($"SystemFileAssociations\\{fileExtention}\\shell\\{regKeyName}");
                InfoMsgEvent?.Invoke("已成功从右键菜单中移除。");
            }
            catch (Exception ex)
            {
                ErrorMsgEvent?.Invoke($"移除右键菜单时出错: {ex.Message}");
            }
        }
        #endregion
    }
}
