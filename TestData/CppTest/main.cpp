#include "../CppTest/test.h"
#include "test2.h"
#include "template.h"

namespace Test
{


ClassA::ClassA()
{
	Template<int> ti(5);
	int test = ti;
	double d = 7.0;
}

void SomeClass::Fn()
{
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