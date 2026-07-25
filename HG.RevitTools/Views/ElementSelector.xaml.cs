using HG.RevitTools.ViewModels;

using System;
using System.Collections.Generic;
using System.Windows;

namespace HG.RevitTools.Views
{
    public partial class ElementSelector : Window
    {
        private readonly ElementSelectorViewModel viewModel;

        public ElementSelector(
            IEnumerable<object> items,
            Func<object, string> displayFunc,
            string headerText = "Select Items")
        {
            InitializeComponent();

            viewModel = new ElementSelectorViewModel(
                items,
                displayFunc,
                headerText);

            HeaderLabel.Content = viewModel.HeaderText;
            Title = viewModel.HeaderText;

            ItemsList.ItemsSource = viewModel.Items;
        }

        public List<T> GetSelectedItems<T>()
        {
            return viewModel.GetSelectedItems<T>();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            viewModel.SelectAll();
        }

        private void Select(object sender, RoutedEventArgs e)
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