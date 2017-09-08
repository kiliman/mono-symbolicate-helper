using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace MonoSymbolicateHelper.Service
{
    class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<NancySelfHost>(s =>
                {
                    s.ConstructUsing(name => new NancySelfHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Service to auto symbolicate stack traces");
                x.SetDisplayName("Mono Symbolicate Helper Service");
                x.SetServiceName("MonoSymbolicateHelper");
            });
        }

    }
}
