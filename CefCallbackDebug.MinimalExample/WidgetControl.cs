using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefCallbackDebug
{
    public class WidgetControl
    {

        public void Test(IJavascriptCallback callback = null)
        {
            var ms = new Random().Next(8000, 16000);
            System.Diagnostics.Debug.WriteLine("Test.Delay = " + ms.ToString());
            var _timer = new System.Threading.Timer(async _ =>
            {

                System.Diagnostics.Debug.WriteLine("Test.Callback");
                if (callback.CanExecute == true)
                {
                    try
                    {
                        await callback.ExecuteAsync();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Test.Callback failed. " + e.Message);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Test.Callback.CanExecute = false");
                }

            }
            , null, ms, -1);
        }
    }
}
