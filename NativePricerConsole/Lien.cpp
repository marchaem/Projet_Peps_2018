#include "Lien.hpp"
#include "Eurostral100.hpp"

Lien::Lien(int size, double r, double *VarHis, double *spot, double *trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1) {
    // cr�ation du BlackScholes
	PnlMat *Varhispnl = pnl_mat_create_from_ptr(size, size, VarHis);
	PnlVect *spotpnl = pnl_vect_create_from_ptr(size, spot);
	PnlVect *trendpnl = pnl_vect_create_from_ptr(size, trend);
	BlackScholesModel* bs_ = new BlackScholesModel(size, r, Varhispnl, spotpnl, trendpnl);
	this->bs = bs_;

	//Cr�ation de l'eurostral
	PnlVect *lambdapnl = pnl_vect_create_from_ptr(size, lambdas1);
	Option *eurostral = new Eurostral100(strike, T1, nbTimeSteps1, size,lambdapnl);
	opt = eurostral;

	//Cr�ation du MonteCarlo

	MonteCarlo * mt_ = new MonteCarlo(fdStep, nbSamples, eurostral, bs_);
	Mt = mt_;

}

double Lien::PriceEurostral() {
	double prix, ic;
	Mt->priceEurostral(prix, ic);

	return prix;

}