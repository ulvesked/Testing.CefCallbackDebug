using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;

namespace CefCallbackDebug
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser browser;
        public System.Threading.Timer durationTimer;
        private int switchInterval = 5000;
        private WidgetControl widgetControl;

        public void WriteLog(string msg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.textBox1.AppendText(msg + "\r\n");
            }));
            System.Diagnostics.Debug.WriteLine(msg);
        }
        public Form1()
        {
            InitializeComponent();
            var cefSettings = new CefSettings();
            cefSettings.LogSeverity = LogSeverity.Verbose;
            cefSettings.RemoteDebuggingPort = 8080;
            Cef.Initialize(cefSettings);
            browser = new ChromiumWebBrowser("about:blank");
            widgetControl = new WidgetControl(this);
            browser.RegisterJsObject("widgetControl", widgetControl);
            this.splitContainer1.Panel1.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            durationTimer = new System.Threading.Timer(_ => this.Invoke(new MethodInvoker(()=>SwitchCefSource())), null, switchInterval, -1);
        }

        private bool isBlank = true;
        private string widgetSrc = Path.Combine(Environment.CurrentDirectory, "test.html");
        private void SwitchCefSource()
        {
            if (isBlank)
            {
                WriteLog(String.Format("Loading {0}", widgetSrc));
                browser.Load(widgetSrc);
                isBlank = false;
            }
            else
            {
                if (widgetControl.Completed != true)
                {
                    WriteLog("ERROR: Not completed!");
                }
                WriteLog(String.Format("Loading {0}", "about:blank"));
                browser.Load("about:blank");
                isBlank = true;
            }
            durationTimer.Change(switchInterval, -1);
        }
        
        public async Task<int> RandomDelay()
        {
            var ms = new Random().Next(2000, 15000);
            WriteLog(String.Format("RandomDelay: {0}", ms));
            await Task.Delay(ms);
            return ms;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            durationTimer.Change(-1, -1);
            Cef.Shutdown();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            WriteLog("Initialized");
            WriteLog("Remote debugging on http://localhost:8080/");
        }
    }
}
