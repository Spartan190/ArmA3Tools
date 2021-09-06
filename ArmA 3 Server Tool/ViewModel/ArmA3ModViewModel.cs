using ArmA3PresetList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmA_3_Server_Tool.ViewModel
{
    public class ArmA3ModViewModel : ArmA3Mod, INotifyPropertyChanged
    {

        public override string DisplayName { 
            get => base.DisplayName; 
            protected set {
                base.DisplayName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }

        public override string Link { 
            get => base.Link;
            protected set {
                base.Link = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Link"));
            }
        }

        public override string WorkshopId { 
            get => base.WorkshopId;
            protected set {
                base.WorkshopId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WorkshopId"));
            }
        }

        public ArmA3ModViewModel(string displayName, string link, string workshopId) : base(displayName, link, workshopId)
        {
        }

        public ArmA3ModViewModel(ArmA3Mod mod) : this(mod.DisplayName, mod.Link, mod.WorkshopId)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
