using System.ComponentModel;
using System.Windows;
using TelAvivMuni_Exercise.ViewModels;

namespace TelAvivMuni_Exercise.Controls
{
    public partial class DataBrowserDialog : Window
    {
        public DataBrowserDialog()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DataBrowserDialogViewModel oldViewModel)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (e.NewValue is DataBrowserDialogViewModel newViewModel)
            {
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataBrowserDialogViewModel.DialogResult))
            {
                if (DataContext is DataBrowserDialogViewModel viewModel && viewModel.DialogResult.HasValue)
                {
                    DialogResult = viewModel.DialogResult;
                }
            }
        }
    }
}
