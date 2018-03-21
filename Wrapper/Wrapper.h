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
	    WrapperClass(int size, double r, cli::array<double,2>^ covlogR, cli::array<double,1>^ spot, cli::array<double, 1>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1);
		double getPriceEurostral();
		double getPriceEurostral(double t, cli::array<double, 2>^ past);
		//cli::array<double>^ getDeltaEurostral(cli::array<double, 2>^ past, double t);
		void getDeltaEurostral(cli::array<double, 2>^ past, double t, [Runtime::InteropServices::OutAttribute] cli::array<double, 1>^ %delta0,double H);
		double getZero();
		double* convertArrayPointer(cli::array<double,1> ^ arr);
		double* convertMatrixPointer(cli::array<double,2> ^ mat);
		//cli::array<double>^ convertPnlVectToCli(PnlVect * delta);
		void convertTabToCli(double * delta, cli::array<double, 1>^ delta0);
		
	};
}
