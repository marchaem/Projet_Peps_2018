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
		cli::array<double, 1>^  getDeltaEurostral(cli::array<double, 2>^ past, double t,double H);
		double getZero();
		double getPLEurostral(cli::array<double, 2>^past,double H);
		double* convertArrayPointer(cli::array<double,1> ^ arr);
		double* convertMatrixPointer(cli::array<double,2> ^ mat);
		//cli::array<double>^ convertPnlVectToCli(PnlVect * delta);
		cli::array<double, 1>^ convertTabToCli(double * delta);
		void test(cli::array<double, 1>^);
		void trackingError(cli::array<double, 2> ^ past, double t, double H, cli::array<double, 1>^ pricet, cli::array<double, 1>^ pocket, cli::array<double, 1>^ trackingE,int nbre);
		
	};
}
