using System;

namespace Arn0ld
{
    class Program
    {
        static void Main(string[] args)
        => new BotCore().MainAsync().GetAwaiter().GetResult();
    }
}