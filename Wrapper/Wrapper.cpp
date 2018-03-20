// This is the main DLL file.

#include "stdafx.h"
#include "Wrapper.h"
using namespace std;

namespace Wrapper {

	WrapperClass::WrapperClass() {
		pricer1 = new Pricer();
	}

	double WrapperClass::getPriceEurostral() {
		return lien->PriceEurostral();
	
	}

	void  WrapperClass::getDeltaEurostral(cli::array<double,1>^ past,double t, cli::array<double,1>^ delta0,double H) {
		double * convert_past = convertArrayPointer(past);
		PnlVect * delta=lien->deltaEurostral(convert_past,t,H);
		//cli::array<double>^ deltacli = gcnew cli::array<double, 1>(5);
		convertPnlVectToCli(delta,delta0);
	}

	WrapperClass::WrapperClass(int size, double r, cli::array<double,2>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1) {
		pricer1 = new Pricer();
		double * convert_varHis = convertMatrixPointer(VarHis);
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
	
	void WrapperClass::convertPnlVectToCli(PnlVect * delta, cli::array<double, 1>^ delta0) {
		for (int i = 0; i < delta->size - 1; i++) {
			delta0[i] = pnl_vect_get(delta, i);
		}
		
	}
}

