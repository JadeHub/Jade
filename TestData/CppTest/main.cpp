#include "../CppTest/test.h"
#include "test2.h"
#include "template.h"

namespace Test
{

Struct22::Struct22(){}

ClassA::ClassA()
{
	Template<int> ti(5);
	int test = ti;
	double d = 7.0;
}

void NewFunction()
{

}

int ClassB::GetAnInt() const
{
	return 0;
}


void SomeClass::t() const
{
	
}


void SomeClass::t1() const {}

void SomeClass::Fn()
{
	mVec.push_back(5);

	SomeClass b(6);
	SomeClass a(8);
	SomeClass* sc = new SomeClass();
	InlinedMethod();
		
	AStruct as;
	AnEnum ae;
	/*int sam = 
		ReturnInt(7);	*/
		
//	int k = sam * 2;		
}

double SomeClass::Fn2(const int i, char* s) const
{
	return (double)i;
}

void SomeClass::VFunc()
{
	TemplMethod(5);
	int i = 8;
	TemplMethod2<5, int>(i);
}

Test::ClassA SomeNewFucntion()
{
	Test::ClassA a;
	
	for(int i = 0;i < 10;i++)
	{
		
	}
	return a;
}

int main()
{
	Test::SomeClass::j = 8;

	Test::SomeClass sc;

	sc.Fn();
	return 0;
}

}