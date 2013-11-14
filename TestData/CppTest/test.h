#pragma once

//#include <string>


namespace Test {

struct AStruct;

struct AStruct
{
	int i;
};

class SomeClass
{
public:
	SomeClass();

	void Fn();
private:
	AStruct MSt;
	int p;
};

}