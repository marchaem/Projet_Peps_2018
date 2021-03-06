//
#include <iostream>
#include <string>
#include <time.h>
#include <typeinfo>
#include "MonteCarlo.hpp"
#include <omp.h>

using namespace std ;

MonteCarlo::MonteCarlo()
{
    rng_ = NULL;
    opt_ = NULL;
    mod_ = NULL;
    fdStep_ = 0.1;
    nbSamples_ = 0;
}

MonteCarlo::MonteCarlo(double fdStep, int nbSamples):
MonteCarlo()
{
    rng_ = pnl_rng_create(PNL_RNG_MERSENNE);
    pnl_rng_sseed(rng_, time(NULL));
    fdStep_ = fdStep;
    nbSamples_ = nbSamples;
}

MonteCarlo::MonteCarlo(double fdStep, int nbSamples, Option *opt, BlackScholesModel *mod):
MonteCarlo(fdStep, nbSamples)
{
    opt_ = opt;
    mod_ = mod;
}

MonteCarlo::~MonteCarlo()
{
    pnl_rng_free(&rng_);
}

void MonteCarlo::price(double &prix, double &ic)
{
    PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
    double sum = 0, sq_sum = 0, payoff_, cf_act_ = actualisation(0.0);
//
// reduction on two variables
// firstprivate to make a copy of each object
//
// #pragma omp parallel for reduction(+:sum, sq_sum) firstprivate(path_, mod_, opt_, rng_)

    for(int M = 1; M <= nbSamples_; M++)
    {
      mod_->asset(path_, opt_->T_, opt_->nbTimeSteps_, rng_);
      payoff_ = opt_->payoff(path_);
      sum += payoff_;
      sq_sum += payoff_ * payoff_;
    }
    prix = cf_act_ * sum;
    ic = 1.96 * cf_act_ * sqrt(sq_sum - pow(sum, 2) / nbSamples_);
    pnl_mat_free(&path_);
}

void MonteCarlo::priceEurostral(double &prix, double &ic)
{
    PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
    double sum = 0, sq_sum = 0, payoff_, cf_act_ = actualisation(0.0);
//
// reduction on two variables
// firstprivate to make a copy of each object
//
// #pragma omp parallel for reduction(+:sum, sq_sum) firstprivate(path_, mod_, opt_, rng_)

    for(int M = 1; M <= nbSamples_; M++)
    {
      mod_->assetEurostral(path_, opt_->T_, opt_->nbTimeSteps_, rng_);
      payoff_ = opt_->payoff(path_);
      sum += payoff_;
      sq_sum += payoff_ * payoff_;
    }
    prix = cf_act_ * sum;
	cout << "le prix dans priceEurostral vaut " << prix << endl;
    ic = 1.96 * cf_act_ * sqrt(sq_sum - pow(sum, 2) / nbSamples_);
    pnl_mat_free(&path_);
}

void MonteCarlo::price(const PnlMat *past, double t, double &prix, double &ic)
{
  if (t> opt_->T_){
    throw std::string("t is greater than the maturity");
  }
  PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
  double sum = 0, sq_sum = 0, payoff_, cf_act_ = actualisation(t);
  PnlVect *vect=pnl_vect_create_from_zero(2);
  PnlVect *pathb=pnl_vect_create_from_zero(2);
  for(int M = 1; M <= nbSamples_; M++)
  {
    mod_->asset(path_, t, opt_->T_, opt_->nbTimeSteps_, rng_, past);
    //pnl_mat_print(path_);
    payoff_ = opt_->payoff(path_);
    pnl_mat_get_row(pathb,path_,1);
    pnl_vect_plus_vect(vect,pathb);
	  sum += payoff_;
    sq_sum += payoff_ * payoff_;
  }

  //pnl_vect_print(vect);
  prix = cf_act_ * sum;
  ic = 1.96 * cf_act_ * sqrt(sq_sum - pow(sum, 2) / nbSamples_);
  pnl_mat_free(&path_);
}

