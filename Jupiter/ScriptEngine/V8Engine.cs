using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Jupiter.ScriptEngine
{
    public class ScriptUtils
    {
        private ViewModels.LogStream log;

        private CancellationToken cancel;

        public ScriptUtils(ViewModels.LogStream log, CancellationToken cancel)
        {
            this.log = log;

            this.cancel = cancel;
        }

        public void WriteLine(object text)
        {
            this.log.Write(text?.ToString());
        }

        public void Sleep(int n)
        {
            try
            {
                System.Threading.Tasks.Task.Delay(n, cancel).Wait();
            }
            catch (AggregateException e)
            {
                if(e.InnerException != null)
                {
                    WriteLine(e.InnerException.Message);
                }
                else
                {
                    WriteLine(e.Message);
                }
            }
        }
    }

    public class V8Engine
    {
        private ScriptClient client;

        private V8ScriptEngine engine;

        public V8Engine(Client client)
        {
            this.client = new ScriptClient(client);
        }

        public void Run(string script, ViewModels.LogStream log, CancellationToken cancel)
        {
            var utils = new ScriptUtils(log, cancel);
            try
            {
                engine?.Interrupt();

                engine = new V8ScriptEngine();
                engine.AddHostObject("utils", utils);
                engine.AddHostObject("client", this.client);

                var evalResult = engine.Evaluate("script", script);
            }
            catch(ScriptEngineException e)
            {
                utils.WriteLine(e.ErrorDetails);
            }
            catch(ScriptInterruptedException e)
            {
                utils.WriteLine(e.ErrorDetails);
            }
        }

        public void Stop()
        {
            engine?.Interrupt();
        }
    }
}