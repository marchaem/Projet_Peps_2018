#pragma once
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include "MonteCarlo.hpp"
#define DLLEXP __declspec(dllexport)

DLLEXP class Lien {
public:
	MonteCarlo * Mt;
	BlackScholesModel *bs;
	Option * opt;

		/*Constructeur*/
	DLLEXP	Lien(int size, double r, double *CovLogR, double *spot, double * trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double *lambdas1);
        /*Transformation de pnl en pointeur double */

		/*Prix en 0*/
	DLLEXP	double PriceEurostral();

		/*Prix */
	DLLEXP	double PriceEurostral(double *past, double t);

		/*delta*/
	DLLEXP	double* deltaEurostral(double *past, double t,double H);

		/*P&l final*/
	DLLEXP	double profitLoss_Eurostral(double *past, double t, double H);

};