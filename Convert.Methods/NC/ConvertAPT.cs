using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Convert.NC;
using NCToolBox.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertAPT : ConvertNC
    {
        #region 单例模式
        private static ConvertAPT instance = null;
        protected static readonly object padlock = new object();
        public static ConvertAPT Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertAPT();
                    return instance;
                }
            }
        }

        protected ConvertAPT() { }
        #endregion

        private readonly NCToolBox.Toolpath.APT.ReadAPT reader 
            = new NCToolBox.Toolpath.APT.ReadAPT();
        protected override AbsractReader Reader => reader;
    }
}
