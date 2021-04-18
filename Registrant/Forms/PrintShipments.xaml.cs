using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                Filter = "Excel файлы (.xls)|*.xls"
            };

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                grid_shipments.SelectAllCells();
                grid_shipments.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, grid_shipments);
                String result = (string)Clipboard.GetData(DataFormats.Text);
                grid_shipments.UnselectAllCells();
                StreamWriter file = new StreamWriter(filename, false, Encoding.Unicode);
                file.WriteLine(result.Replace(",", " "));
                file.Close();
            }
        }

        private void btn_print_Click_1(object sender, RoutedEventArgs e)
        {
            var item = grid_shipments.ItemsSource as IList<Models.PrintShipments>;
            var json = JsonConvert.SerializeObject(item);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream(Environment.CurrentDirectory + @"\print.pdf", FileMode.Create));
            document.Open();

            Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            PdfPTable table = new PdfPTable(dt.Columns.Count);

            float[] widths = new float[dt.Columns.Count];

            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = 10f;
            }

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"))
            {
                Colspan = dt.Columns.Count
            };

            foreach (DataColumn c in dt.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        table.AddCell(new Phrase(r[i].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
        }
    }
}
