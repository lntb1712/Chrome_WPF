using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.StorageProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF.ViewSelectors
{
    public class StorageProductCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DisplayTemplate { get; set; }
        public DataTemplate? EditTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StorageProductResponseDTO storage && storage.IsNewRow)
            {
                return EditTemplate!;
            }

            return DisplayTemplate!;
        }
    }
}
