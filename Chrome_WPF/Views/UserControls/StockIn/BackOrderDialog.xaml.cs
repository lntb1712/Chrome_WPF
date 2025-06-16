using Chrome_WPF.ViewModels.StockInViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Chrome_WPF.Views.UserControls.StockIn
{
    /// <summary>
    /// Interaction logic for BackOrderDialog.xaml
    /// </summary>
    public partial class BackOrderDialog : Window
    {
        private readonly BackOrderDialogViewModel _viewModel;

        public BackOrderDialog(BackOrderDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;

            // Theo dõi thay đổi của IsClosed để đóng cửa sổ
            var binding = new Binding("IsClosed") { Source = _viewModel };
            BindingOperations.SetBinding(this, IsClosedProperty, binding);
        }

        // Định nghĩa phụ thuộc cho IsClosed
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register("IsClosed", typeof(bool), typeof(BackOrderDialog),
                new PropertyMetadata(false, OnIsClosedChanged));

        public bool IsClosed
        {
            get => (bool)GetValue(IsClosedProperty);
            set => SetValue(IsClosedProperty, value);
        }

        private static void OnIsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BackOrderDialog window && (bool)e.NewValue)
            {
                window.Close(); // Đóng cửa sổ khi IsClosed thành true
            }
        }
    }
}