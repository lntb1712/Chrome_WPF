using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chrome_WPF.Models.GroupFunctionDTO
{
    public class ApplicableLocationResponseDTO : INotifyPropertyChanged
    {
        private string? _applicableLocation;
        private bool _isSelected;

        public string ApplicableLocation
        {
            get => _applicableLocation!;
            set
            {
                _applicableLocation = value;
                OnPropertyChanged(nameof(ApplicableLocation));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}