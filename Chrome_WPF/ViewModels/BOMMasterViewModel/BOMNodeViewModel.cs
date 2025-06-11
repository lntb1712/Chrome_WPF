using Chrome_WPF.Models.BOMComponentDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.ViewModels.BOMMasterViewModel
{
    public class BOMNodeViewModel : INotifyPropertyChanged
    {
        public BOMNodeDTO Data { get; set; }
        public ObservableCollection<BOMNodeViewModel> Children { get; set; }

        public BOMNodeViewModel(BOMNodeDTO data)
        {
            Data = data;
            Children = new ObservableCollection<BOMNodeViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
