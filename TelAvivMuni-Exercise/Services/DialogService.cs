using System.Collections;
using System.Windows;
using TelAvivMuni_Exercise.Controls;
using TelAvivMuni_Exercise.ViewModels;

namespace TelAvivMuni_Exercise.Services
{
    public class DialogService : IDialogService
    {
        public object? ShowBrowseDialog(IEnumerable items, string title, object? currentSelection)
        {
            var viewModel = new DataBrowserDialogViewModel(items, currentSelection);

            var dialog = new DataBrowserDialog
            {
                DataContext = viewModel,
                Title = title,
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                return viewModel.SelectedItem;
            }

            return currentSelection;
        }
    }
}
