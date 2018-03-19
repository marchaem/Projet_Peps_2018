#pragma once
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include "MonteCarlo.hpp"

class Lien {
public:
	MonteCarlo * Mt;
	BlackScholesModel *bs;
	Option * opt;

		/*Constructeur*/
		Lien(int size, double r, double *VarHis, double *spot, double *trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1);
        /*Transformation de pnl en pointeur double */

		/*Prix en 0*/
		double PriceEurostral();

		/*Prix */
		double PriceEurostral(double *past, double t);

		/*delta*/
		double* deltaEurostral(double *past, double t,int Time);

		/*P&l final*/
		double profitLoss_Eurostral(const PnlMat *past, double H);

};