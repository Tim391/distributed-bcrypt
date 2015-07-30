using Topshelf;

namespace DistributedBCrypt.Worker
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<DistributedBCrypt.Worker.Worker>(s =>
                {
                    s.ConstructUsing(name => new DistributedBCrypt.Worker.Worker());
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
