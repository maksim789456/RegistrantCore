using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Registrant.Forms
{
    public partial class PrintShipments 
    {
        Controllers.PrintShipmentController controller;
        public PrintShipments()
        {
            InitializeComponent();
            controller = new Controllers.PrintShipmentController();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            grid_shipments.ItemsSource = null;
            if (DatePicker.SelectedDate != null)
                grid_shipments.ItemsSource = controller.GetShipmentsDate(DatePicker.SelectedDate.Value);
        }

        private void btn_month_Click(object sender, RoutedEventArgs e)
        {
            grid_shipments.ItemsSource = null;
            if (DatePicker.SelectedDate != null)
                grid_shipments.ItemsSource = controller.GetShipmentsMonth(DatePicker.SelectedDate.Value);
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            ModernWpf.MessageBox.Show("This is a test text!", "Some title", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private void btn_saveExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Реестр", 
                DefaultExt = ".xls", 
                Filter = "Говнофайлы (.xls)|*.xls"
            };

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                grid_shipments.SelectAllCells();
                grid_shipments.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, grid_shipments);
                String result = (string)Clipboard.GetData(DataFormats.Text);
                grid_shipments.UnselectAllCells();
                System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false, Encoding.Unicode);
                file.WriteLine(result.Replace(",", " "));
                file.Close();
            }
        }

    }
}
