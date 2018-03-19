// Wrapper.h

#pragma once
#include "Pricer.hpp"

using namespace System;
using namespace cli;

namespace Wrapper {

	public ref class WrapperClass
	{
		Pricer * pricer1;
	public:
		WrapperClass::WrapperClass();
		double getPrice();
		double getZero();
		double* convertArrayPointer(array<double> ^ arr);
		double* convertMatrixPointer(array<double,2> ^ mat);
	};
}
