// Wrapper.h

#pragma once
#include "Pricer.hpp"
#include "Lien.hpp"

using namespace System;

namespace Wrapper {

	public ref class WrapperClass
	{
		Pricer * pricer1;
		Lien * lien;
		int size_;

	public:
		WrapperClass();
	    WrapperClass(int size, double r, cli::array<double,2>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1);
		double getPriceEurostral();
		void getDeltaEurostral(cli::array<double, 1>^ past, double t, cli::array<double, 1>^ delta0,double H);
		double getZero();
		double* convertArrayPointer(cli::array<double,1> ^ arr);
		double* convertMatrixPointer(cli::array<double,2> ^ mat);
		void convertTabToCli(double * delta, cli::array<double, 1>^ delta0);
		
	};
}
