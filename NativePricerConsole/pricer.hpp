#pragma once
#define DLLEXP  __declspec( dllexport )
namespace pricerOption {
	DLLEXP class pricer {
	public:
		//MonteCarlo * mc=new MonteCarlo();
		//BlackScholesModel * bsModel=new BlackScholesModel();
		//Eurostral100 * euro100 = new Eurostral100();
		//Option * opt;
		//Utilities * util = new Utilities();
		//QuantoOption * quanto = new QuantoOption();
		DLLEXP double price();
		DLLEXP double zero();
		 };
		 
}