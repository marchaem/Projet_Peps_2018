#include "Lien.hpp"
#include "Eurostral100.hpp"

Lien::Lien(int size, double r, double *VarHis, double *spot, double *trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1) {
    // création du BlackScholes
	PnlMat *Varhispnl = pnl_mat_create_from_ptr(size, size, VarHis);
	PnlVect *spotpnl = pnl_vect_create_from_ptr(size, spot);
	PnlVect *trendpnl = pnl_vect_create_from_ptr(size, trend);
	BlackScholesModel* bs_ = new BlackScholesModel(size, r, Varhispnl, spotpnl, trendpnl);
	this->bs = bs_;

	//Création de l'eurostral
	PnlVect *lambdapnl = pnl_vect_create_from_ptr(size, lambdas1);
	Option *eurostral = new Eurostral100(strike, T1, nbTimeSteps1, size,lambdapnl);
	opt = eurostral;

	//Création du MonteCarlo

	MonteCarlo * mt_ = new MonteCarlo(fdStep, nbSamples, eurostral, bs_);

}
double Lien::PriceEurostral() {
	double prix;
	double ic;
	Mt->priceEurostral( prix, ic);
	return prix;
}
double Lien::PriceEurostral(double *past, double t) {
	double prix;
	double ic;
	PnlMat* PastPnl = pnl_mat_create_from_ptr( opt->nbTimeSteps_,bs->size_, past);
	Mt->priceEurostral(PastPnl, t, prix, ic);
	return prix;
}
//todo on peut trouver int Time à partir de t
double* Lien::deltaEurostral(double* past, double t,int Time){
	PnlVect* Delta = pnl_vect_create(bs->size_);
	PnlMat* pastpnl = pnl_mat_create_from_ptr( Time,bs->size_,past);
	Mt->deltaEurostral(pastpnl, t, Delta);
	double* Deltadouble;
	for (int i = 0; i < Time; i++) {
		Deltadouble[i] = pnl_vect_get(Delta, i);
	}
	return Deltadouble;
}