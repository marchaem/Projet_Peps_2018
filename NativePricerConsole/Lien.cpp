#include "Lien.hpp"
#include "Eurostral100.hpp"

Lien::Lien(int size, double r, double *VarHis, double *spot, double *trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1) {
    // création du BlackScholes
	PnlMat *Varhispnl = pnl_mat_create_from_ptr(size, size, VarHis);
	PnlVect *spotpnl = pnl_vect_create_from_ptr(size, spot);
	PnlVect *trendpnl = pnl_vect_create_from_ptr(size, trend);
	bs = new BlackScholesModel(size, r, Varhispnl, spotpnl, trendpnl);

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

double * Lien::deltaEurostral(double * past, double t,double H) {
	int nbdate = 0;
	if (t / H - floor(t / H) == 0) {
		nbdate = floor(t / H);
	}
	else {
		nbdate = floor(t / H) + 1;
	}
	PnlMat * pastMat = pnl_mat_create_from_ptr(this->bs->size_, nbdate, past);
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
	PnlMat* PastPnl = pnl_mat_create_from_ptr(opt->nbTimeSteps_, bs->size_, past);
	Mt->priceEurostral(PastPnl, t, prix, ic);
	return prix;
}
//todo on peut trouver int Time à partir de t
