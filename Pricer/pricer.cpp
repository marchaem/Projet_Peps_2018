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
#include "Eurostral100.hpp"
#include "pricer.hpp"

using namespace std;

double pricerOption::pricer::zero() {
	return 0.0;
}

  double pricerOption::pricer::price()
{
  try {
    Utilities u;
    //Check the number and the value of the parameters, and return the input file with the option description
    //Parse the input file
    double T=8, r=0.1, strike=100.0, rho=0.1, fdStep=0.01;
	int size = 5, timestep = 1;
    PnlVect *spot=pnl_vect_create_from_double(size,100), *sigma= pnl_vect_create_from_double(size, 1.0/5), *divid= pnl_vect_create_from_double(size, 1.0 / 5), *lambdas= pnl_vect_create_from_double(size, 1.0 / 5), *trend= pnl_vect_create_from_double(size, 1.0 / 5);
	string optionType = "eurostral";
    
    size_t n_samples=5;


    //trend = pnl_vect_create(5) ;
    /*trend = pnl_vect_create(5);
    pnl_vect_set(trend,0,r);
    pnl_vect_set(trend,1,0.04-0.1*0.2*0.2);
    pnl_vect_set(trend,2,0.05-0.1*0.2*0.2);
    pnl_vect_set(trend,3,r-0.04);
    pnl_vect_set(trend,4,r-0.05);*/

    //Create the Option, the BlackScholesModel and the MonteCarlo instances
    //BlackScholesModel bsModel(size, r, rho, sigma, spot);
    BlackScholesModel bsModel(size, r, rho, sigma, spot,trend);
    Option *opt;
    if (optionType == "asian"){
      opt = new AsianOption(strike, T, timestep, size, lambdas);
    } else if (optionType == "basket"){
      opt = new BasketOption(strike, T, timestep, size, lambdas);
    } else if (optionType == "performance"){
      opt = new PerformanceOption(T, timestep, size, lambdas);
    } else if (optionType == "quanto" ) {
      opt = new QuantoOption(strike,T,timestep,size,lambdas);
    } else if (optionType == "eurostral") {
      opt = new Eurostral100(strike,T,timestep,size,lambdas);
    }else {
      throw string("Unimplemented type of financial product: must be 'asian', 'basket', or 'performance' or 'quanto'");
    }
    MonteCarlo mc(fdStep, n_samples, opt, &bsModel);

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
    pnl_vect_free(&spot);
    pnl_vect_free(&sigma);
    pnl_vect_free(&lambdas);
    pnl_vect_free(&divid);
    delete opt;
  } catch(string const& e) {
    cout << e << endl;
  }
}
