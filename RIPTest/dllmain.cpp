// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <iostream>
#include <intrin.h>
#include <Windows.h>
#include <Psapi.h>
#include "ClassTest.h"

ClassTest* CTest;

bool VerifyReturnAddress(void* retAddr)
{
    auto currModHandle = GetModuleHandle(NULL);

    MODULEINFO modinfo;
    GetModuleInformation(GetCurrentProcess(), currModHandle, &modinfo, sizeof(modinfo));

    if (retAddr >= currModHandle && (DWORD)retAddr <= (DWORD)currModHandle + modinfo.SizeOfImage)
        return true;
    else
        return false;
}

extern "C" __declspec(dllexport) void StandardFunction(int a1) 
{
    std::cout << "Message - " << a1 << std::endl;
    printf("Return address from %s: %p\n", __FUNCTION__, _ReturnAddress());

    bool isRetAddrInsideTheBaseModule = VerifyReturnAddress(_ReturnAddress());

    if (isRetAddrInsideTheBaseModule)
        std::cout << "Return address resides with-in the address range" << std::endl;
    else
        std::cout << "Return address does not reside with-in the address range" << std::endl;
}

void __stdcall Initialize()
{
    CTest = new ClassTest();
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(0, 0, (LPTHREAD_START_ROUTINE)Initialize, 0, 0, 0);
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

