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
        private Form1 form;
        public bool Completed = false;
        public WidgetControl(Form1 owner)
        {
            form = owner;
        }
        public void SetDuration(int milliseconds)
        {
            form.WriteLog("WidgetControl: SetDuration(" + milliseconds.ToString() + ")");
            Completed = false;
            try
            {
                if (form.durationTimer != null)
                {
                    form.durationTimer.Change(milliseconds, Timeout.Infinite);
                   form.WriteLog( String.Format("WidgetControl.SetDuration: LengthTimer changed."));
                }
                else
                {
                   form.WriteLog( String.Format("WidgetControl.SetDuration: LengthTimer is null."));
                }
            }
            catch (Exception e)
            {
               form.WriteLog( String.Format("WidgetControl.SetDuration: Could not set implicit duration. {0}", e.Message));
            }
        }
        public void SetDurationComplete()
        {
           form.WriteLog( "WidgetControl: SetDurationComplete");
            Completed = true;
            try
            {
                if (form.durationTimer != null)
                {
                    form.durationTimer.Change(0, Timeout.Infinite);
                   form.WriteLog( String.Format("WidgetControl.SetDurationComplete: LengthTimer changed."));
                }
                else
                {
                   form.WriteLog( String.Format("WidgetControl.SetDurationComplete: LengthTimer is null."));
                }
            }
            catch (Exception e)
            {
               form.WriteLog( String.Format("WidgetControl.SetDurationComplete: Could not SetDurationComplete. {0}", e.Message));
            }
        }
        
        class JsCallbacks
        {
            public IJavascriptCallback Completed;
            public IJavascriptCallback Error;
            public IJavascriptCallback Debug;
        }

        public void Test(string input, IJavascriptCallback completedCallback = null, IJavascriptCallback errorCallback = null, IJavascriptCallback debugCallback = null)
        {
            Completed = false;

            if (debugCallback != null)
            {
               form.WriteLog(String.Format("debugCallback.1: canExecute={0}, isDisposed={1}", debugCallback.CanExecute, debugCallback.IsDisposed));
                debugCallback.ExecuteAsync(String.Format("debugCallback.1: canExecute={0}, isDisposed={1}", debugCallback.CanExecute, debugCallback.IsDisposed));
            }

            var state = new JsCallbacks
            {
                Completed = completedCallback,
                Error = errorCallback,
                Debug = debugCallback
            };

           form.WriteLog(String.Format("WidgetControl: Test {0}", input));

            var debugTimer = new System.Threading.Timer(_ =>
            {
                var st = ((JsCallbacks)_);
                if (st.Debug != null)
                {

                   form.WriteLog(String.Format("{2} - debugCallback.timer: canExecute={0}, isDisposed={1}", st.Debug.CanExecute, st.Debug.IsDisposed, DateTime.Now.ToString("s")));
                    st.Debug.ExecuteAsync(String.Format("{2} - debugCallback.timer: canExecute={0}, isDisposed={1}", st.Debug.CanExecute, st.Debug.IsDisposed, DateTime.Now.ToString("s")));

                }
            }, state, 1000, 2000);

            // Run it as a task to make sure not to lock the JS thread
            //Task.Run(() =>
            //{
            try
            {
                if (debugCallback != null)
                {
                   form.WriteLog(String.Format("{2} - debugCallback.2: canExecute={0}, isDisposed={1}", debugCallback.CanExecute, debugCallback.IsDisposed, DateTime.Now.ToString("s")));
                    debugCallback.ExecuteAsync(String.Format("{2} - debugCallback.2: canExecute={0}, isDisposed={1}", debugCallback.CanExecute, debugCallback.IsDisposed, DateTime.Now.ToString("s")));
                }
                form.RandomDelay().ContinueWith(async (t, _state) =>
                 {
                    //var result = ((Task<int>)t).Result;
                    var st = ((JsCallbacks)_state);
                     try
                     {
                         if (st.Debug != null)
                         {
                             var dc = (IJavascriptCallback)st.Debug;
                            form.WriteLog(String.Format("{2} - debugCallback.3: canExecute={0}, isDisposed={1}", dc.CanExecute, dc.IsDisposed, DateTime.Now.ToString("s")));
                             await dc.ExecuteAsync(String.Format("{2} - debugCallback.3: canExecute={0}, isDisposed={1}", dc.CanExecute, dc.IsDisposed, DateTime.Now.ToString("s")));
                         }
                         if (t.IsFaulted)
                         {
                            form.WriteLog(String.Format("WidgetControl: Test failed", t.Exception.Message));
                             await st.Error.ExecuteAsync(t.Exception.Message);
                         }
                         else
                         {
                           form.WriteLog(String.Format("WidgetControl: Test completed"));
                             await st.Completed.ExecuteAsync(t.Result);
                         }
                     }
                     catch (Exception e)
                     {
                        form.WriteLog(String.Format("WidgetControl: Test callback failed", e.Message));
                     }
                     debugTimer.Change(-1, -1);
                     debugTimer.Dispose();
                     debugTimer = null;

                 }, state: state);
            }
            catch (Exception e)
            {
                errorCallback.ExecuteAsync(e.Message);
               form.WriteLog(String.Format("WidgetControl: Test failed", e.Message));
            }
            //    });

        }
    }
}
