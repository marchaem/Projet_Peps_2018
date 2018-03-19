// Wrapper.h

#pragma once
#include "Pricer.hpp"
#include "Lien.hpp"

using namespace System;
using namespace cli;

namespace Wrapper {

	public ref class WrapperClass
	{
		Pricer * pricer1;
		Lien * lien;
		
	public:
		WrapperClass::WrapperClass();
	    WrapperClass::WrapperClass(int size, double r, cli::array<double>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1);
		double getPrice(int size, double r, cli::array<double>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1);
		double getZero();
		double* convertArrayPointer(cli::array<double> ^ arr);
		double* convertMatrixPointer(cli::array<double,2> ^ mat);
		
	};
}
