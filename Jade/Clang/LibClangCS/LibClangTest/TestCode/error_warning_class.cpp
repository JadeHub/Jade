#include "error_warning_class.h"

namespace TestCode
{

ErrorClass::ErrorClass()
{
}

void ErrorClass::Method(int param)
{
	//Warning: unused
	int unused = 0;
}

void BadFnPtr()
{
	int P,Q;
	//Error: Bad function ptr
	(P-Q)();
}

}

