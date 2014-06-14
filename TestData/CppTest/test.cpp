#include "test.h" 

void GlobalFn()
{
}
 
namespace Test {

int SomeClass::j = 0;

SomeClass::SomeClass()
: ii(0), p(nullptr)
{
	unsigned u = 0;
	int i = u;

}

SomeClass::SomeClass(int i)
: ii(i), p(&i)
{
	int p = 0;
}

SomeClass::SomeClass(const SomeClass& other)
: ii(other.ii), p(other.p)
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




}

