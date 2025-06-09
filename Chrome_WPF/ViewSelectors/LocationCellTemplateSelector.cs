using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.ProductCustomerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF.ViewSelectors
{
    public class LocationCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DisplayTemplate { get; set; }
        public DataTemplate? EditTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LocationMasterResponseDTO location && location.IsNewRow)
            {
                return EditTemplate!;
            }

            return DisplayTemplate!;
        }
    }
}