void MonteCarlo::priceEurostral(const PnlMat *past, double t, double &prix, double &ic)
{
  if (t> opt_->T_){
    throw std::string("t is greater than the maturity");
  }
  PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
  double sum = 0, sq_sum = 0, payoff_, cf_act_ = actualisation(t);
  PnlVect *vect=pnl_vect_create_from_zero(2);
  PnlVect *pathb=pnl_vect_create_from_zero(2);
  for(int M = 1; M <= nbSamples_; M++)
  {
    mod_->assetEurostral(path_, t, opt_->T_, opt_->nbTimeSteps_, rng_, past);
    //pnl_mat_print(path_);
    payoff_ = opt_->payoff(path_);
    pnl_mat_get_row(pathb,path_,1);
    pnl_vect_plus_vect(vect,pathb);
	  sum += payoff_;
    sq_sum += payoff_ * payoff_;
  }

  //pnl_vect_print(vect);
  prix = cf_act_ * sum;
  ic = 1.96 * cf_act_ * sqrt(sq_sum - pow(sum, 2) / nbSamples_);
  pnl_mat_free(&path_);
}

void MonteCarlo::delta(const PnlMat *past, double t, PnlVect *delta){
    if (t> opt_->T_)
      throw std::string("t is greater than the maturity");

    PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
    PnlMat *path_Plus = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
    PnlMat *path_Minus = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);

    double sum = 0, payoff_ = 0.0;

    for (int d = 0; d < mod_->size_; d++)
    {
      for(int M = 1; M <= nbSamples_; M++)
      {
        mod_->asset(path_, t, opt_->T_, opt_->nbTimeSteps_, rng_, past);
        mod_->shiftAsset(path_Plus, path_, d, fdStep_ ,t, opt_->T_ / opt_->nbTimeSteps_ );
        mod_->shiftAsset(path_Minus, path_, d, -fdStep_ ,t, opt_->T_ / opt_->nbTimeSteps_ );
        payoff_ = opt_->payoff(path_Plus) - opt_->payoff(path_Minus);
        sum += payoff_;
      }
      sum = sum * actualisation(t) * (1/ (2* fdStep_ *MGET(past,past->m -1,d)));
      pnl_vect_set(delta,d,sum);
    }


    double tmp;
    //cout << typeid(opt_).name() << endl;
    pnl_mat_free(&path_);
    pnl_mat_free(&path_Plus);
    pnl_mat_free(&path_Minus);
}

void MonteCarlo::deltaEurostral(const PnlMat *past, double t, PnlVect *delta){
    if (t> opt_->T_)
      throw std::string("t is greater than the maturity");
	int d;
//#pragma omp parallel
	//{
		//PnlRng *rng = pnl_rng_dcmt_create_id(omp_get_thread_num(), 1);
		//pnl_rng_sseed(rng, time(NULL));
		PnlMat *path_ = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
		PnlMat *path_Plus = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);
		PnlMat *path_Minus = pnl_mat_create_from_zero(opt_->nbTimeSteps_ + 1, mod_->size_);

		double sum = 0, payoff_ = 0.0;
//#pragma omp for private(d,payoff_,past)
		for ( d = 0; d < mod_->size_; d++)
		{
			for (int M = 1; M <= nbSamples_; M++)
			{
				mod_->assetEurostral(path_, t, opt_->T_, opt_->nbTimeSteps_, rng_, past);
				mod_->shiftAsset(path_Plus, path_, d, fdStep_, t, opt_->T_ / opt_->nbTimeSteps_);
				mod_->shiftAsset(path_Minus, path_, d, -fdStep_, t, opt_->T_ / opt_->nbTimeSteps_);
				payoff_ = opt_->payoff(path_Plus) - opt_->payoff(path_Minus);
				sum += payoff_;
			}
			sum = sum * actualisation(t) * (1 / (2 * fdStep_ *MGET(past, past->m - 1, d)));
			pnl_vect_set(delta, d, sum);
		}


		double tmp;
		//cout << typeid(opt_).name() << endl;
		tmp = pnl_vect_get(delta, 1) / pnl_mat_get(path_, 0, 3);
		pnl_vect_set(delta, 1, tmp);
		tmp = pnl_vect_get(delta, 2) / pnl_mat_get(path_, 0, 4);
		pnl_vect_set(delta, 2, tmp);
		tmp = exp(pnl_vect_get(mod_->trends_, 1)*opt_->T_)*(pnl_mat_get(path_, 0, 1)*pnl_vect_get(delta, 1) - pnl_vect_get(delta, 3));
		pnl_vect_set(delta, 3, tmp);
		tmp = exp(pnl_vect_get(mod_->trends_, 2)*opt_->T_)*(pnl_mat_get(path_, 0, 2)*pnl_vect_get(delta, 2) - pnl_vect_get(delta, 4));
		pnl_vect_set(delta, 4, tmp);

		pnl_mat_free(&path_);
		pnl_mat_free(&path_Plus);
		pnl_mat_free(&path_Minus);
	//}
}

