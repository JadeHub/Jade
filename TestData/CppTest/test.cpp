#include "test.h"

void GlobalFn()
{
}

namespace Test {

SomeClass::SomeClass()
{
	unsigned u = 0;
	int i = u;

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
	AStruct as;
	as.i = i;
	return AnEnum_Item1;
}


void SomeClass::Fn()
{
	SomeClass a(8);
	SomeClass* sc = new SomeClass();
		
	AStruct as;
	AnEnum ae;
	int sam = 
		ReturnInt(7);	
		
	int k = sam * 2;		
}

}






