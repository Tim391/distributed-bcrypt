using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace DistributedBCrypt.Worker1
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<Worker>(s =>
                {
                    s.ConstructUsing(name => new Worker());
                    s.WhenStarted(svc => svc.Start());
                    s.WhenStopped(svc => svc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("BCryptWorker");        
                x.SetDisplayName("BCryptWorker");                      
                x.SetServiceName("BCryptWorker");
            });
        }
    }
}
