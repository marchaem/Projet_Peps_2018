// This is the main DLL file.

#include "stdafx.h"
#include "Wrapper.h"
using namespace std;


/*int main(int argc, char* argv[], char *envp[]) {
	/*int size = 5;
	double r = 0.01;
	cli::array<double, 2> ^ varHis = gcnew cli::array<double, 2>(5, 5);
	for (int i = 0; i < 5; i++)
	{
		for (int j = 0; j < 5; j++)
		{
			varHis[i, j] = 0.15;
		}

	}

	cli::array<double> ^ spots = gcnew cli::array<double>(5);
	for (int i = 0; i < 5; i++)
	{
		spots[i] = 100;
	}



	cli::array<double> ^ trends = gcnew cli::array<double>(5);
	for (int i = 0; i < 5; i++)
	{
		trends[i] = 0.05;
	}

	cli::array<double> ^ lambdas = gcnew cli::array<double>(5);
	for (int i = 0; i < 5; i++)
	{
		lambdas[i] = 0.05;
	}

	Wrapper::WrapperClass^ wc = gcnew Wrapper::WrapperClass(size, r, varHis, spots, trends, 0.1, 1, 100, 8.0, 3, lambdas);

	cli::array<double> ^ deltas = gcnew cli::array<double>(5);
	cli::array<double,2> ^ past = gcnew cli::array<double,2>(1,5);
	for (int i = 0; i < 1; i++)
		{
			for (int j = 0; j < 5; j++) {
				past[i,j] = 20.0;
			}
		}
	wc->getDeltaEurostral(past, 0.0, deltas, 0.1);
	cout << " hello " << endl;
	cout << deltas[0] << endl;
	cout << "bye" << endl;

	return 0;
}*/

namespace Wrapper {

	WrapperClass::WrapperClass() {
		pricer1 = new Pricer();
	}

	double WrapperClass::getPriceEurostral() {
		return lien->PriceEurostral();	
	}

	double WrapperClass::getPriceEurostral(double t, cli::array<double, 2>^ past) {
		double * convert_past = convertMatrixPointer(past);		
		return lien->PriceEurostral(convert_past, t);
	}

	double WrapperClass::getPLEurostral(cli::array<double, 2>^past, double H) {
		double * convert_past = convertMatrixPointer(past);
		return lien->profitLoss_Eurostral(convert_past, H);
	}

	double WrapperClass::getForwardTest(double H) {
		return lien->forwardTest(H);
	}

	/*cli::array<double>^ WrapperClass::getDeltaEurostral(cli::array<double,2>^ past,double t) {
		double * convert_past = convertMatrixPointer(past);
		PnlVect * delta=lien->deltaEurostral(convert_past,t);
		return(convertPnlVectToCli(delta));
	}*/
	
	 void WrapperClass::test(cli::array<double, 1>^ test) {
		 test[0] = 5.2152;
	}

	cli::array<double, 1>^    WrapperClass::getDeltaEurostral(cli::array<double,2>^ past,double t,double H) {
		/*pin_ptr<double> pPast = &past[0];
		double *convert_past = pPast;*/

		double *convert_past= convertMatrixPointer(past);
		//double * convert_past = convertArrayPointer(past);
		double * delta = new double[size_];		
		delta = lien->deltaEurostral(convert_past,t,H);
		//cli::array<double>^ deltacli = gcnew cli::array<double, 1>(5);	
		return convertTabToCli(delta);
	}

	WrapperClass::WrapperClass(int size, double r, cli::array<double,2>^ covlogR, cli::array<double,1>^ spot, cli::array<double, 1>^ trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, cli::array<double,1>^ lambdas1) {
		//pricer1 = new Pricer();
		size_ = size;
		pin_ptr<double> pS = &spot[0];
		double *convert_spot = pS;
		pin_ptr<double> pT = &trend[0];
		double *convert_trend = pT;
		pin_ptr<double> pL = &lambdas1[0];
		double *convert_lambdas = pL;
		double * convert_covlogr = convertMatrixPointer(covlogR);
		//double * convert_spot = convertArrayPointer(spot);
		//double * convert_trend = convertArrayPointer(trend);
		//double * convert_lambdas = convertArrayPointer(lambdas1);
		lien = new Lien(size,r,convert_covlogr,convert_spot,convert_trend,fdStep,nbSamples,strike,T1,nbTimeSteps1,convert_lambdas);
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
		
		int a = mat->GetLength(1);
		//cout << "dans convert" << a << endl;
		//cout << "dans convert" << mat->GetLength(1) << endl;
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
	
	cli::array<double, 1>^  WrapperClass::convertTabToCli(double* delta ) {

		cli::array<double>^ deltacli = gcnew cli::array<double, 1>(5);
		for (int i = 0; i < 5; i++) {
			deltacli[i] = delta[i];
		}
		return deltacli;
	}

	void WrapperClass::trackingError(cli::array<double, 2> ^ past, double t, double H, cli::array<double, 1>^ pricet, cli::array<double, 1>^ pocket, cli::array<double, 1>^ trackingE,int nbre) {
		double * pastP = convertMatrixPointer(past);
		double * pricetP = new double[pricet->Length];
		double * pocketP = new double[pocket->Length];
		double * trackingP = new double[trackingE->Length-1];
		lien->trackingError(pastP, t, H, pricetP, pocketP, trackingP,nbre);
		
		
		for (int i = 0; i < pricet->Length-1; i++) {
			pricet[i] = pricetP[i];
			pocket[i] = pocketP[i];
			trackingE[i] = trackingP[i];
		}
		pricet[pricet->Length - 1] = pricetP[pricet->Length - 1];
		pocket[pocket->Length - 1] = pocketP[pocket->Length - 1];
		


	}
	 
}

