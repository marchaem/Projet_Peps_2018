// This is the main DLL file.

#include "stdafx.h"
using namespace std;
using namespace cli;
#include "Wrapper.h"

namespace Wrapper {

	WrapperClass::WrapperClass() {
		pricer1 = new Pricer();
	}

	double WrapperClass::getPrice(int size, double r, cli::array<double>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1) {
		//Console::WriteLine("on rentre dans le getPrice");
		return lien->PriceEurostral();
	
	}

	WrapperClass::WrapperClass(int size, double r, cli::array<double>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1) {
		pricer1 = new Pricer();
		double * convert_varHis = convertArrayPointer(VarHis);
		double * convert_spot = convertArrayPointer(spot);
		double * convert_trend = convertArrayPointer(trend);
		double * convert_lambdas = convertArrayPointer(lambdas1);
		lien = new Lien(size,r,convert_varHis,convert_spot,convert_trend,fdStep,nbSamples,strike,T1,nbTimeSteps1,convert_lambdas);
	}

	double WrapperClass::getZero() {
		return 0.0;
	}

	double* WrapperClass::convertArrayPointer(cli::array<double> ^ arr) {
		double* res = new double[arr->Length];
		
		for (int i = 0; i < arr->Length; i++) {
			*(res + i) = arr[i];
		}

		return res;
	}

	double* WrapperClass::convertMatrixPointer(cli::array<double,2> ^ mat) {
		double* res = new double[mat->Length];
		
		int a = mat->GetLength(0);

		for (int i = 0; i < mat->GetLength(0); i++) {
		
			for (int j = 0; j < mat->GetLength(1); j++) {
				*(res + mat->GetLength(0)*i + j) = mat[i, j];
			}
		
		}

		return res;
	}
}

