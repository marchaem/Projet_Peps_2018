#include "Utilities.hpp"

Utilities::Utilities(){}

PnlMat* Utilities::getConstatationDates(const PnlMat* past, Option* opt, double t){

  //We copy all the data corresponding to the constatation dates occuring before t
  PnlVect vectInit = pnl_vect_wrap_mat_row(past, 0);
  PnlMat* res = pnl_mat_create_from_zero(1, past->n);
  pnl_mat_set_row(res, &vectInit, 0);

  int i = 1;
  while (t > (opt->T_ / opt->nbTimeSteps_) * i){
    PnlVect vect = pnl_vect_wrap_mat_row(past, floor(i * (past->m -1) / opt->nbTimeSteps_ ));
    pnl_mat_add_row(res, res->m, &vect);
    i++;
  }
  if (t!=0){
    PnlVect vectFin = pnl_vect_wrap_mat_row(past, floor(t*(opt->nbTimeSteps_ / opt->T_)*((past->m -1)/ opt->nbTimeSteps_)));
    pnl_mat_add_row(res, res->m, &vectFin);
  }

  return res;
}

void Utilities::getConstatationDatesFromZero(PnlMat* constDates, const PnlMat* past, Option* opt, double t){
  //the date corresponding to the last line of constDates must be inferior or equal to t

  PnlVect vectInit = pnl_vect_wrap_mat_row(past, 0);
  pnl_mat_set_row(constDates, &vectInit, 0);
  int initSizeMat = constDates->m;

  int i = 1;
  while (t > (opt->T_ / opt->nbTimeSteps_) * i){
    if (i == initSizeMat-1){
      PnlVect vect = pnl_vect_wrap_mat_row(past, floor(i * (past->m -1) / opt->nbTimeSteps_ ));
      pnl_mat_set_row(constDates, &vect, i);
    }
    if (i > initSizeMat) {
      PnlVect vect = pnl_vect_wrap_mat_row(past, floor(i * (past->m -1) / opt->nbTimeSteps_ ));
      pnl_mat_add_row(constDates, constDates->m, &vect);
    }
    i++;
  }
  if (t!=0){
    PnlVect vectFin = pnl_vect_wrap_mat_row(past, floor(t*(opt->nbTimeSteps_ / opt->T_)*((past->m -1)/ opt->nbTimeSteps_)));
    if (i < initSizeMat){
      pnl_mat_set_row(constDates, &vectFin, i);
    } else {
      pnl_mat_add_row(constDates, constDates->m, &vectFin);
    }
  }
}



