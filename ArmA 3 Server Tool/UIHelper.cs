using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArmA_3_Server_Tool
{
    public class UIHelper
    {
        public UIHelper()
        {

        }

        public void ShowLabel(Label label, int milliseconds, Dispatcher dispatcher)
        {
            label.Visibility = Visibility.Visible;
            Task.Run(() =>
            {
                Task.Delay(milliseconds).ContinueWith(_ =>
                {

                    dispatcher.Invoke(() => {
                        label.Visibility = Visibility.Hidden;
                    });
                });

            });
        }
    }
}
