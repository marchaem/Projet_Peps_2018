#include <iostream>
#include <string>
#include <typeinfo>
#include<time.h>
#include "Option.hpp"
#include "QuantoOption.hpp"
#include "AsianOption.hpp"
#include "BasketOption.hpp"
#include "PerformanceOption.hpp"
#include "MonteCarlo.hpp"
#include "BlackScholesModel.hpp"
#include "Utilities.hpp"
#include "Eurostral100.hpp  "
#include "Pricer.hpp"
using namespace std;

Pricer::Pricer() {
}
double Pricer::zero() {
	return 0.0; 
	
}

void main() {
	Pricer * pricertest = new Pricer();
	cout << "on rentre ici " << pricertest->price() << endl;
	cout << "lol" << endl;
}

  double Pricer::price()
{
  try {
    Utilities u;
    
	// Eurostral 
    double T=8, r=0.1, strike=100.0, rho=0.1, fdStep=0.01;
	PnlMat* Covlog = pnl_mat_create_from_double(5, 5, 0.1);
	int size = 5, timestep = 16;
	PnlVect *spot = pnl_vect_create_from_double(size, 3000);
	pnl_vect_set(spot, 3, 1.0);
	pnl_vect_set(spot, 4, 1.0);
	PnlVect* lambdas = pnl_vect_create_from_double(size, 1.0 / 5);
	PnlVect*trend= pnl_vect_create_from_double(size, 1.0 / 5);
	// nombre de simulation montecarlo
	int n_samples = 500;
	//nombre de rebalancement ( tous les 7 jours )
	double H = 416;
    pnl_vect_set(trend,0,r);
    pnl_vect_set(trend,1,0.04-0.1*0.2*0.2);
    pnl_vect_set(trend,2,0.05-0.1*0.2*0.2);
    pnl_vect_set(trend,3,r-0.04);
    pnl_vect_set(trend,4,r-0.05);

	BlackScholesModel * Bs = new BlackScholesModel(size, r, Covlog, spot, trend);
	Eurostral100* eurostral = new Eurostral100(strike, T, timestep, size, lambdas);
	MonteCarlo* mt = new MonteCarlo(fdStep, n_samples, eurostral, Bs);

	//Création d'une trajectoire toute la durée tous les 7 jours 
	PnlMat* trajectoire = pnl_mat_create(H + 1,size);
	Bs->assetEurostral(trajectoire, T, H, mt->rng_);
	

	PnlVect *V = pnl_vect_create_from_zero(H + 1);
	PnlVect *delta = pnl_vect_create_from_zero(size);

	double p0 = mt->PL_init_Eurostral(trajectoire, delta);
	cout << p0 << endl;
	PnlMat * past = pnl_mat_create_from_zero(5, 5);
	for (int i = 0; i < 5; i++) {
		for (int j = 0; j < 5; j++) {
			pnl_mat_set(past, i, j, pow(-1, i) * 0.01*i + pnl_vect_get(spot, j));
		}

	}
	double t = 0.54757015;
	PnlVect* price = pnl_vect_create_from_double(5,0.0);
	PnlVect *pocket = pnl_vect_create(5);
	PnlVect *TrackingE = pnl_vect_create(5);

	PnlMat * pastConst = pnl_mat_create(2, 5);
	
	
	

	 mt->tracking_error(past,pastConst, t, H, price, pocket, TrackingE);
	//pnl_vect_print(price);
	//pnl_vect_print(pocket);
	//pnl_vect_print(TrackingE);
	
	pnl_vect_free(&spot);
	pnl_vect_free(&lambdas);
	pnl_vect_free(&V);
	return 1.0;
	
	/*

    //Compute the price and display the result

   // if (argc == 2){
	
      double initialPrice, initialConf;
      PnlMat* past = pnl_mat_create(1, spot->size);
      pnl_mat_set_row(past, spot, 0);
      PnlVect* deltas = pnl_vect_create_from_zero(bsModel.size_);

      clock_t startPrice;
      startPrice = clock();
      if ( optionType == "eurostral") {
	mc.priceEurostral(initialPrice, initialConf);
	cout << initialPrice << endl;
      } else {
	mc.price(initialPrice, initialConf);
      }
      clock_t endPrice = (clock() - startPrice) / (double)(CLOCKS_PER_SEC/1000);

      clock_t startDelta;
      startDelta = clock();
      if ( optionType == "eurostral") {
	mc.deltaEurostral(past,0,deltas);
      } else {
	mc.delta(past, 0, deltas);
      }
      clock_t endDelta = (clock() - startDelta) / (double)(CLOCKS_PER_SEC/1000);
      if (optionType=="quanto" || optionType=="basket"){
          cout << "\nPrice at t=0 with formule fermé: " << opt->exactPrice0(&bsModel) << endl;
      }
	  cout << initialPrice << endl;


      return initialPrice;
      //cout << "\nPrice at t=0: " << initialPrice << endl;
      //cout << "IC: " << initialConf << endl;
      //cout << "Confidence interval at t=0: [" << initialPrice - initialConf << " ; " << initialPrice + initialConf << "]" << endl;
      //cout << "Time for price: " << endPrice << " ms\n" << endl;
      //cout << "Deltas at t=0: " <<endl;
      //pnl_vect_print_asrow(deltas);
      //cout << "Time for delta: " << endDelta << " ms\n" << endl;

      //Free the memory
      pnl_mat_free(&past);
      pnl_vect_free(&deltas);

    /*} else if (argc == 4) {
      double pl;
      char *infile = argv[2];
      PnlMat* past = pnl_mat_create_from_file(infile);

      clock_t startPL;
      startPL = clock();
      if ( optionType == "eurostral") {
            mc.profitLoss_Eurostral(past, past->m - 1, pl);
      } else {
      mc.profitLoss(past, past->m - 1, pl);
  }
    clock_t endPL = (clock() - startPL) / (double)(CLOCKS_PER_SEC/1000);
      cout << "\nP&L : " << pl << endl;
      cout << "Time for P&L: " << endPL << " ms\n" << endl;

      //Free the memory
      pnl_mat_free(&past);

    } else if (argc == 6){
      double price, conf,pl;
	  double t = 1;
      char *infile = argv[4];
      PnlMat* past = pnl_mat_create_from_file(infile);
      PnlMat* pastAtConstatation = u.getConstatationDates(past, mc.opt_, t);
      PnlVect* deltas = pnl_vect_create_from_zero(bsModel.size_);

      clock_t startPL;
      startPL = clock();
      if (optionType=="eurostral"){
          mc.profitLoss_Eurostral(past, past->m - 1, pl);
      }
      else {
          mc.profitLoss(past, past->m - 1, pl);
      }
      clock_t endPL = (clock() - startPL) / (double)(CLOCKS_PER_SEC/1000);

      cout << "\nP&L : " << pl << endl;
      cout << "Time for P&L: " << endPL << " ms\n" << endl;
      if (optionType=="eurostral"){
          mc.priceEurostral(pastAtConstatation, t, price, conf);
          mc.deltaEurostral(pastAtConstatation, t, deltas);
      }
      else{
          mc.price(pastAtConstatation, t, price, conf);
          mc.delta(pastAtConstatation, t, deltas);
      }

      cout << "\nPrice at t= " << t << ": " << price << endl;
      cout << "IC: " << conf << endl;
      cout << "Confidence interval at t=" << t << ": [" << price - conf << " ; " << price + conf << "]" << endl;
      cout << "Deltas at t=" <<  t << ": " <<endl;
      pnl_vect_print_asrow(deltas);
      cout << endl;

      //Free the memory
      pnl_mat_free(&past);
      pnl_mat_free(&pastAtConstatation);
      pnl_vect_free(&deltas);
	  
    } */

    //Free the memory
    
  } catch(string const& e) {
    cout << e << endl;
  }
}
