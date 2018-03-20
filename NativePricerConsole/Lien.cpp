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

PnlVect * Lien::deltaEurostral(double * past, double t) {
	PnlMat * pastMat = pnl_mat_create_from_ptr(this->bs->size_, this->bs->size_, past);
	PnlVect * delta = pnl_vect_create(5);
	Mt->deltaEurostral(pastMat, t, delta);
	pnl_mat_free(&pastMat);
	return delta;	
}