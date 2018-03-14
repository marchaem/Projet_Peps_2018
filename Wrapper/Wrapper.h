// Wrapper.h

#pragma once
#include "pricer.hpp"

using namespace System;
using namespace pricerOption;

namespace Wrapper {

	public ref class WrapperClass
	{
		pricerOption::pricer * pricer;
	public:
		WrapperClass::WrapperClass();
		double getPrice();
		double getZero();
	};
}
