using HG.RevitTools.ViewModels;

using System.Windows;

namespace HG.RevitTools.Views
{
    public partial class SheetNamingFormatView : Window
    {
        private readonly SheetNamingFormatViewModel viewModel;

        public SheetNamingFormatView()
        {
            InitializeComponent();

            viewModel = new SheetNamingFormatViewModel();

            DataContext = viewModel;
        }

        public string SheetNameTemplate => viewModel.Template;

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}