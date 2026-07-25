using HG.RevitTools.ViewModels.ParameterChecker;
using System.Windows;

namespace HG.RevitTools.Views.ParameterChecker
{
    public partial class ParameterCheckerWindow : Window
    {
        public ParameterCheckerViewModel ViewModel
        {
            get
            {
                return DataContext
                    as ParameterCheckerViewModel;
            }
        }

        public ParameterCheckerWindow(
            ParameterCheckerViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void SelectMismatches_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ViewModel == null ||
                !ViewModel.CanCheckElements)
            {
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}