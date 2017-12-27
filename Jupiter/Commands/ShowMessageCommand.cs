using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Jupiter.Commands
{
    public sealed class ShowMessageCommand
    {
        private static ICommand command;
        public static ICommand Command
        {
            get
            {
                if(command == null)
                {
                    command = new DelegateCommand(
                    (param) => ShowMessageCommandExecute(param),
                    (param) => CanShowMessageCommandExecute(param));
                }
                return command;
            }
        }

        private static bool CanShowMessageCommandExecute(object param)
        {
            return param != null;
        }

        private static void ShowMessageCommandExecute(object param)
        {
            MessageBox.Show(param.ToString(), "!!ERROR!!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
