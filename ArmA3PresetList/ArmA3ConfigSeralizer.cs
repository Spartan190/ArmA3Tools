using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmA3PresetList
{
    class ArmA3ConfigSeralizer
    {

        private enum LAST
        {
            CLASS,
            ARRAY,
            PROPERTY
        }

        /// <summary>
        /// Deserializes an AmrA 3 Config file (SQM/HPP) to JSON
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public string DeserialzeToJson(string config)
        {
            StringBuilder output = new StringBuilder();
            StringReader input = new StringReader(config);

            LAST last = LAST.CLASS;

            output.Append("{");
            bool emptyClass = true;
            bool classEnd = false;

            string currentLine = "";
            do
            {
                currentLine = input.ReadLine();
                if(currentLine == null)
                {
                    break;
                }
                currentLine = currentLine.Trim().Replace("\\", "\\\\").Replace("\t", "").Replace("\n", "");

                // ignore comments
                if(currentLine.StartsWith("//") || currentLine.StartsWith("#"))
                {
                    continue;
                }

                if(currentLine.StartsWith("class "))
                {
                    if(last == LAST.PROPERTY || last == LAST.ARRAY)
                    {
                        output.Append(",");
                    }

                    if(emptyClass && classEnd)
                    {
                        output.Append(",");
                    }

                    if (currentLine.EndsWith("{};"))
                    {
                        //Empty class
                        if(currentLine.EndsWith(" {};"))
                        {
                            output.Append("\"").Append(currentLine.Substring(6, currentLine.Length - 10)).Append("\":{}");
                        } else
                        {
                            output.Append("\"").Append(currentLine.Substring(6, currentLine.Length - 9)).Append("\":{}");
                        }
                        classEnd = true;
                    } else
                    {
                        if(currentLine.EndsWith("{"))
                        {
                            if(currentLine.EndsWith(" {"))
                            {
                                output.Append("\"").Append(currentLine.Substring(6, currentLine.Length - 8)).Append("\":{");
                            } else
                            {
                                output.Append("\"").Append(currentLine.Substring(6, currentLine.Length - 7)).Append("\":{");
                            }
                        } else
                        {
                            output.Append("\"").Append(currentLine.Substring(6, currentLine.Length - 6)).Append("\":");
                        }
                        classEnd = false;
                    }
                    emptyClass = true;
                    last = LAST.CLASS;
                } else
                {
                    int found = currentLine.IndexOf("=");
                    if(found != -1)
                    {
                        emptyClass = false;
                        int array = currentLine.IndexOf("[]");
                        if(last != LAST.CLASS || (last == LAST.CLASS && classEnd))
                        {
                            output.Append(",");
                        }

                        if(array != -1 && array < found)
                        {
                            // Array
                            output.Append("\"").Append(currentLine.Substring(0, array)).Append("\":[");
                            int end_array = currentLine.IndexOf("};");
                            string value;
                            if (end_array != -1)
                            {
                                if (currentLine.IndexOf("= {") == -1)
                                {
                                    value = currentLine.Substring(found + 2, currentLine.Length - found - 4);
                                } else
                                {
                                    value = currentLine.Substring(found + 3, currentLine.Length - found - 5);
                                }
                                value = value.Replace("{", "[").Replace("}", "]");
                                string newvalue = "";
                                bool stringOpen = false;
                                for(int i = 0; i < value.Length; i++)
                                {
                                    if (stringOpen)
                                    {
                                        if (value[i] == '"')
                                        {
                                            if(value[i + 1] == '"')
                                            {
                                                newvalue += "\\\"\\\"";
                                                i++;
                                                continue;
                                            } else
                                            {
                                                stringOpen = false;
                                            }
                                        }
                                    } else
                                    {
                                        if(value[i] == '"')
                                        {
                                            stringOpen = true;
                                        }
                                    }
                                    newvalue += value[i];
                                }
                                value = newvalue;
                            } else
                            {
                                bool done = false;
                                string newvalue = "";
                                while (!done)
                                {
                                    string next;
                                    next = input.ReadLine();
                                    done = next.IndexOf("};") != -1;
                                    if (!done)
                                    {
                                        string val = next.Trim();
                                        if(val == "{")
                                        {
                                            continue;
                                        }
                                        bool stringOpen = false;
                                        for(int i = 0; i < val.Length; i++)
                                        {
                                            if (stringOpen)
                                            {
                                                if(val[i] == '"')
                                                {
                                                    if (i < val.Length-1 && val[i + 1] == '"')
                                                    {
                                                        newvalue += "\\\"\\\"";
                                                        i++;
                                                        continue;
                                                    } else
                                                    {
                                                        stringOpen = false;
                                                    }
                                                }
                                            } else
                                            {
                                                if(val[i] == '"')
                                                {
                                                    stringOpen = true;
                                                }
                                            }
                                            newvalue += val[i];
                                        }
                                    }
                                }
                                value = newvalue;
                            }

                            output.Append(value).Append("]");
                            last = LAST.ARRAY;
                        } else
                        {
                            //Property
                            string value;
                            string property;
                            int eqpos = currentLine.IndexOf(" = ");
                            if(eqpos + 1 == found)
                            {
                                value = currentLine.Substring(found + 2, currentLine.Length - found - 3);
                                property = currentLine.Substring(0, found - 1);
                            } else
                            {
                                value = currentLine.Substring(found + 1, currentLine.Length - found - 2);
                                property = currentLine.Substring(0, found);
                            }
                            while(value.IndexOf("\" \\\\n \"") != -1)
                            {
                                value = value.Replace("\" \\\\n \"", "\\n");
                            }
                            if(value != "\"\"")
                            {
                                if (value.StartsWith("\"\"\""))
                                {
                                    value = value.Substring(1, value.Length - 1);
                                    value = value.Replace("\"\"", "\\\"\\\"");
                                    value = "\"" + value;
                                } else
                                {
                                    value = value.Replace("\"\"", "\\\"\\\"");
                                }
                            }
                            output.Append("\"").Append(property).Append("\":").Append(value);
                            last = LAST.PROPERTY;
                        }
                    } else
                    {
                        if(currentLine == "};")
                        {
                            output.Append("}");
                            classEnd = true;
                        } else
                        {
                            output.Append(currentLine);
                        }
                    }
                }

            } while (currentLine != null);
            output.AppendLine("}");

            return output.ToString();
        }

        enum STATE { Empty, KeyWaiting, KeyLoading, ValueWaiting, ValueLoading, ArrayLoading };

        /// <summary>
        /// Serializes a JSON ArmA3 Config data to HPP/SQM
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string SerializeFromJson(string json)
        {

            StringReader file = new StringReader(json);

            StringBuilder output = new StringBuilder();

            STATE state = STATE.Empty;

            int indent = 0;

            int c = -1;
            string bufferK = "";
            string bufferV = "";
            do
            {
                c = file.Read();
                if(c == -1)
                {
                    break;
                }

                switch (state)
                {
                    case STATE.Empty:
                        switch (c)
                        {
                            case '{':
                                state = STATE.KeyWaiting;
                                break;
                        }
                        break;
                    case STATE.KeyWaiting:
                        switch (c)
                        {
                            case '"':
                                state = STATE.KeyLoading;
                                if (bufferK != "")
                                {
                                    output.Append(new string(' ', Math.Abs(indent))).Append("class ").Append(bufferK).Append("\n").Append(new string(' ', Math.Abs(indent))).Append("{\n");
                                    bufferK = "";
                                    indent += 2;
                                }
                                break;
                            case '}':
                                if (bufferK != "")
                                {
                                    output.Append(new string(' ', Math.Abs(indent))).Append("class ").Append(bufferK).Append("{};\n");
                                    bufferK = "";
                                }
                                else
                                {
                                    if (indent != 0)
                                    {
                                        indent -= 2;
                                        output.Append(new string(' ', Math.Abs(indent))).Append("};\n");
                                    }
                                }
                                break;
                        }
                        break;
                    case STATE.KeyLoading:
                        switch (c)
                        {
                            case '"':
                                state = STATE.ValueWaiting;
                                break;
                            default:
                                bufferK += (char)c;
                                break;
                        }
                        break;
                    case STATE.ValueWaiting:
                        switch (c)
                        {
                            case ':':
                                break;
                            case '{':
                                state = STATE.KeyWaiting;
                                break;
                            case '[':
                                state = STATE.ArrayLoading;
                                bufferK += "[]";
                                break;
                            case ' ':
                                break;
                            default:
                                state = STATE.ValueLoading;
                                bufferV += (char)c;
                                break;
                        }
                        break;
                    case STATE.ValueLoading:
                        switch (c)
                        {
                            case '}':
                                state = STATE.KeyWaiting;
                                output.Append(new string(' ', Math.Abs(indent))).Append(bufferK).Append(" = ").Append(bufferV).Append(";\n");
                                indent -= 2;
                                output.Append(new string(' ', Math.Abs(indent))).Append("};\n");
                                bufferK = "";
                                bufferV = "";
                                break;
                            case ',':
                                state = STATE.KeyWaiting;
                                output.Append(new string(' ', Math.Abs(indent))).Append(bufferK).Append(" = ").Append(bufferV).Append(";\n");
                                bufferK = "";
                                bufferV = "";
                                break;
                            default:
                                bufferV += (char)c;
                                break;
                        }
                        break;
                    case STATE.ArrayLoading:
                        switch (c)
                        {
                            case '}':
                                state = STATE.KeyWaiting;
                                output.Append(new string(' ', Math.Abs(indent))).Append(bufferK).Append(" = {").Append(bufferV).Append("};\n");
                                indent -= 2;
                                output.Append(new string(' ', Math.Abs(indent))).Append("};\n");
                                bufferK = "";
                                bufferV = "";
                                break;
                            case ']':
                                state = STATE.KeyWaiting;
                                output.Append(new string(' ', Math.Abs(indent))).Append(bufferK).Append(" = {").Append(bufferV).Append("};\n");
                                bufferK = "";
                                bufferV = "";
                                break;
                            case '[':
                                break;
                            default:
                                bufferV += (char)c;
                                break;
                        }
                        break;
                }
            } while (c != -1);

            return output.ToString();

        }

    }
}