double MonteCarlo::PL_init(const PnlMat *past, PnlVect *delta)
{
    PnlVect S = pnl_vect_wrap_mat_row(past, 0);
    PnlMat *currPast = pnl_mat_create_from_zero(1, past->n);
    double p, ic;

    pnl_mat_extract_subblock(currPast, past, 0, 1, 0, past->n);

    this->delta(currPast, 0.0, delta);
    this->price(p, ic);
    p -= pnl_vect_scalar_prod(delta, &S);

    pnl_mat_free(&currPast);
    return(p);
}

double MonteCarlo::PL_init_Eurostral(const PnlMat *past, PnlVect *delta){
    PnlVect *S =pnl_vect_create_from_zero(past->n);
    pnl_mat_get_row(S,past, 0);
    PnlMat *currPast = pnl_mat_create_from_zero(1, past->n);
    double p, ic;
    double r_dollar= -1*pnl_vect_get(mod_->trends_,3) + mod_->r_;
    double r_aus= -1*pnl_vect_get(mod_->trends_,4) + mod_->r_;
    pnl_mat_extract_subblock(currPast, past, 0, 1, 0, past->n);

    this->deltaEurostral(currPast, 0.0, delta);
    this->price(p, ic);
    //p -= pnl_vect_scalar_prod(delta, &S);
    p-= pnl_vect_get(delta,0)*pnl_vect_get(S,0);
    p-=pnl_vect_get(delta,1)*pnl_vect_get(S,1)*pnl_vect_get(S,3);
    p-= pnl_vect_get(delta,2)*pnl_vect_get(S,2)*pnl_vect_get(S,4);
    p-=pnl_vect_get(delta,3)*exp(-(opt_->T_* r_dollar))*pnl_vect_get(S,3);
    p-=pnl_vect_get(delta,4)*exp(-(opt_->T_* r_aus))*pnl_vect_get(S,4);


    pnl_mat_free(&currPast);
    return(p);
}


