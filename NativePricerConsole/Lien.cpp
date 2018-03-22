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
	int partieEntiere = floor(t);
	int nbdate = 2 + 2 * partieEntiere + floor(2 * (t - partieEntiere));
	if (t - partieEntiere == 0 || t - partieEntiere == 0.5) {
		nbdate--;
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

	PnlMat* PastPnl = pnl_mat_create_from_ptr(nbdate, bs->size_, past);
	
	Mt->priceEurostral(PastPnl, t, prix, ic);
	
	return prix;
	
}

double Lien::profitLoss_Eurostral(double * past, double t, double H) {
	int rebalanc = floor(H);
	double pl;

	PnlMat* pastpnl = pnl_mat_create_from_ptr(bs->size_,rebalanc+1, past);
	PnlMat * pastpnl2 = pnl_mat_create(rebalanc + 1, bs->size_);
	pastpnl2 = pnl_mat_transpose(pastpnl);
	//return pnl_mat_get(pastpnl2,150,0);
	
	double pas = (double)opt->T_ / H;
	int compteur = floor(t / pas);
	if ((t / pas - floor(t / pas) > 0.001 && t / pas - floor(t / pas) < 0.999)) {
		compteur--;
	}
	//return pnl_mat_get(pastpnl2, 100, 0);
	int test = pnl_mat_resize(pastpnl2, compteur, bs->size_);
	//return pnl_mat_get(pastpnl2, 100, 0);
	PnlMat* donnees = pnl_mat_create_from_scalar(rebalanc + 1, bs->size_,2);
	//return pnl_mat_get(donnees, 0, 0);
	PnlMat* pasttt = pnl_mat_create_from_scalar(1, 5, 15);
	bs->assetEurostral(donnees, t, opt->T_, 16, Mt->rng_, pasttt);
	return pnl_mat_get(donnees, 0, 0);
	Mt->profitLoss_Eurostral(donnees, H, pl);
	return pnl_mat_get(donnees,100, 0);
	
}

