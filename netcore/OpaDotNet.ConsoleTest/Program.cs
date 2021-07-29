using OPADotNet.Embedded;
using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;

namespace OpaDotNet.ConsoleTest
{
    class Program
    {

        static void Main(string[] args)
        {
            RestOpaClient restOpaClient = new RestOpaClient("http://127.0.0.1:8181");

            OpaStore store = new OpaStore();

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(store);

            var sync = new OpaSyncBuilder()
                .SetRestClient(restOpaClient)
                .AddModule("data.example", "data.reports")
                .SetEmbeddedClient(opaClientEmbedded)
                .Build();

            sync.Start().Wait();

            var partial = opaClientEmbedded.PreparePartial("data.example.allow == true");

            var part = partial.Partial(new
            {
                subject = new
                {
                    login = "test",
                    clearance_level = 20
                }
            }, new List<string>()
            {
                "data.reports"
            }).Result;
            Console.WriteLine("Hello World!");
        }
    }
}
