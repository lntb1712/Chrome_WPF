using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chrome_WPF.Models.GroupFunctionDTO
{
    public class GroupFunctionResponseDTO : INotifyPropertyChanged
    {
        private ObservableCollection<ApplicableLocationResponseDTO> _applicableLocations;

        public string GroupId { get; set; } = null!;
        public string? FunctionId { get; set; }
        public string? FunctionName { get; set; }
        public bool IsEnable { get; set; }

        public ObservableCollection<ApplicableLocationResponseDTO> ApplicableLocations
        {
            get => _applicableLocations;
            set
            {
                if (_applicableLocations != null)
                {
                    foreach (var location in _applicableLocations)
                    {
                        location.PropertyChanged -= LocationPropertyChanged;
                    }
                }
                _applicableLocations = value ?? new ObservableCollection<ApplicableLocationResponseDTO>();
                foreach (var location in _applicableLocations)
                {
                    location.PropertyChanged += LocationPropertyChanged;
                }
                OnPropertyChanged(nameof(ApplicableLocations));
                OnPropertyChanged(nameof(SelectedWarehousesDisplay));
            }
        }

        public string SelectedWarehousesDisplay
        {
            get
            {
                var selected = ApplicableLocations?.Where(x => x.IsSelected).Select(x => x.ApplicableLocation);
                return selected != null && selected.Any()
                    ? "Đã chọn: " + string.Join(", ", selected)
                    : "Chọn kho áp dụng";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void LocationPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicableLocationResponseDTO.IsSelected))
            {
                OnPropertyChanged(nameof(SelectedWarehousesDisplay));
            }
        }

        public GroupFunctionResponseDTO()
        {
            _applicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>();
        }
    }
}