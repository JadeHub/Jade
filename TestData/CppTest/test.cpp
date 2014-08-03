#include "test.h" 

void GlobalFunctionWithExternPrototype()
{
}
 
namespace TestNS {

int SomeClass::staticInt = 0;

SomeClass::SomeClass()
: PrtToAStruct(nullptr), ii(0), mEnum(AnEnum_Item1), p(nullptr)
{
	unsigned u = 0;
	int i = u;

}

SomeClass::SomeClass(int i)
: PrtToAStruct(nullptr), ii(1), mEnum(AnEnum_Item1), p(nullptr)
{
	int p = 0;
}

SomeClass::SomeClass(const SomeClass& other)
: PrtToAStruct(other.PrtToAStruct), ii(other.ii), mEnum(other.mEnum), p(other.p)
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

