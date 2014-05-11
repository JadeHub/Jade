#include "../CppTest/test.h"
#include "test2.h"
#include "template.h"

namespace Test
{


ClassA::ClassA()
{
	Template<int> ti(5);

	int test = ti;

}

void SomeClass::Fn()
{
	SomeClass a(8);
	SomeClass* sc = new SomeClass();
		
	AStruct as;
	AnEnum ae;
	/*int sam = 
		ReturnInt(7);	*/
		
//	int k = sam * 2;		
}

}


int main()
{
	Test::SomeClass::j = 8;

	Test::SomeClass sc;

	sc.Fn();
	return 0;
}