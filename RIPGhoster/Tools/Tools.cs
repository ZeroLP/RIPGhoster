using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace RIPGhoster.Tools
{
    public class Memory
    {
        public unsafe static nint AllocateSafeMemory(int size)
        {
            var currProc = System.Diagnostics.Process.GetCurrentProcess();
            NativeImport.GetModuleInformation(currProc.Handle, currProc.MainModule.BaseAddress, out var modInfo, (uint)Marshal.SizeOf(new NativeImport.NativeStructs.MODULEINFO()));

            nint dest = ((nint)(((nint)currProc.MainModule.BaseAddress + modInfo.SizeOfImage) - 0x1000));

            Console.WriteLine($"Allocating safe memory at: 0x{dest:X} with size: {size}");

            NativeImport.VirtualProtect((IntPtr)dest, (nuint)size, NativeImport.NativeStructs.MemoryProtection.ExecuteReadWrite, out var oldProt_a);

            //Credit @github.com/Sadulisten for this piece of code for nopping out instructions in range. (Slightly modified)
            byte[] nopInstructions = Enumerable.Range(0, size).Select(b => (byte)0x90).ToArray();

            fixed (byte* src = nopInstructions)
                Unsafe.CopyBlockUnaligned((byte*)dest, src, (uint)nopInstructions.Length);
            //Credit end

            return dest;
        }
    }
}
