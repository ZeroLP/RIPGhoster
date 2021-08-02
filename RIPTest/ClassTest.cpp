#include "pch.h"
#include "ClassTest.h"
#include <intrin.h>
#include <Windows.h>
#include <Psapi.h>

#pragma intrinsic(_ReturnAddress)

bool ClassTest::VerifyReturnAddress(void* retAddr)
{
	auto currModHandle = GetModuleHandle(NULL);

	MODULEINFO modinfo;
	GetModuleInformation(GetCurrentProcess(), currModHandle, &modinfo, sizeof(modinfo));

	if (retAddr >= currModHandle && (DWORD)retAddr <= (DWORD)currModHandle + modinfo.SizeOfImage)
		return true;
	else
		return false;
}

void ClassTest::Method1(int arg1)
{
	std::cout << "argg: " << arg1 << std::endl;

	printf("Return address from %s: %p\n", __FUNCTION__, _ReturnAddress());

	bool isRetAddrInsideTheBaseModule = VerifyReturnAddress(_ReturnAddress());

	if (isRetAddrInsideTheBaseModule)
		std::cout << "Return address resides with-in the address range" << std::endl;
	else
		std::cout << "Return address does not reside with-in the address range" << std::endl;
}

