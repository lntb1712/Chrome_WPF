using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.GroupManagementDTO
{
    public class GroupManagementResponseDTO : INotifyPropertyChanged
    {
        private bool _isSelected;
        public string GroupId { get; set; } = null!;
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
