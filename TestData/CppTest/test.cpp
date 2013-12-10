#include "test.h"

void GlobalFn()
{
}


namespace Test {

SomeClass::SomeClass()
{
}

SomeClass::SomeClass(int i)
{
}

SomeClass::SomeClass(const SomeClass& other)
{
}

SomeClass::~SomeClass()
{
}

SomeClass& SomeClass::operator=(const SomeClass& other)
{
	return *this;
}

int ReturnInt(int i)
{
	return AnEnum_Item1;
}


void SomeClass::Fn()
{
	AStruct as;
	AnEnum ae;
	int sam = 
		ReturnInt(7);	
		
	int k = sam * 2;		
}

}

