#pragma once

template<typename T>
class Template
{
public:
	Template(const T& t) : mT(t) {}

	operator const T&()  {return mT;}

private:
	T mT;
};