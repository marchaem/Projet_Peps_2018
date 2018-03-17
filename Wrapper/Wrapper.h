// Wrapper.h

#pragma once
#include "Pricer.hpp"

using namespace System;

namespace Wrapper {

	public ref class WrapperClass
	{
		Pricer * pricer1;
	public:
		WrapperClass::WrapperClass();
		double getPrice();
		double getZero();
	};
}
