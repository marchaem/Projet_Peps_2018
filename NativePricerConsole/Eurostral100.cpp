#include "Eurostral100.hpp"
#include <iostream>
#include <cstdio>
#include <algorithm>

using namespace std;

Eurostral100::Eurostral100(double strike, double T1,int nbTimeSteps1,int size1,PnlVect *lambdas1)
  :Option(T1, nbTimeSteps1, size1,lambdas1)
{
  K_ = strike;
}

Eurostral100::Eurostral100()
  :Option()
{}
double Eurostral100::exactPrice0(BlackScholesModel *b){
    return 0.0;
}

Eurostral100::~Eurostral100()
{}

double Eurostral100::payoff(const PnlMat *path){
  PnlVect* indice1 = pnl_vect_create(path->m);
  PnlVect* indice2 = pnl_vect_create(path->m);
  PnlVect* indice3 = pnl_vect_create(path->m);

  pnl_mat_get_col(indice1,path,0);
  pnl_mat_get_col(indice2,path,1);
  pnl_mat_get_col(indice3,path,2);

  double payoff = 0.0 , init1, init2, init3, cour1, cour2, cour3;

  init1 = pnl_vect_get(indice1,0);
  init2 = pnl_vect_get(indice2,0);
  init3 = pnl_vect_get(indice3,0);

  for ( int i = 0 ; i < path->m -1 ; i++ ) {
    cour1 = pnl_vect_get(indice1,i+1)/init1 - 1.0;
    cour2 = pnl_vect_get(indice2,i+1)/init2 - 1.0;
    cour3 = pnl_vect_get(indice3,i+1)/init3 - 1.0;

	double tmp = 0.0;
	while (cour1 > cour3 || cour1 > cour2 || cour2 > cour3) {
		if (cour1 > cour2) {
			tmp = cour1;
			cour1 = cour2;
			cour2 = tmp;
		}
		if (cour2 > cour3) {
			tmp = cour2;
			cour2 = cour3;
			cour3 = tmp;
		}
	}
   
    payoff += 0.5*cour3 + 0.3*cour2 + 0.2*cour1;
  }

  payoff /= 16 ;
  pnl_vect_free(&indice1);
  pnl_vect_free(&indice2);
  pnl_vect_free(&indice3);
  if ( payoff < 0 ) {
    return 150.0;
  }

  return 150*(1.0+0.55*payoff);
}
