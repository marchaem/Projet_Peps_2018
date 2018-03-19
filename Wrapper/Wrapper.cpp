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

	double* WrapperClass::convertArrayPointer(array<double> ^ arr) {
		double* res = new double[arr->Length];
		
		for (int i = 0; i < arr->Length; i++) {
			*(res + i) = arr[i];
		}

		return res;
	}

	double* WrapperClass::convertMatrixPointer(array<double,2> ^ mat) {
		double* res = new double[mat->Length];
		
		int a = mat->GetLength(0);

		for (int i = 0; i < mat->GetLength(0); i++) {
		
			for (int j = 0; j < mat->GetLength(1); j++) {
				*(res + mat->GetLength(0)*i + j) = mat[i, j];
			}
		
		}

		return res;
	}
}

