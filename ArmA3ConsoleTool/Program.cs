using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace ArmA3PresetList
{


    class Program
    {
        static void Main(string[] args)
        {

            /*string content = File.ReadAllText(args[0]);
            ArmA3ConfigSeralizer configSeralizer = new ArmA3ConfigSeralizer();
            string json = configSeralizer.DeserialzeToJson(content);
            string missionSqm = configSeralizer.SerializeFromJson(json);
            File.WriteAllText("test.json", json);
            File.WriteAllText("test.sqm", missionSqm);
            Console.WriteLine("done");*/

            if (args.Length == 0)
            {
                Console.WriteLine("No argument given. Please specify the ArmA 3 Preset file path as argument.");
                Environment.Exit(-1);
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Only one argument is supported. Ignoring the excessive arguments. Assuming the first argument as the ArmA 3 file path.");
            }
            var filePath = args[0];

            ArmA3PresetFile armA3PresetFile = new ArmA3PresetFile(filePath);

            StringBuilder modDisplayNames = new StringBuilder();
            StringBuilder modIds = new StringBuilder();
            StringBuilder checkRegex = new StringBuilder();
            int distinctCount = 0;
            foreach (var mod in armA3PresetFile.armA3Mods)
            {
                
                string modDisplayName = HttpUtility.HtmlDecode(mod.DisplayName);

                if (!modDisplayNames.ToString().Contains($"@{modDisplayName};"))
                {
                    modDisplayNames.Append($"@{modDisplayName};");
                    checkRegex.Append($"({modDisplayName.Replace("(", "\\(").Replace(")", "\\)")}\\n)|");
                    distinctCount++;
                }


                string modId = mod.WorkshopId;

                modIds.Append($"{modId};");

            }

            Console.WriteLine($"Mods Names ({distinctCount}):");
            Console.WriteLine(modDisplayNames.ToString());
            Console.WriteLine();
            Console.WriteLine($"Mods IDs {armA3PresetFile.armA3Mods.Count}:");
            Console.WriteLine(modIds.Remove(modIds.Length - 1, 1).ToString());
            Console.WriteLine();
            Console.WriteLine("Mods Regex:");
            Console.WriteLine(checkRegex.Remove(checkRegex.Length - 1, 1).ToString());

            Console.ReadKey();
        }
    }
}
