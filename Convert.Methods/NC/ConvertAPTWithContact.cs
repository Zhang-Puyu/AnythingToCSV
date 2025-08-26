using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Convert.NC;
using NCToolBox.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertAPTWithContact : ConvertNC
    {
        #region 单例模式
        private static ConvertAPTWithContact instance = null;
        protected static readonly object padlock = new object();
        public static ConvertAPTWithContact Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertAPTWithContact();
                    return instance;
                }
            }
        }

        private ConvertAPTWithContact() { }
        #endregion

        private readonly NCToolBox.Toolpath.APT.ReadAPTWithContact reader 
            = new NCToolBox.Toolpath.APT.ReadAPTWithContact();
        protected override AbsractReader Reader => reader;
    }
}
