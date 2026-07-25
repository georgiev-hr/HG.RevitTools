using System.ComponentModel;

namespace HG.RevitTools.ViewModels
{
    public class SelectableItem : INotifyPropertyChanged
    {
        public object Item { get; set; }

        public string DisplayName { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;

                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}