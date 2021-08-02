using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RIPGhoster
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RIPGhoster";

            var riptestHandle = NativeLibrary.Load($@"{System.IO.Directory.GetCurrentDirectory()}\RIPTest.dll");
            
            //Could just use Kernel32!GetProcAddress here.
            var standardFunctionRVA = (nint)riptestHandle + 0x12C80; //55 8B EC 81 EC ? ? ? ? 53 56 57 8D 7D F4

            Console.WriteLine($"StandardFunction: 0x{standardFunctionRVA:X}");

            Console.WriteLine("");
            StandardFunctionCaller.CallStandardFunctionUnsafe(standardFunctionRVA, 100);

            Console.WriteLine("");
            StandardFunctionCaller.CallStandardFunctionSafeWithJmpTable(standardFunctionRVA, 100);

            /*var thisHandle = (nint)riptestHandle + 0x1D3DC; //89 15 ? ? ? ? 5F 
            var method1RVA = (nint)riptestHandle + 0x12400; //55 8B EC 81 EC ? ? ? ? 53 56 57 51 8D 7D E8

            Console.WriteLine($"ClassTest Handle: 0x{thisHandle:X}");
            Console.WriteLine($"ClassTest::Method1: 0x{method1RVA:X}");

            Console.WriteLine("");
            ThisCallFunctionCaller.CallThisCallFunctionUnsafe(method1RVA, thisHandle, 100);

            Console.WriteLine("");
            ThisCallFunctionCaller.CallThisCallFunctionSafeWithJmpTable(method1RVA, thisHandle, 100);*/

            Console.ReadLine();
        }
    }
}
