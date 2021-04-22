#include <iostream>
#include <string>
#include <fstream>
#include <streambuf>
#include "parse2json.h"

int main() {
	std::ifstream t("mission.sqm");
	std::string str((std::istreambuf_iterator<char>(t)),
		std::istreambuf_iterator<char>());

	std::cout << config2json(str.c_str());

	return 0;
}