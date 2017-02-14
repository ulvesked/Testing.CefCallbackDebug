# Testing.CefCallbackDebug

Sample application for CefSharp issue 1939: https://github.com/cefsharp/CefSharp/issues/1939

Register an object with RegisterJsObject with a method that accepts an IJavascriptCallback parameter. In the method, initialize a timer that fires the callback after 10-15 seconds. The callback is sometimes not executed in the browser.

Expected result: The IJavascriptCallback is executed in the browser when triggered from C#.
Actual result: Occationally the callback sent from C# (using IJavascriptCallback.ExecuteAsync) is not executed in the browser.

## CefCallbackDebug.MinimalExample

The CefCallbackDebug.MinimalExample is a really stripped down sample which rarely shows the issues, but they still occur. Run the application and point your browser to http://localhost:8080 for remote debugging and watch the console for "test" + "success" messages.

## CefCallbackDebug

This is a slightly more complex real application which loads two different webpages and uses more callbacks. The issues seem to occur more frequently in this one. This project also has more friendly debugging.
