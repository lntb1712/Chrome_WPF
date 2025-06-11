using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Chrome_WPF.Helpers
{
    public static class DataGridHelper
    {
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow row, int columnIndex)
        {
            if (dataGrid == null || row == null) return null!;
            var presenter = row.FindVisualChild<DataGridCellsPresenter>();
            if (presenter == null) return null!;
            return (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
        }
    }
}
