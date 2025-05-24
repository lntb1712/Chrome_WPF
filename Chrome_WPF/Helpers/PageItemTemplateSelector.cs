using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF.Helpers
{
    public class PageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? PageNumberTemplate { get; set; }
        public DataTemplate? EllipsisTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
            {
                return EllipsisTemplate!;
            }
            return PageNumberTemplate!;
        }
    }
}
