// This is the main DLL file.
#include "stdafx.h"
#include "Wrapper2.h"



using namespace pricerOption;
namespace Wrapper {

	WrapperClass::WrapperClass() {
		
		Console::WriteLine("on crée un nouveau wrapper");
	}

	double WrapperClass::getPrice() {
		Console::WriteLine("on rentre dans le price");
		return(pricer->price());
	}

	double WrapperClass::getZero() {
		return 0.0;
	}
}