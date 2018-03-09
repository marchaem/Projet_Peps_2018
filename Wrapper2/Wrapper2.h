#pragma once
#include "pricer.hpp"
#include "Eurostral100.hpp"
#include "MonteCarlo.hpp"
#include "QuantoOption.hpp"
using namespace System;
using namespace pricerOption;


namespace Wrapper {

	public ref class WrapperClass
	{
	public:
		pricer * pricer = new pricerOption::pricer();
		WrapperClass();
		double getPrice();
		double getZero();
	};
}