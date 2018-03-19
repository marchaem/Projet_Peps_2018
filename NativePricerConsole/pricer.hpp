#pragma once
#define DLLEXP __declspec(dllexport)
	class Pricer{
	public:
		DLLEXP Pricer();
		DLLEXP double price();
		DLLEXP double zero();
	};
		 