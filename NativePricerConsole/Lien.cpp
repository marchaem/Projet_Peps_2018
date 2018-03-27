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
	Mt = new MonteCarlo(fdStep, nbSamples, eurostral, bs);

	//Création du MonteCarlo

	

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
	for (int i = 0; i < delta->size; i++) {
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

double Lien::profitLoss_Eurostral(double * past, double H) {
	int rebalanc = floor(H);
	double pl;

	PnlMat * pastpnl = pnl_mat_create_from_ptr(rebalanc + 1, bs->size_,past);
	
	Mt->profitLoss_Eurostral(pastpnl, H, pl);
	
	return pl;
	
}

double Lien::forwardTest(double H) {
	double pl;
	PnlMat * past = pnl_mat_create_from_double(floor(H) + 1, 5,5.0);
	bs->assetEurostral(past, 8.0, 416, Mt->rng_);
	Mt->profitLoss_Eurostral(past, H, pl);
	return pl;
}
void Lien::trackingError(double * past, double t, double H, double * pricet, double * pocket, double * trackingE,int nbre) {
	/*double weeks = (H / opt->T_) *t;
	int fweeks = floor(weeks);
	int nbdate;
	if (fweeks == weeks) {
		nbdate = 1 + fweeks;
	}
	else {
		nbdate = 2 + fweeks;
	}*/
	
	PnlMat* pastpnl = pnl_mat_create_from_ptr(nbre, bs->size_, past);
	PnlVect* pricetpnl = pnl_vect_create(nbre);
	PnlVect* pocketpnl = pnl_vect_create(nbre);
	PnlVect * trackEpnl = pnl_vect_create(nbre-1);
	Mt->tracking_error(pastpnl, t, H, pricetpnl, pocketpnl, trackEpnl);
	for (int i = 0; i< nbre-1; i++) {
		pricet[i] = pnl_vect_get(pricetpnl, i);
		pocket[i] = pnl_vect_get(pocketpnl, i);
		trackingE[i] = pnl_vect_get(trackEpnl, i);

	}
	pricet[nbre-1] = pnl_vect_get(pricetpnl, nbre -1);
	pocket[nbre-1] = pnl_vect_get(pocketpnl, nbre-1);
	
}

