using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class PrintIPSystem : MySystem
{
    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Console.WriteLine($"Your IP adress: {ip}");
                return;
            }
        }
    }
}
