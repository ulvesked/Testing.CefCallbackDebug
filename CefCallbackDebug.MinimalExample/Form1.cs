using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;

namespace CefCallbackDebug.MinimalExample
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser browser;
        private System.Threading.Timer timer;
        public Form1()
        {
            InitializeComponent();
            var cefSettings = new CefSettings();
            cefSettings.LogSeverity = LogSeverity.Verbose;
            cefSettings.RemoteDebuggingPort = 8080;
            Cef.Initialize(cefSettings);
            browser = new ChromiumWebBrowser(Path.Combine(Environment.CurrentDirectory, "test.html"));
            browser.RegisterJsObject("widgetControl", new WidgetControl());
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            timer = new System.Threading.Timer(_ => browser.Reload(), null, 20000, 20000);
        }
    }
}
