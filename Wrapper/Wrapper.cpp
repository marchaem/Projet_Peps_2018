// This is the main DLL file.

#include "stdafx.h"
using namespace std;
#include "Wrapper.h"

namespace Wrapper {

	WrapperClass::WrapperClass() {
		pricer1 = new Pricer();
	}

	double WrapperClass::getPrice() {
		//Console::WriteLine("on rentre dans le getPrice");
		return pricer1->price();
	
	}


	double WrapperClass::getZero() {
		return 0.0;
	}
}