void MonteCarlo::PL_build_V(const PnlMat *past, double H,
                            PnlVect *delta, PnlVect *V)
{
    pnl_vect_set(V, 0, PL_init(past, delta));

    double expCoef = exp(mod_->r_ * opt_->T_/ H), tho;
    PnlVect *prevDelta = pnl_vect_create_from_zero(past->n);
    PnlMat *cPast = pnl_mat_create_from_zero(1, past->n);

    pnl_vect_clone(prevDelta, delta);

    for(int i = 1; i < H; i++)
    {
        tho = i * opt_->T_ /H;
        u.getConstatationDatesFromZero(cPast, past, opt_, tho);
        this->delta(cPast, tho, delta);

        PnlVect S = pnl_vect_wrap_mat_row(cPast, cPast->m-1);

        pnl_vect_minus_vect(prevDelta, delta);

        pnl_vect_set(V, i,
                     pnl_vect_get(V, i-1) * expCoef +
                     pnl_vect_scalar_prod(prevDelta, &S)
                     );

        pnl_vect_clone(prevDelta, delta);
    }
    pnl_vect_free(&prevDelta);
    pnl_mat_free(&cPast);
}
void MonteCarlo::PL_build_V_Eurostral(const PnlMat *past, double H,PnlVect *delta, PnlVect *V){
    pnl_vect_set(V, 0, PL_init_Eurostral(past, delta));
	
    double expCoef = exp(mod_->r_ * opt_->T_/ H), tho,thomoins;
    PnlVect *prevDelta = pnl_vect_create_from_zero(past->n);
    PnlMat *cPast = pnl_mat_create_from_zero(1, past->n);
    double r_dollar= -1*pnl_vect_get(mod_->trends_,3) + mod_->r_;
    double r_aus= -1*pnl_vect_get(mod_->trends_,4) + mod_->r_;
    pnl_vect_clone(prevDelta, delta);
    double trackingErrror;
    double prix,ic;
    for(int i = 1; i < H; i++)
    {
        tho = i * opt_->T_ /H;
		thomoins = (i - 1)*opt_->T_ / H;
		
        u.getConstatationDatesFromZero(cPast, past, opt_, tho);
	
		cout << "matrice past " << endl;

		pnl_mat_print(cPast);
        this->deltaEurostral(cPast, tho, delta);

        PnlVect S = pnl_vect_wrap_mat_row(cPast, cPast->m-1);
        pnl_vect_set(&S,1,pnl_vect_get(&S,1)*pnl_vect_get(&S,3));
        pnl_vect_set(&S,2,pnl_vect_get(&S,2)*pnl_vect_get(&S,4));
        pnl_vect_set(&S,3,exp(-((opt_->T_-thomoins)* r_dollar))* pnl_vect_get(&S,3)) ;
        pnl_vect_set(&S,4,exp(-((opt_->T_-thomoins)* r_aus))* pnl_vect_get(&S,4)) ;

		

        this->priceEurostral(cPast,tho,prix,ic);
		trackingErrror = pnl_vect_scalar_prod(&S, prevDelta) + pnl_vect_get(V, i - 1)*expCoef - prix;
		cout << " dans le portefeuille Zc eu" << pnl_vect_get(V, i - 1) << endl;
		cout << "tracking error " << trackingErrror << endl;
	
        pnl_vect_minus_vect(prevDelta, delta);

        pnl_vect_set(V, i,
                     pnl_vect_get(V, i-1) * expCoef +
                     pnl_vect_scalar_prod(prevDelta, &S)
                     );

        pnl_vect_clone(prevDelta, delta);
    }
    pnl_vect_free(&prevDelta);
    pnl_mat_free(&cPast);
}
void MonteCarlo::PL_finalSet(const PnlMat *past, double v_h,
                             PnlVect *delta, double& pl)
{
    double p, ic;
    PnlMat* constDates = u.getConstatationDates(past, opt_, opt_->T_);
    PnlVect S = pnl_vect_wrap_mat_row(past, past->m-1);

    this->price(constDates, opt_->T_, p, ic);
    pl = v_h + pnl_vect_scalar_prod(delta, &S) - p;

    pnl_mat_free(&constDates);
}
void MonteCarlo::PL_finalSet_Eurostral(const PnlMat *past, double v_h, PnlVect *delta, double& pl,double H)
{
    double p, ic;
    PnlMat* constDates = u.getConstatationDates(past, opt_, opt_->T_);
    PnlVect S = pnl_vect_wrap_mat_row(past, past->m-1);
	double r_dollar = -1 * pnl_vect_get(mod_->trends_, 3) + mod_->r_;
	double r_aus = -1 * pnl_vect_get(mod_->trends_, 4) + mod_->r_;
    this->price(constDates, opt_->T_, p, ic);

    pnl_vect_set(&S,1,pnl_vect_get(&S,1)*pnl_vect_get(&S,3));
    pnl_vect_set(&S,2,pnl_vect_get(&S,2)*pnl_vect_get(&S,4));
	pnl_vect_set(&S, 3, exp(-((opt_->T_/H)* r_dollar))* pnl_vect_get(&S, 3));
	pnl_vect_set(&S, 4, exp(-((opt_->T_/H)* r_aus))* pnl_vect_get(&S, 4));
    pl = v_h + pnl_vect_scalar_prod(delta, &S) - p;

    pnl_mat_free(&constDates);
}


void MonteCarlo::profitLoss(const PnlMat *past, double H, double &pl)
{
    assert(past->m == H+1);
    PnlVect *V = pnl_vect_create_from_zero(H+1);
    PnlVect *delta = pnl_vect_create_from_zero(past->n);

    PL_build_V(past, H, delta, V);
    pnl_vect_set(V, H, pnl_vect_get(V, H-1) * exp(mod_->r_ * opt_->T_/ H));
    PL_finalSet(past, pnl_vect_get(V, H), delta, pl);

    pnl_vect_free(&V);
    pnl_vect_free(&delta);
}
void MonteCarlo::profitLoss_Eurostral(const PnlMat *past, double H, double &pl)
{
    assert(past->m == H+1);
    PnlVect *V = pnl_vect_create_from_zero(H+1);
    PnlVect *delta = pnl_vect_create_from_zero(past->n);
    PL_build_V_Eurostral(past, H, delta, V);
    pnl_vect_set(V, H, pnl_vect_get(V, H-1) * exp(mod_->r_ * opt_->T_/ H));
    PL_finalSet_Eurostral(past, pnl_vect_get(V, H), delta, pl,H);

    pnl_vect_free(&V);
    pnl_vect_free(&delta);
}

void MonteCarlo::forwardTestPL(double H, double &pl) {
	PnlMat * past = pnl_mat_create(H + 1, 5);
	mod_->assetEurostral(past, 8.0, 417, rng_);
	profitLoss_Eurostral(past, H, pl);
	pnl_mat_free(&past);

}
double MonteCarlo::actualisation(double t)
{
    return(exp(- mod_->r_ * (opt_->T_ - t)) / nbSamples_);
}

