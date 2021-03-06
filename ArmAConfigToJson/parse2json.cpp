#include "pch.h"
#include <fstream>
#include <string>
#include <iostream>
#include <algorithm>
#include <sstream>

bool startswith(std::string& source, std::string compare) {
	return source.substr(0, compare.size()) == compare;
}

bool endswith(std::string const& value, std::string const& ending)
{
	if (ending.size() > value.size()) return false;
	return std::equal(ending.rbegin(), ending.rend(), value.rbegin());
}

inline std::string& ltrim(std::string& s, const char* t = " \t\n\r\f\v")
{
	s.erase(0, s.find_first_not_of(t));
	return s;
}

inline std::string& rtrim(std::string& s, const char* t = " \t\n\r\f\v")
{
	s.erase(s.find_last_not_of(t) + 1);
	return s;
}

inline std::string& trim(std::string& s, const char* t = " \t\n\r\f\v")
{
	return ltrim(rtrim(s, t), t);
}

void replace(std::string& str, const std::string& from, const std::string& to) {
	size_t start_pos = 0;
	do {
		start_pos = str.find(from, start_pos);
		if (start_pos == std::string::npos)
			return;
		str.replace(start_pos, from.length(), to);
		start_pos += to.size();
	} while (start_pos != std::string::npos);
}

enum LAST { CLASS, ARRAY, PROPERTY };

const char* config2json(const char* config)
{

	/*if (argv[1] == "--version") {
		std::cout << "0.2" << std::endl;
		return 0;
	}*/

	std::istringstream ifile(config);

	/*if (argc == 3) {
		ofile.open(argv[2], std::ios::out);
		std::cout << "Parsing " << argv[1] << " into " << argv[2] << std::endl;
	}*/

	std::ostringstream output = std::ostringstream();

	std::string str;
	LAST last = CLASS;
	output << "{";
	bool emptyClass = true;
	bool classEnd = false;
	while (std::getline(ifile, str))
	{
		trim(str);
		replace(str, "\\", "\\\\");
		replace(str, "\t", "");
		replace(str, "\n", "");

		if (!startswith(str, "//") && !startswith(str, "#")) {
			if (startswith(str, "class ")) {
				if (last == PROPERTY || last == ARRAY) {
					output << ",";
				}
				if (emptyClass && classEnd) {
					output << ",";
				}
				if (endswith(str, "{};")) {
					//Empty Class
					if (endswith(str, " {};")) {
						output << "\"" << str.substr(6, str.size() - 10) << "\":{}";
					}
					else {
						output << "\"" << str.substr(6, str.size() - 9) << "\":{}";
					}
					classEnd = true;
				}
				else {
					if (endswith(str, "{")) {
						if (endswith(str, " {")) {
							output << "\"" << str.substr(6, str.size() - 8) << "\":{";
						}
						else {
							output << "\"" << str.substr(6, str.size() - 7) << "\":{";
						}
					}
					else {
						output << "\"" << str.substr(6, str.size() - 6) << "\":";
					}
					classEnd = false;
				}
				emptyClass = true;
				last = CLASS;
			}
			else {
				// Stopped here -------------------------------------------------------------------------------------------------------------------
				std::size_t found = str.find("=");
				if (found != std::string::npos) {
					emptyClass = false;
					std::size_t array = str.find("[]");
					if (last != CLASS || (last == CLASS && classEnd)) {
						output << ",";
					}
					if (array != std::string::npos && array < found) {
						//Array
						output << "\"" << str.substr(0, array) << "\":[";
						std::size_t end_array = str.find("};");
						std::string value;
						if (end_array != std::string::npos) {
							if (str.find("= {") == std::string::npos) {
								value = str.substr(found + 2, str.size() - found - 4);
							}
							else {
								value = str.substr(found + 3, str.size() - found - 5);
							}
							replace(value, "{", "[");
							replace(value, "}", "]");
							std::string newvalue = "";
							bool stringOpen = false;
							for (int i = 0; i < value.size(); i++) {
								if (stringOpen) {
									if (value[i] == '"') {
										if (value[i + 1] == '"') {
											newvalue += "\\\"\\\"";
											i++;
											continue;
										}
										else {
											stringOpen = false;
										}
									}
								}
								else {
									if (value[i] == '"') {
										stringOpen = true;
									}
								}
								newvalue += value[i];
							}
							value = newvalue;
						}
						else {
							bool done = false;
							std::string newvalue = "";
							while (!done) {
								std::string next;
								std::getline(ifile, next);
								done = next.find("};") != std::string::npos;
								if (!done) {
									std::string value = trim(next);
									if (value == "{") {
										continue;
									}
									bool stringOpen = false;
									for (int i = 0; i < value.size(); i++) {
										if (stringOpen) {
											if (value[i] == '"') {
												if (value[i + 1] == '"') {
													newvalue += "\\\"\\\"";
													i++;
													continue;
												}
												else {
													stringOpen = false;
												}
											}
										}
										else {
											if (value[i] == '"') {
												stringOpen = true;
											}
										}
										newvalue += value[i];
									}
								}
							}
							value = newvalue;
						}

						output << value << "]";
						last = ARRAY;
					}
					else {
						//Property
						std::string value;
						std::string property;
						std::size_t eqpos = str.find(" = ");
						if (eqpos + 1 == found) {
							value = str.substr(found + 2, str.size() - found - 3);
							property = str.substr(0, found - 1);
						}
						else {
							value = str.substr(found + 1, str.size() - found - 2);
							property = str.substr(0, found);
						}
						while (value.find("\" \\\\n \"") != std::string::npos) {
							replace(value, "\" \\\\n \"", "\\n");
						}
						if (value != "\"\"") {
							if (startswith(value, "\"\"\"")) {
								value = value.substr(1, value.size() - 1);
								replace(value, "\"\"", "\\\"\\\"");
								value = "\"" + value;
							}
							else {
								replace(value, "\"\"", "\\\"\\\"");
							}
						}
						output << "\"" << property << "\":" << value;
						last = PROPERTY;
					}
				}
				else {
					if (str == "};") {
						output << "}";
						classEnd = true;
					}
					else {
						output << str;
					}
				}
			}
		}
	}
	output << "}" << std::endl;
	/*if (argc == 3)
		ofile.close();*/

	return output.str().c_str();
}

