#pragma once

#include "test2.h"

extern void GlobalFn();

namespace Test {
	
struct AStruct;

struct AStruct
{
	int i;
};

enum AnEnum
{
	AnEnum_Item1 = 0
};

class SomeClass
{
public:
	SomeClass();
	SomeClass(int i);
	SomeClass(const SomeClass& other);
	~SomeClass();

	SomeClass& operator=(const SomeClass& other);

	Struct22 a22;

	static	int j;
	int ii;

	void Fn();

	inline bool InlinedMethod()
	{
		return false;
	}

private:
	AStruct MSt;
	AnEnum mEnum;
	int p;
};

class ClassA
{
public:
	ClassA();
};

class ClassB
{
};

}