MonteCarlo& MonteCarlo::operator= (const MonteCarlo& mc)
{
    fdStep_ = mc.fdStep_;
    nbSamples_ = mc.nbSamples_;
    rng_ = mc.rng_;
    mod_ = mc.mod_;
    opt_ = mc.opt_;
    return(*this);
}

void MonteCarlo::set(double fdStep, int nbSamples, Option * opt, BlackScholesModel * mod)
{
	fdStep_ = fdStep;
	nbSamples_ = nbSamples;
	opt_ = opt;
	mod_ = mod;
}

void MonteCarlo::tracking_error(const PnlMat *past,const PnlMat* pastconst, double t, double H, PnlVect *Pricet, PnlVect* pocket, PnlVect* trackingE, PnlVect* V_) {
	PnlVect *V = pnl_vect_create_from_zero(past->m);
	PnlVect *delta = pnl_vect_create_from_zero(past->n);
	// on met dans le vecteur V la valeur investi dans le z�ro coupon europ�en 
	pnl_vect_set(V, 0, PL_init_Eurostral(past, delta));
	double prix,ic;
	// prix en 0
	priceEurostral(prix, ic);

	// initialisation
	pnl_vect_set(Pricet, 0, prix);
	pnl_vect_set(pocket, 0, prix);
	

	// facteur de capitalisation 
	double expCoef = exp(mod_->r_ * opt_->T_ / H), tho,thomoins;
	PnlVect *prevDelta = pnl_vect_create_from_zero(past->n);
	PnlMat *cPast = pnl_mat_create_from_zero(1, past->n);
	pnl_mat_print(cPast);

	// r�cup�ration des taux d'int�r�t �tranger
	double r_dollar = -1 * pnl_vect_get(mod_->trends_, 3) + mod_->r_;
	double r_aus = -1 * pnl_vect_get(mod_->trends_, 4) + mod_->r_;

	
	pnl_vect_clone(prevDelta, delta);
	
	for (int i = 1; i < past->m-1; i++)
	{
		// on boucle jusqu'� la date d'aujourd'hui
		tho = i * opt_->T_ / H;
		thomoins = (i - 1)*opt_->T_ / H;
		
		
		//u.getConstatationDatesFromZero(cPast, past, opt_, tho);
		
		
		// on r�cup�re la matrice historiques ne contenant que les dates de constations et la derni�re date
		this->getConstat(cPast, past,pastconst, H, i-1);

		cout << "matrice past " << endl;

		pnl_mat_print(cPast);

		//cout << "matrice past " << endl;

		
		this->priceEurostral(cPast, tho, prix, ic);
		pnl_vect_set(Pricet, i, prix);
		// calcul du nouveau nouveau vecteur delta
		this->deltaEurostral(cPast, tho, delta);
		

		PnlVect S = pnl_vect_wrap_mat_row(cPast, cPast->m - 1);

		// Impl�mentation des prix des actifs en euro
		pnl_vect_set(&S, 1, pnl_vect_get(&S, 1)*pnl_vect_get(&S, 3));
		pnl_vect_set(&S, 2, pnl_vect_get(&S, 2)*pnl_vect_get(&S, 4));
		pnl_vect_set(&S, 3, exp(-((opt_->T_ - tho)* r_dollar))* pnl_vect_get(&S, 3));
		pnl_vect_set(&S, 4, exp(-((opt_->T_ - tho)* r_aus))* pnl_vect_get(&S, 4));
		
		
		// calcul du nouveau prix
		

		// la tracking error correspond � la diff�rence du nouveau prix et la nouvelle valeur du portefeuille
		pnl_vect_set(pocket, i, pnl_vect_scalar_prod(&S, prevDelta) +pnl_vect_get(V, i - 1)*expCoef);
		
			
		pnl_vect_set(trackingE,i-1,pnl_vect_get(pocket,i)-prix);

		cout << pnl_vect_get(trackingE, i-1) << endl;
		
		pnl_vect_minus_vect(prevDelta, delta);

		pnl_vect_set(V, i,
			pnl_vect_get(V, i - 1) * expCoef +
			pnl_vect_scalar_prod(prevDelta, &S)
		);

		pnl_vect_clone(prevDelta, delta);
	}


	
	int n = pnl_mat_resize(cPast, cPast->m+1, opt_->size_);
	PnlVect S = pnl_vect_wrap_mat_row(past, past->m-1);
	pnl_mat_set_row(cPast, &S,cPast->m -1 );
	this->deltaEurostral(cPast, t, delta);
	this->priceEurostral(cPast, t, prix, ic);
	pnl_vect_set(&S, 1, pnl_vect_get(&S, 1)*pnl_vect_get(&S, 3));
	pnl_vect_set(&S, 2, pnl_vect_get(&S, 2)*pnl_vect_get(&S, 4));
	pnl_vect_set(&S, 3, exp(-((opt_->T_ - t)* r_dollar))* pnl_vect_get(&S, 3));
	pnl_vect_set(&S, 4, exp(-((opt_->T_ - t)* r_aus))* pnl_vect_get(&S, 4));
	pnl_vect_set(Pricet, Pricet->size - 1, prix);
	pnl_vect_set(pocket, pocket->size - 1, pnl_vect_scalar_prod(&S, prevDelta) + pnl_vect_get(V, V->size - 2)*expCoef);
	pnl_vect_set(trackingE, trackingE->size - 1, pnl_vect_get(pocket, pocket->size - 1) - prix);
	pnl_vect_minus_vect(prevDelta, delta);

	pnl_vect_set(V, past->m-1,
		pnl_vect_get(V, past->m-2) * expCoef +
		pnl_vect_scalar_prod(prevDelta, &S)
	);
	pnl_vect_clone(V_,V);



	pnl_vect_free(&prevDelta);
	pnl_vect_free(&V);
	pnl_mat_free(&cPast);

}

