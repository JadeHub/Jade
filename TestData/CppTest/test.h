#pragma once

#include "test2.h"
#include <vector>


extern void GlobalFn();

namespace Test {
	
struct AStruct;

struct AStruct
{
	int i;

	virtual void VFunc(int i, int j);
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

	const AStruct * PrtToAStruct;

	Struct22 a22;

	std::vector<int> mVec;

	static	int j;
	int ii;

	void Fn();
	double Fn2(const int i, const char* s) const;


	inline bool InlinedMethod()
	{
		return false;
	}
	
	void t() const;

	virtual void VFunc(int i, int j) override;

	template <typename P1>
	void TemplMethod(P1 p1) const {
	
		class d{};
	}

	template <typename P1, typename P2>
	void TemplMethod2(P1& p1, P1 p2) { p1 = i; }
private:
	void t1() const;
	void t1(int j) const {}

private:
	AStruct MSt;
	const AnEnum mEnum;
	int* p;
	
	int x;
};

class ClassA
{
public:
	ClassA();
};

class ClassB : public AStruct, public ClassA
{
public:
	ClassB(char& c) : mRef(c) {}
	
	int GetAnInt() const;
	
private:
	char& mRef;	
};

}
