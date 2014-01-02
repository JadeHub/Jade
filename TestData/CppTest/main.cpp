#include "test.h"
#include "test2.h"

namespace Test
{

ClassA::ClassA()
{
}

}


int main()
{
	Test::SomeClass::j = 8;

	Test::SomeClass sc;

	sc.Fn();
	return 0;
}