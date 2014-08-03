#include "../CppTest/test.h"
#include "test2.h"
#include "template.h"

int global = 0;

void GlobalFunctionNoImpl();

namespace {

	void AnonNamespaceFunc() {}
}

namespace Blah { namespace ModuleA { namespace ModuleB { class ClassInAB{ }; } } }

namespace TestNS
{

Struct22::Struct22(){}

ClassA::ClassA()
{
	Template<int> ti(5);
	Template<float> tf(5);
	int test = ti;
	double d = 7.0;
	
}

///\brief NewFunction!
void NewFunction(int i)
{

}

int ClassB::GetAnInt() const
{
	return 0;	
}

template<typename T> 
void GlobalTemplFunc(T t)
{
}

void SomeClass::t() const
{
	SomeClass::staticInt = 0;
	const char * str = "";
	
	this->Fn2(1, str);
	TemplMethod(5);
}

//void SomeClass::t1(int j) const {}

void SomeClass::t1() const {}

class Overloads
{
public:
	Overloads() {}
	~Overloads();
	
	void Func() {}
	void Func(int i, int j) {}
	void Func(double d, int j) {}	
	
	void Test() {}	
private:
	Overloads(const Overloads&); 
	Overloads& operator =(const Overloads& o);	
	Overloads&& operator =(const Overloads&& o);
	
	int i;
};

Overloads::~Overloads()
{
}

void SomeClass::Fn()
{
	mVec.push_back(5);

	SomeClass b(6);
	SomeClass a(8);
	SomeClass* sc = new SomeClass();
	InlinedMethod();
	
	AStruct as;
	AnEnum ae;
	int i = 7;
	this->TemplMethod2<int, int>(i, 8);
	
	Overloads o;
			
	int k = as.i;
	
	/*int sam = 
		ReturnInt(7);	*/
		
//	int k = sam * 2;		
}

double SomeClass::Fn2(const int i, const char* s) const
{
	return (double)i;
}

void SomeClass::VFunc(int i, int j)
{
	TemplMethod(5);
	int ii = 8;	
}

void GlobalFunctionNoPrototype()
{
}

int main()
{
	TestNS::SomeClass::staticInt = 8;

	TestNS::SomeClass sc;

	sc.Fn();
	return 0;
}

}