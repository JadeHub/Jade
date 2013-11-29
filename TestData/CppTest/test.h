#pragma once

#include "test2.h"

namespace Test {

struct AStruct;

struct AStruct
{
	int i;
};

enum AnEnum
{
	k = 0
};

class SomeClass
{
public:
	SomeClass();


	static	int j;

	void Fn();
private:
	AStruct MSt;
	AnEnum mEnum;
	int p;
};

}

