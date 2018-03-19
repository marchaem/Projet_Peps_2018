#pragma once
#define DLLEXP __declspec(dllexport)
	DLLEXP class Pricer{
	public:
		DLLEXP Pricer();
		DLLEXP double price();
		DLLEXP double zero();
	};
		 