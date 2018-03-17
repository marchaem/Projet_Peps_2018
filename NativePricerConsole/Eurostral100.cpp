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
  //Path Contient La trajectoire des trois indices et deux taux de changes et seuls les indices interviennent dans le payoff
  PnlVect*  indice1 = pnl_vect_create(path->m);
  PnlVect* indice2 = pnl_vect_create(path->m);
  PnlVect* indice3 = pnl_vect_create(path->m);
  // On récupère ainsi les 3 indices
  pnl_mat_get_col(indice1,path,0);
  pnl_mat_get_col(indice2,path,1);
  pnl_mat_get_col(indice3,path,2);
  double payoff = 0.0 , init1, init2, init3, rent1, rent2, rent3, ma, mi, entre;
  // La performance à chaque semestre est comparé aux valeurs initiales : 
  init1 = pnl_vect_get(indice1,0);
  init2 = pnl_vect_get(indice2,0);
  init3 = pnl_vect_get(indice3,0);
  for ( int i = 0 ; i < (path->m -1) ; i++ ) {
    // calcul des rentabilités au semestre i pour chaque indice
    rent1 = (pnl_vect_get(indice1,i+1)-init1)/init1;
    rent2 = (pnl_vect_get(indice2,i+1)-init2)/init2;
    rent3 = (pnl_vect_get(indice3,i+1)-init3)/init3;
	// Tri TODO Meilleur tri ?
    ma = max(max(rent1,rent2),rent3);
    mi = min(min(rent1,rent2),rent3);
    if ( rent1 != ma && rent1 != mi ) {
      entre = rent1;
    } else if ( rent2 != ma && rent2 != mi ) {
      entre = rent2;
    } else {
      entre = rent3;
    }
	//Application des coefficients du produits
    payoff += 0.5*ma + 0.3*entre + 0.2*mi;
  }
  payoff /= 16 ;
  if ( payoff < 0 ) {
    return 1.0;
  }
  return 1.0+0.55*payoff;
}
