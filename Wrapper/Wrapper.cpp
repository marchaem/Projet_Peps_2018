// This is the main DLL file.

#include "stdafx.h"
using namespace std;
#include "Wrapper.h"
using namespace pricerOption;
namespace Wrapper {
	double WrapperClass::getPrice() {
		Console::WriteLine("on rentre dans le getPrice");
		return(this->pricer->price());
	}

	WrapperClass::WrapperClass() {
		this->pricer = new pricerOption::pricer();
	}

	double WrapperClass::getZero() {
		return 0.0;
	}
}

