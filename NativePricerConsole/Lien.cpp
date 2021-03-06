#include "Lien.hpp"
#include "Eurostral100.hpp"

Lien::Lien(int size, double r, double *CovLogR, double *spot,double * trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1) {
    // cr�ation du BlackScholes
	PnlMat *CovlogRpnl = pnl_mat_create_from_ptr(size, size,CovLogR);
	PnlVect *spotpnl = pnl_vect_create_from_ptr(size, spot);
	PnlVect * trendpnl = pnl_vect_create_from_ptr(size, trend);
	bs = new BlackScholesModel(size,r,CovlogRpnl,spotpnl,trendpnl);

	//Cr�ation de l'eurostral
	PnlVect *lambdapnl = pnl_vect_create_from_ptr(size, lambdas1);
	Option *eurostral = new Eurostral100(strike, T1, nbTimeSteps1, size,lambdapnl);
	opt = eurostral;
	Mt = new MonteCarlo(fdStep, nbSamples, eurostral, bs);

	//Cr�ation du MonteCarlo

	

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
void Lien::trackingError(double * past,double * pastConst, double t, double H, double * pricet, double * pocket, double * trackingE, double * V,int nbre,int nbConst) {
	

	PnlMat* pastpnl = pnl_mat_create_from_ptr(nbre, bs->size_, past);
	PnlMat* pastConstpnl = pnl_mat_create_from_ptr(nbConst, bs->size_, pastConst);
	PnlVect* pricetpnl = pnl_vect_create(nbre);
	PnlVect* pocketpnl = pnl_vect_create(nbre);
	PnlVect* vpnl = pnl_vect_create(nbre);
	PnlVect * trackEpnl = pnl_vect_create(nbre-1);
	Mt->tracking_error(pastpnl,pastConstpnl, t, H, pricetpnl, pocketpnl, trackEpnl,vpnl);
	for (int i = 0; i< nbre-1; i++) {
		pricet[i] = pnl_vect_get(pricetpnl, i);
		pocket[i] = pnl_vect_get(pocketpnl, i);
		trackingE[i] = pnl_vect_get(trackEpnl, i);
		V[i] = pnl_vect_get(vpnl, i);

	}
	pricet[nbre-1] = pnl_vect_get(pricetpnl, nbre -1);
	pocket[nbre-1] = pnl_vect_get(pocketpnl, nbre-1);
	V[nbre - 1] = pnl_vect_get(vpnl, nbre - 1);
	pnl_mat_free(&pastpnl);
	pnl_mat_free(&pastConstpnl);
	pnl_vect_free(&pricetpnl);
	pnl_vect_free(&pocketpnl);
	pnl_vect_free(&trackEpnl);
	
}

