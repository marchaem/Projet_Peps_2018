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

<<<<<<< HEAD
	/*cli::array<double>^ WrapperClass::getDeltaEurostral(cli::array<double,2>^ past,double t) {
		double * convert_past = convertMatrixPointer(past);
		PnlVect * delta=lien->deltaEurostral(convert_past,t);
		return(convertPnlVectToCli(delta));
	}*/

	WrapperClass::WrapperClass(int size, double r, cli::array<double,2>^ VarHis, cli::array<double>^ spot, cli::array<double>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double>^ lambdas1) {

	void  WrapperClass::getDeltaEurostral(cli::array<double,1>^ past,double t, cli::array<double,1>^ delta0,double H) {
		pin_ptr<double> pPast = &past[0];
		double *convert_past = pPast;
		//double * convert_past = convertArrayPointer(past);
		double * delta = new double[size_];
		delta=lien->deltaEurostral(convert_past,t,H);
		//cli::array<double>^ deltacli = gcnew cli::array<double, 1>(5);		
		convertTabToCli(delta,delta0);
	}

	WrapperClass::WrapperClass(int size, double r, cli::array<double,2>^ VarHis, cli::array<double,1>^ spot, cli::array<double,1>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double,1>^ lambdas1) {
		pricer1 = new Pricer();
		size_ = size;
		pin_ptr<double> pS = &spot[0];
		double *convert_spot = pS;
		pin_ptr<double> pT = &trend[0];
		double *convert_trend = pT;
		pin_ptr<double> pL = &lambdas1[0];
		double *convert_lambdas = pL;
		double * convert_varHis = convertMatrixPointer(VarHis);
		//double * convert_spot = convertArrayPointer(spot);
		//double * convert_trend = convertArrayPointer(trend);
		//double * convert_lambdas = convertArrayPointer(lambdas1);
		lien = new Lien(size,r,convert_varHis,convert_spot,convert_trend,fdStep,nbSamples,strike,T1,nbTimeSteps1,convert_lambdas);
	}
	
	double WrapperClass::getZero() {
		return 0.0;
	}

	double* WrapperClass::convertArrayPointer(cli::array<double,1> ^ arr) {
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
				//*(res + mat->GetLength(0)*i + j) = mat[i, j];
				res[a*i+j] = mat[i, j];
			}		
		}
		return res;
	}

	/*cli::array<double>^ WrapperClass::convertPnlVectToCli(PnlVect * delta) {
		cli::array<double>^ delta_cli = gcnew cli::array<double,1>(delta->size);
		for (int i = 0; i < delta->size - 1; i++) {
			delta_cli[i] = pnl_vect_get(delta, i);
		}
		return delta_cli;
	}*/
	
	void WrapperClass::convertTabToCli(double* delta, cli::array<double, 1>^ delta0) {
		for (int i = 0; i < sizeof(delta); i++) {
			delta0[i] = delta[i];
		}
		
	}
}

