using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ArmA3PresetList
{


    class ArmA3Mod
    {
        public readonly string displayName;
        public readonly string link;

        public ArmA3Mod(string displayName, string link)
        {
            this.displayName = displayName;
            this.link = link;
        }
    }
    class ArmA3PresetFile
    {

        public readonly List<ArmA3Mod> armA3Mods = new List<ArmA3Mod>();

        public ArmA3PresetFile(string filePath)
        {
            LoadFile(filePath);
        }

        private void LoadFile(string filePath)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.Load(Path.GetFullPath(filePath));

            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                StringBuilder parseErrorString = new StringBuilder();
                parseErrorString.AppendLine($"Error reading the ArmA 3 preset document.");
                foreach (var error in htmlDoc.ParseErrors)
                {
                    parseErrorString.AppendLine($"   Code: {error.Code}");
                    parseErrorString.AppendLine($"      Reson: {error.Reason}");
                    parseErrorString.AppendLine($"      at Line: {error.Line} Position: {error.LinePosition}");
                    parseErrorString.AppendLine($"      {error.SourceText}");
                }
                throw new FileFormatException(parseErrorString.ToString());
            }
            else
            {
                if (htmlDoc.DocumentNode != null)
                {

                    StringBuilder modCodes = new StringBuilder();
                    StringBuilder modNames = new StringBuilder();

                    var modContainers = htmlDoc.DocumentNode.SelectNodes("//body/div/table/tr");
                    foreach (var modContainer in modContainers)
                    {
                        var modData = modContainer.SelectNodes("td");
                        string modDisplayName = modData[0].InnerText;

                        string modLink = modData[2].SelectSingleNode("a").InnerText;

                        armA3Mods.Add(new ArmA3Mod(modDisplayName, modLink));
                    }
                }
                else
                {
                    throw new FileFormatException("Couldn't get the Document Node");
                }
            }
        }

    }
}
