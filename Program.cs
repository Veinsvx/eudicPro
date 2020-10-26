using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oulupor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex m = new Mutex(false, "Product_Index_Cntvs", out bool bCreatedNew);
            if(bCreatedNew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("应用程序已经在运行中...");
                System.Threading.Thread.Sleep(600);
                //  终止此进程并为基础操作系统提供指定的退出代码。
                System.Environment.Exit(1);
            }

        }
    }
}
