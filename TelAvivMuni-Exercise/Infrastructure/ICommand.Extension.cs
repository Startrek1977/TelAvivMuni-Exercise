using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TelAvivMuni_Exercise.Infrastructure
{
    public static class ICommandExtension
    {
        public static void NotifyCanExecuteChanged(this ICommand command)
        {
            if (command is RelayCommand relayCommand)
            {
                relayCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
