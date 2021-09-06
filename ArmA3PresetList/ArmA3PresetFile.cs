using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ArmA3PresetList
{


    public class ArmA3Mod
    {
        public virtual string DisplayName {
            get;
            protected set;
        }
        public virtual string Link {
            get;
            protected set;
        }

        public virtual string WorkshopId {
            get;
            protected set;
        }

        public ArmA3Mod(string displayName, string link, string workshopId)
        {
            this.DisplayName = displayName;
            this.Link = link;
            this.WorkshopId = workshopId;
        }
    }
    public class ArmA3PresetFile
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

                    var modContainers = htmlDoc.DocumentNode.SelectNodes("//body/div/table/tr");
                    foreach (var modContainer in modContainers)
                    {
                        var modData = modContainer.SelectNodes("td");
                        string modDisplayName = modData[0].InnerText;
                        int colonIndex = modDisplayName.IndexOf(':');
                        if (colonIndex != -1)
                        {
                            modDisplayName = modDisplayName.Remove(colonIndex);
                        }

                        //modDisplayName = modDisplayName.Replace("  ", " ");
                        modDisplayName = modDisplayName.TrimEnd();

                        string modLink = modData[2].SelectSingleNode("a").InnerText;

                        string modId = modLink.Substring(modLink.LastIndexOf("?id=") + 4);

                        armA3Mods.Add(new ArmA3Mod(modDisplayName, modLink, modId));
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
