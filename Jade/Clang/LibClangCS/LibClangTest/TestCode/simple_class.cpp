#include "simple_class.h"

namespace TestCode
{

SimpleClass::SimpleClass()
{
}

SimpleClass::SimpleClass(const SimpleClass& other)
{
}

SimpleClass::~SimpleClass()
{
}

SimpleClass& SimpleClass::operator=(const SimpleClass& rhs)
{
	return *this;
}

void SimpleClass::Method(int param)
{
}

}
