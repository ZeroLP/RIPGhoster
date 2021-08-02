#pragma once

#include <iostream>

class ClassTest
{
public:
	void Method1(int arg1);
	void GetThisHandle() { std::cout << this << std::endl; }
	bool VerifyReturnAddress(void* retAddr);

	void(__thiscall ClassTest::* pFunc)(int) = &ClassTest::Method1;
	void* pPtr = (void*&)pFunc;
};

