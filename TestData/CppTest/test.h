#pragma once

#include "test2.h"



extern void GlobalFn();

namespace Test {
	
struct AStruct;

struct AStruct
{
	int i;

	virtual void VFunc();
};

enum AnEnum
{
	AnEnum_Item1 = 0
};

class SomeClass : AStruct
{
public:
	SomeClass();
	SomeClass(int i);
	SomeClass(const SomeClass& other);
	~SomeClass();

	SomeClass& operator=(const SomeClass& other);

	Struct22 a22;

	static	int j;
	int ii;

	void Fn();
	double Fn2(const int i, char* s) const;


	inline bool InlinedMethod()
	{
		return false;
	}
	
	void t() const;

	virtual void VFunc() override;
private:
	void t1() const;

private:
	AStruct MSt;
	AnEnum mEnum;
	int* p;
};

class ClassA
{
public:
	ClassA();
};

class ClassB : public AStruct, public ClassA
{
public:
	ClassB();
	
	int GetAnInt() const;
};

}
