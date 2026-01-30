using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using TelAvivMuni_Exercise.Controls;
using TelAvivMuni_Exercise.Models;
using TelAvivMuni_Exercise.ViewModels;

namespace TelAvivMuni_Exercise.Services
{
    /// <summary>
    /// Provides dialog display services for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Displays a modal browse dialog for selecting an item from a collection.
        /// </summary>
        /// <param name="items">The collection of items to display in the dialog.</param>
        /// <param name="title">The dialog window title.</param>
        /// <param name="currentSelection">The currently selected item, if any.</param>
        /// <param name="columns">Optional column configuration for the data grid.</param>
        /// <returns>The selected item if confirmed; otherwise, the original selection.</returns>
        public object? ShowBrowseDialog(IEnumerable items, string title, object? currentSelection, ObservableCollection<BrowserColumn>? columns = null)
        {
            if (items == null)
            {
                MessageBox.Show("No items available to display.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return currentSelection;
            }

            var mainWindow = Application.Current.MainWindow;

            // Create fresh instances each time (simple, no state leakage, GC handles cleanup)
            var viewModel = new DataBrowserDialogViewModel(items, currentSelection, columns);
            var dialog = new DataBrowserDialog
            {
                DataContext = viewModel,
                Title = title,
                Owner = mainWindow
            };

            // Position dialog to the right of the main window
            dialog.Left = mainWindow.Left + mainWindow.ActualWidth;
            dialog.Top = mainWindow.Top;

            if (dialog.ShowDialog() == true)
            {
                return viewModel.SelectedItem;
            }

            return currentSelection;
        }
    }
}
