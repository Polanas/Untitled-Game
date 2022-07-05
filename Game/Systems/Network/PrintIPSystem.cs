using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class PrintIPSystem : MySystem
{

    public override void OnGroupActivate()
    {
        base.OnGroupActivate();

        var client = new HttpClient();
        string ip;

        using (HttpResponseMessage response = client.GetAsync("http://icanhazip.com").Result)
        {
            using (HttpContent conent = response.Content)
            {
                ip = conent.ReadAsStringAsync().Result;
            }
        }

        Console.WriteLine($"Your IP adress: {ip}");

        client.Dispose();
    }
}
