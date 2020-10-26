using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace oulupor
{
    public partial class Form1 : Form
    {
        private static readonly int WM_MOUSEMOVE = 512;

        #region 需要使用的api
        [DllImport("user32.dll", EntryPoint = "FindWindow")] private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindow")] private static extern int FindWindow(string lpszClass, string lpszWindow,int a);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]private static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")] private static extern int GetWindowRect(int hwnd, ref System.Drawing.Rectangle lpRect);
        [DllImport("user32.dll", EntryPoint = "SendMessage")] private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            timer1.Enabled =true;
            timer1.Interval = 600;
            //Process.Start("eudic.exe");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IntPtr ParenthWnd = new IntPtr(0);
            ParenthWnd = FindWindow(null, "软件激活");
            //判断这个窗体是否有效
            if (ParenthWnd != IntPtr.Zero)
            {
                textBox1.Text+="找到窗口";
                Process[] psEaiNotes = Process.GetProcessesByName("eudic");
                foreach (Process psEaiNote in psEaiNotes)
                {
                    psEaiNote.Kill();
                    XiuGaiZhuCeBiao();
                    Process.Start("eudic.exe");
                    RefreshTaskbarIcon();
                    textBox1.Text += String.Format("刷新任务栏成功！\r\n");
                }
            }
            else
            {
                //textBox1.Text += "没有找到窗口\r\n";
            }
        }

        /// <summary>
        /// 修改注册表信息
        /// </summary>
        void XiuGaiZhuCeBiao()
        {
            //不管是创建、获取、删除键值，首先都需要打开要设置/创建键值的注册表项
            RegistryKey key = Registry.CurrentUser;
            //该键值须存在
            RegistryKey software = key.OpenSubKey(@"Software\Francophonie\Eudic\Customer Info", true);
            textBox1.Text += String.Format("注册表数值失效为{0}", software.GetValue("TimesLeft3"));
            software.SetValue("TimesLeft3", 0x000c85e7, RegistryValueKind.DWord);
            textBox1.Text += String.Format("注册表数值现在修改为{0}!", software.GetValue("TimesLeft3"));
        }

        /// <summary>
        /// 刷新任务栏
        /// </summary>
        public static void RefreshTaskbarIcon()
        {
            //任务栏窗口
            int one = FindWindow("Shell_TrayWnd", null,0);
            //任务栏右边托盘图标+时间区
            int two = FindWindowEx(one, 0, "TrayNotifyWnd", null);
            //不同系统可能有可能没有这层
            int three = FindWindowEx(two, 0, "SysPager", null);
            //托盘图标窗口
            int foor;
            if (three >0)
            {
                foor = FindWindowEx(three, 0, "ToolbarWindow32", null);
            }
            else
            {
                foor = FindWindowEx(two, 0, "ToolbarWindow32", null);
            }
            if (foor >0)
            {
                System.Drawing.Rectangle r = new System.Drawing.Rectangle();
                GetWindowRect(foor, ref r);
                //从任务栏左上角从左到右 MOUSEMOVE一遍，所有图标状态会被更新
                for (int x =0 ; x < (r.Right - r.Left) - r.X; x++)
                {
                    SendMessage(foor, WM_MOUSEMOVE,0 , ( 1<<16 ) | x);
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }
    }
}
