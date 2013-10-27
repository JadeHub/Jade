#include <stdio.h>

class OpaqueClass;

struct FakeClass {
    int a;
    OpaqueClass *p;
    FakeClass();
};

int main(int, char**)
{
	FakeClass fc;
	return 0;
}
