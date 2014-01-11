#pragma once

namespace TestCode
{
	class SimpleClass
	{
	public:
		SimpleClass();
		SimpleClass(const SimpleClass& other);
		~SimpleClass();

		SimpleClass& operator=(const SimpleClass& rhs);

		void Method(int param);
	};
}
