//using System;
//using System.Collections.Generic;

//using Horseshoe.NET.Caching;
//using Horseshoe.NET.ConsoleX;

//namespace TestConsole
//{
//    class AppCacheTests : RoutineX
//    {
//        private IRuntimeCache runtimeCache { get; }

//        public override string Text => "AppCache Tests";

//        public AppCacheTests()
//        {
//            runtimeCache = new RuntimeCache();
//        }

//        public override IList<MenuObject> Menu => new []
//        {
//            BuildMenuRoutine
//            (
//                "Quick runtime cache test",
//                () =>
//                {
//                    int num = 0;
//                    var key = "Quick-runtime-cache-test-key";
//                    bool fromCache;
//                    bool looping = true;
//                    Console.WriteLine("Press [Enter] key to retrieve the cached value.  Press [Esc] when finished.");
//                    Console.WriteLine("Note, cache duration is 5 seconds.");
//                    Console.WriteLine("A new number will appear (and be cached) if [space] is pressed after cache expires.");
//                    Console.WriteLine(runtimeCache.GetFromCache<int>(key, () => ++num, cacheDuration: 5));
//                    while (looping)
//                    {
//                        var keyInfo = Console.ReadKey();
//                        if (keyInfo.Key == ConsoleKey.Enter)
//                        {
//                            int value = runtimeCache.GetFromCache<int>(key, () => ++num, out fromCache, cacheDuration: 5);
//                            Console.WriteLine(value + (fromCache ? "" : "  (not from cache)"));
//                        }
//                        else
//                        {
//                            Console.Write("\b \b");
//                            if (keyInfo.Key == ConsoleKey.Escape)
//                            {
//                                looping = false;
//                            }
//                        }
//                    }
//                    Console.WriteLine("cache test complete!");
//                }
//            ),
//        };
//    }
//}
