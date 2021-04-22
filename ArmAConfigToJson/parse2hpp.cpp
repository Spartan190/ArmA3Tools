#include "pch.h"
#include <fstream>
#include <string>
#include <iostream>
#include <sstream>

enum STATE { Empty, KeyWaiting, KeyLoading, ValueWaiting, ValueLoading, ArrayLoading };

const char* json2config(const char* json) {

	/*        if (argv[1] == "--version") {
				std::cout << "0.2" << std::endl;
				return 0;
			}*/

	std::istringstream file(json);

	/*if (argc == 3) {
		ofile.open(argv[2], std::ios::out);
		std::cout << "Parsing " << argv[1] << " into " << argv[2] << std::endl;
	}*/
	std::ostringstream output = std::ostringstream();

	STATE state = Empty;

	int indent = 0;

	char c;
	std::string bufferK;
	std::string bufferV;
	while (file.get(c)) {
		switch (state) {
		case Empty:
			switch (c) {
			case '{':
				state = KeyWaiting;
				break;
			}
			break;
		case KeyWaiting:
			switch (c) {
			case '"':
				state = KeyLoading;
				if (bufferK != "") {
					output << std::string(indent, ' ') << "class " << bufferK << "\n" << std::string(indent, ' ') << "{\n";
					bufferK = "";
					indent += 2;
				}
				break;
			case '}':
				if (bufferK != "") {
					output << std::string(indent, ' ') << "class " << bufferK << "{};\n";
					bufferK = "";
				}
				else {
					if (indent != 0) {
						indent -= 2;
						output << std::string(indent, ' ') << "};\n";
					}
				}
			}
			break;
		case KeyLoading:
			switch (c) {
			case '"':
				state = ValueWaiting;
				break;
			default:
				bufferK += c;
			}
			break;
		case ValueWaiting:
			switch (c) {
			case ':':
				break;
			case '{':
				state = KeyWaiting;
				break;
			case '[':
				state = ArrayLoading;
				bufferK += "[]";
			case ' ':
				break;
			default:
				state = ValueLoading;
				bufferV += c;
			}
			break;
		case ValueLoading:
			switch (c) {
			case '}':
				state = KeyWaiting;
				output << std::string(indent, ' ') << bufferK << " = " << bufferV << ";\n";
				indent -= 2;
				output << std::string(indent, ' ') << "};\n";
				bufferK = "";
				bufferV = "";
				break;
			case ',':
				state = KeyWaiting;
				output << std::string(indent, ' ') << bufferK << " = " << bufferV << ";\n";
				bufferK = "";
				bufferV = "";
				break;
			default:
				bufferV += c;
			}
			break;
		case ArrayLoading:
			switch (c) {
			case '}':
				state = KeyWaiting;
				output << std::string(indent, ' ') << bufferK << " = {" << bufferV << "};\n";
				indent -= 2;
				output << std::string(indent, ' ') << "};\n";
				bufferK = "";
				bufferV = "";
				break;
			case ']':
				state = KeyWaiting;
				output << std::string(indent, ' ') << bufferK << " = {" << bufferV << "};\n";
				bufferK = "";
				bufferV = "";
				break;
			case '[':
				break;
			default:
				bufferV += c;
			}
			break;
		}
	}

	return output.str().c_str();
}
