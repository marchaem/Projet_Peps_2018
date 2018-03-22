#include "Lien.hpp"
#include "Eurostral100.hpp"

Lien::Lien(int size, double r, double *CovLogR, double *spot,double * trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1) {
    // création du BlackScholes
	PnlMat *CovlogRpnl = pnl_mat_create_from_ptr(size, size,CovLogR);
	PnlVect *spotpnl = pnl_vect_create_from_ptr(size, spot);
	PnlVect * trendpnl = pnl_vect_create_from_ptr(size, trend);
	bs = new BlackScholesModel(size,r,CovlogRpnl,spotpnl,trendpnl);

	//Création de l'eurostral
	PnlVect *lambdapnl = pnl_vect_create_from_ptr(size, lambdas1);
	Option *eurostral = new Eurostral100(strike, T1, nbTimeSteps1, size,lambdapnl);
	opt = eurostral;

	//Création du MonteCarlo

	Mt = new MonteCarlo(fdStep, nbSamples, eurostral, bs);
	

}

double Lien::PriceEurostral() {
	double prix, ic;
	Mt->priceEurostral(prix,ic);
	return prix;

}

double* Lien::deltaEurostral(double * past, double t,double H) {
	int nbdate = 1;
	if (t / opt->nbTimeSteps_ - floor(t / opt->nbTimeSteps_) == 0) {
		if (t - floor(t) < 0.5) {
			nbdate += 2 * floor(t);
		}
		else {
			nbdate += 2 * floor(t) + 1;
		}
	}
	else {

		if (t - floor(t) < 0.5) {
			nbdate += 2 * floor(t) + 1;
		}
		else {
			nbdate += 2 * floor(t) + 2;
		}
	}
	
	PnlMat * pastMat = pnl_mat_create_from_ptr(nbdate, bs->size_, past);
	PnlVect * delta = pnl_vect_create(bs->size_);
	
	Mt->deltaEurostral(pastMat, t, delta);
	
	pnl_mat_free(&pastMat);
	
	double* deltabis = new double[bs->size_];
	for (int i = 0; i < delta->size - 1; i++) {
		deltabis[i] = pnl_vect_get(delta, i);
	}
	
	return deltabis;	
}
double Lien::PriceEurostral(double *past, double t) {
	
	double prix;
	double ic;
	int partieEntiere = floor(t);
	int nbdate = 2 + 2 * partieEntiere + floor(2 * (t - partieEntiere));
	if (t - partieEntiere == 0 || t - partieEntiere == 0.5) {
		nbdate--;
	}

	return nbdate;
	PnlMat* PastPnl = pnl_mat_create_from_ptr(nbdate, bs->size_, past);
	
	Mt->priceEurostral(PastPnl, t, prix, ic);
	
	return prix;
	
}

double Lien::profitLoss_Eurostral(double * past, double H) {
	int rebalanc = floor(opt->T_ / H);
	double pl;
	PnlMat* pastpnl = pnl_mat_create_from_ptr(rebalanc, bs->size_, past);
	Mt->profitLoss_Eurostral(pastpnl, H, pl);
	return pl;
}

