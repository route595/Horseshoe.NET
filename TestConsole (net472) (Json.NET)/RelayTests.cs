using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.RelayMessage;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole
{
    class RelayTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "relay messages to console",
                () =>
                {
                    var relay = new RelayToConsole();
                    var lib = new LibWRelay { Relay = relay };
                    relay.Message("start time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    lib.Go();
                    Console.WriteLine();
                    Console.WriteLine("One more time, this time catching exception...");
                    Console.WriteLine();
                    try
                    {
                        lib.Go(throwExceptionInTask3: true);
                    }
                    catch (Exception ex)
                    {
                        relay.Exception(ex);
                    }
                    relay.Message("end time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), indentHint: IndentHint.Reset);
                }
            ),
            BuildMenuRoutine
            (
                "relay progress to console [basic]",
                () =>
                {
                }
            )
        };
    }

    class LibWRelay
    {
        public const int TASK_2_INIT = 13001;
        public const int TASK_2_SUCCESS = 13002;

        public IMessageRelay Relay { get; set; }

        public void Go(bool throwExceptionInTask3 = false)
        {
            Relay?.Message("LibWRelay.Go()", indentHint: IndentHint.IncrementNext);
            Relay?.Message("Task 1");
            Thread.Sleep(500);
            Relay?.Message("result: Success");
            Relay?.Message("Task 2", id: TASK_2_INIT);
            Thread.Sleep(500);
            Relay?.Message("result: Success", id: TASK_2_SUCCESS);
            Relay?.Message("Task 3");
            Thread.Sleep(500);
            if (throwExceptionInTask3)
                throw new Exception("Exception in Task 3");
            Relay?.Message("result: Success");
            Relay?.Message("end", indentHint: IndentHint.Decrement);
        }
    }
}
