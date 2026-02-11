using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace TelAvivMuni_Exercise.Presentation;

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
