#pragma once
#define DLLEXP   __declspec( dllexport )
namespace pricerOption {
	 class pricer {
	 public :
		 DLLEXP double price();
		 DLLEXP double zero();
	};
		
	 

}