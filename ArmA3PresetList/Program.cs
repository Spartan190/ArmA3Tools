using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArmA3PresetList
{


    class Program
    {
        static void Main(string[] args)
        {

            /*string content = File.ReadAllText(args[0]);
            ArmA3ConfigSeralizer configSeralizer = new ArmA3ConfigSeralizer();

            Console.WriteLine(configSeralizer.DeserialzeToJson(content));*/

            if(args.Length == 0)
            {
                Console.WriteLine("No argument given. Please specify the ArmA 3 Preset file path as argument.");
                Environment.Exit(-1);
            } else if(args.Length > 1)
            {
                Console.WriteLine("Only one argument is supported. Ignoring the excessive arguments. Assuming the first argument as the ArmA 3 file path.");
            }
            var filePath = args[0];

            ArmA3PresetFile armA3PresetFile = new ArmA3PresetFile(filePath);

            StringBuilder modDisplayNames = new StringBuilder();
            StringBuilder modIds = new StringBuilder();
            StringBuilder checkRegex = new StringBuilder();

            foreach(var mod in armA3PresetFile.armA3Mods)
            {
                string modDisplayName = mod.displayName;
                int colonIndex = modDisplayName.IndexOf(':');
                if (colonIndex != -1)
                {
                    modDisplayName = modDisplayName.Remove(colonIndex);
                }

                modDisplayName = modDisplayName.Replace("  ", " ");
                modDisplayName = modDisplayName.TrimEnd();
                modDisplayNames.Append($"@{modDisplayName};");

                checkRegex.Append($"({modDisplayName.Replace("(", "\\(").Replace(")", "\\)")}\\n)|");

                string modId = mod.link.Substring(mod.link.LastIndexOf("?id=")+4);

                modIds.Append($"{modId};");
            }

            Console.WriteLine($"Mods Names ({armA3PresetFile.armA3Mods.Count}):");
            Console.WriteLine(modDisplayNames.ToString());
            Console.WriteLine();
            Console.WriteLine("Mods IDs:");
            Console.WriteLine(modIds.Remove(modIds.Length-1,1).ToString());
            Console.WriteLine();
            Console.WriteLine("Mods Regex:");
            Console.WriteLine(checkRegex.Remove(checkRegex.Length - 1, 1).ToString());
            
            Console.ReadKey();
        }
    }
}
