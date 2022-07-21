using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coroutines;

namespace Game;

class CoroutinesTest : MySystem
{

    private CoroutineRunner _runner;

  

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _runner = new();
    }

    public override void Run(EcsSystems systems)
    {
       
        _runner.Update(sharedData.physicsData.deltaTime);
    }

    IEnumerator DoSomeCounting()
    {
        Console.WriteLine("Counting to 3 slowly...");
        yield return CountTo(3, 2.0f);
        Console.WriteLine("Done!");

        Console.WriteLine("Counting to 5 normally...");
        yield return CountTo(5, 1.0f);
        Console.WriteLine("Done!");

        Console.WriteLine("Counting to 99 quickly...");
        yield return CountTo(99, 0.1f);
        Console.WriteLine("Done!");
    }

    private IEnumerator CountTo(int num, float delay)
    {
        for (int i = 1; i <= num; ++i)
        {
            yield return delay;
            Console.WriteLine(i);
        }
    }

}