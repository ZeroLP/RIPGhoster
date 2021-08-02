using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RIPGhoster.Tools;

namespace RIPGhoster
{
    public class StandardFunctionCaller
    {
        /*[DllImport("RIPTest.dll")]
        private static extern void StandardFunction(int a1);*/

        public unsafe static void CallStandardFunctionUnsafe(nint funcAddr, int a1)
        {
            Console.WriteLine($"Calling unsafe StandardFunction at 0x{funcAddr:X} with a1 param: {a1}.");

            ((delegate* unmanaged[Stdcall]<int, void>)funcAddr)(a1);
        }

        /// <summary>
        /// Calls the StandardFunction by jmping to a virtual jmptable which calls a custom constructed function that is allocated near from the end of the process address space.
        /// </summary>
        public unsafe static void CallStandardFunctionSafeWithJmpTable(nint funcAddr, int a1)
        {
            byte[] safeFunctionInstruct = new byte[]
            {
                //.CallFoo(int a1) => Foo(int a1)

                0x55,                                  //push ehp
                0x89, 0xE5,                            //mov ebp, esp

                0x8B, 0x45, 0x08,                      //mov eax, [ebp + 8]
                0x50,                                  //push eax
                0xBA, 0x00, 0x00, 0x00, 0x00,          //mov edx, 0x0
                0xFF, 0xD2,                            //call edx

                0x8B, 0xE5,                            //mov esp, ebp
                0x5D,                                  //pop ebp
                0xC3                                   //ret
            };

            var currProc = Process.GetCurrentProcess();
            NativeImport.GetModuleInformation(currProc.Handle, currProc.MainModule.BaseAddress, out var modInfo, (uint)Marshal.SizeOf(new NativeImport.NativeStructs.MODULEINFO()));

            var allocSafeFunctionPtr = Memory.AllocateSafeMemory(safeFunctionInstruct.Length);

            NativeImport.VirtualProtect(allocSafeFunctionPtr, (nuint)safeFunctionInstruct.Length, NativeImport.NativeStructs.MemoryProtection.ExecuteReadWrite, out var oldProt);

            fixed (void* inst = &safeFunctionInstruct[0])
            {
                NativeImport.VirtualProtect((IntPtr)inst, (nuint)safeFunctionInstruct.Length, NativeImport.NativeStructs.MemoryProtection.ExecuteReadWrite, out var oldProt_a);

                *(nint*)((nint)inst + 8) = funcAddr;

                byte* pDest = (byte*)allocSafeFunctionPtr;
                byte* pSrc = (byte*)inst;

                for (int i = 0; i < safeFunctionInstruct.Length; i++)
                    *(pDest + i) = *(pSrc + i);
            }

            byte[] jmpTableinstruct = new byte[]
            {
                0x68, 0x00, 0x00, 0x00, 0x00, //push eax
                0xC3                          //ret
            };

            fixed (byte* inst = &jmpTableinstruct[0])
            {
                NativeImport.VirtualProtect((IntPtr)inst, (nuint)jmpTableinstruct.Length, NativeImport.NativeStructs.MemoryProtection.ExecuteReadWrite, out var oldProt_b);

                *(nint*)((nint)inst + 1) = allocSafeFunctionPtr;

                Console.WriteLine($"Calling safe StandardFunction at 0x{allocSafeFunctionPtr:X} with param a1: {a1} and JMPTable at: 0x{(nint)inst:X}.");
                ((delegate* unmanaged[Cdecl]<int, void>)inst)(a1);
            }
        }
    }
}