void MonteCarlo::getConstat(PnlMat * dateconstatation,const PnlMat* past,const PnlMat* pastconst, double H, int i) {

	cout << "on passe dans getConstat" << endl;
	if (i== 0) {
		int n = pnl_mat_resize(dateconstatation, 2, opt_->size_);
		PnlVect S = pnl_vect_wrap_mat_row(past, 0);
		pnl_mat_set_row(dateconstatation, &S, 0);
		S = pnl_vect_wrap_mat_row(past, 1);
		pnl_mat_set_row(dateconstatation, &S, 1);
	}
	else {
		int nb = floor(H / opt_->nbTimeSteps_);
		if (i % nb == 0) {

			int n = pnl_mat_resize(dateconstatation, dateconstatation->m +1, opt_->size_);
			PnlVect S = pnl_vect_wrap_mat_row(past, i+1);
			pnl_mat_set_row(dateconstatation, &S, dateconstatation->m-1);
		}
		else {
			int nb = floor(H / opt_->nbTimeSteps_);
			if ((i + 1) % nb == 0) {
				int k = (i + 1) / nb;
				PnlVect S = pnl_vect_wrap_mat_row(pastconst, k);
				pnl_mat_set_row(dateconstatation, &S, dateconstatation->m - 1);
			}
			else {
				PnlVect S = pnl_vect_wrap_mat_row(past, i + 1);
				pnl_mat_set_row(dateconstatation, &S, dateconstatation->m - 1);
			}
			
		}
	}
	
	/*double temps = t*opt_->nbTimeSteps_ / opt_->T_;
	int nbdate;
	
	if (floor(temps) == temps) {
		nbdate = floor(temps)+1;
		double pas = H / opt_->nbTimeSteps_;
		int n = pnl_mat_resize(dateconstatation, nbdate, opt_->size_);
		PnlVect S = pnl_vect_wrap_mat_row(past, 0);
		pnl_mat_set_row(dateconstatation, &S, 0);
		for (int i = 1; i < nbdate; i++) {
			S = pnl_vect_wrap_mat_row(past, i*pas);
			pnl_mat_set_row(dateconstatation, &S, i);
			
		}
		
	}
	else {
		nbdate = floor(temps) + 2;
		double pas = H / opt_->nbTimeSteps_;
		int n = pnl_mat_resize(dateconstatation, nbdate, opt_->size_);
		PnlVect S = pnl_vect_wrap_mat_row(past, 0);
		pnl_mat_set_row(dateconstatation, &S, 0);
		for (int i = 1; i < nbdate-1; i++) {
			S = pnl_vect_wrap_mat_row(past, i*pas);
			pnl_mat_set_row(dateconstatation, &S, i);
		
		}
		S = pnl_vect_wrap_mat_row(past, floor(t*H/opt_->T_));
		pnl_mat_set_row(dateconstatation, &S, nbdate-1);
		
	}*/

	
}

