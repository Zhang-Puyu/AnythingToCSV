using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Convert.NC;
using NCToolBox.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertGCode : ConvertNC
    {
        #region 单例模式
        private static ConvertGCode instance = null;
        private static readonly object padlock = new object();
        public static ConvertGCode Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertGCode();
                    return instance;
                }
            }
        }
        private ConvertGCode() { }
        #endregion

        private readonly NCToolBox.Toolpath.GCode.ReadGCode reader 
            = new NCToolBox.Toolpath.GCode.ReadGCode();
        protected override AbsractReader Reader => reader;
    }
}
