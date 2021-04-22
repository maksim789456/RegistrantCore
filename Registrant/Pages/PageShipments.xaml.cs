using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Registrant.Controllers;
using Registrant.DB;
using Registrant.Models;

namespace Registrant.Pages
{
    public partial class PageShipments
    {
        private ShipmentController _shipmentController;

        public PageShipments()
        {
            InitializeComponent();
            _shipmentController = new ShipmentController();
            DatePicker.SelectedDate = DateTime.Now;

            Thread thread = new Thread(RefreshThread);
            thread.Start();

            if (App.LevelAccess == "admin" || App.LevelAccess == "shipment")
            {
                btn_new.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Thread method
        /// </summary>
        void RefreshThread()
        {
            while (true)
            {
                if (Dispatcher.Invoke(() => string.IsNullOrWhiteSpace(tb_search.Text)))
                {
                    if (Dispatcher.Invoke(() => DatePicker.SelectedDate != null))
                    {
                        if (Dispatcher.Invoke(() => cb_sort.SelectedIndex == 0))
                        {
                            Dispatcher.Invoke(() => DataGrid_Shipments.ItemsSource = _shipmentController.GetShipments(DatePicker.SelectedDate ?? default));
                            Dispatcher.Invoke(() => DataGrid_Shipments.Items.Refresh());
                        }
                        else if (Dispatcher.Invoke(() => cb_sort.SelectedIndex == 1))
                        {
                            Dispatcher.Invoke(() => DataGrid_Shipments.ItemsSource = _shipmentController.GetShipmentsFactReg(DatePicker.SelectedDate ?? default));
                            Dispatcher.Invoke(() => DataGrid_Shipments.Items.Refresh());
                        }
                        else if (Dispatcher.Invoke(() => cb_sort.SelectedIndex == 2))
                        {
                            Dispatcher.Invoke(() => DataGrid_Shipments.ItemsSource = _shipmentController.GetShipmentsArrive(DatePicker.SelectedDate ?? default));
                            Dispatcher.Invoke(() => DataGrid_Shipments.Items.Refresh());
                        }
                        else if (Dispatcher.Invoke(() => cb_sort.SelectedIndex == 3))
                        {
                            Dispatcher.Invoke(() => DataGrid_Shipments.ItemsSource = _shipmentController.GetShipmentsLeft(DatePicker.SelectedDate ?? default));
                            Dispatcher.Invoke(() => DataGrid_Shipments.Items.Refresh());
                        }
                    }
                }
                Thread.Sleep(Settings.App.Default.RefreshContent);
            }
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_Shipments.ItemsSource = null;
            if (DatePicker.SelectedDate.HasValue)
                Text_date.Text = "Реестр за " + DatePicker.SelectedDate.Value.ToShortDateString();
            btn_refresh_Click(sender, e);
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_search.Text)) return;
            if (DatePicker.SelectedDate == null) return;
            
            DataGrid_Shipments.ItemsSource = null;
            DataGrid_Shipments.ItemsSource = cb_sort.SelectedIndex switch
            {
                0 => _shipmentController.GetShipments(DatePicker.SelectedDate.Value),
                1 => _shipmentController.GetShipmentsFactReg(DatePicker.SelectedDate.Value),
                2 => _shipmentController.GetShipmentsArrive(DatePicker.SelectedDate.Value),
                3 => _shipmentController.GetShipmentsLeft(DatePicker.SelectedDate.Value),
                _ => _shipmentController.GetShipments(DatePicker.SelectedDate.Value)
            };
        }

        private void cb_sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_refresh_Click(sender, e);
        }

        private void tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_search.Text == "")
            {
                btn_refresh_Click(sender, e);
            }
            else
            {
                try
                {
                    DataGrid_Shipments.ItemsSource = null;

                    var allShipments = _shipmentController.GetShipmentsAll();
                    var data = allShipments.Where(t => t.Fio.ToUpper().StartsWith(tb_search.Text.ToUpper())).ToList();
                    var sDop = allShipments.Where(t => t.Fio.ToUpper().Contains(tb_search.Text.ToUpper())).ToList();
                    data.AddRange(sDop);
                    var noDupes = data.Distinct().ToList();
                    DataGrid_Shipments.ItemsSource = noDupes;

                    //TODO: WTF?
                    if (noDupes.Count == 0)
                    {
                        //Ничекго не нашел
                    }
                    else
                    {
                        // Нашел
                    }
                }
                catch (Exception ex)
                {
                    MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.ContentErrorText.ShowAsync();
                        mainWindow.text_debuger.Text = ex.ToString();
                    }
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Forms.AddOrEditShipment addOrEditShipment = new Forms.AddOrEditShipment();
            addOrEditShipment.ShowDialog();
            btn_refresh_Click(sender, e);
        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Shipments;

            if (current != null)
            {
                MessageBoxResult? result = MessageBox.Show("Сменить статус водителя " + current.Fio + " на Загрузка начата?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {

                    try
                    {
                        using RegistrantCoreContext ef = new RegistrantCoreContext();
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                        if (shipment != null) 
                            shipment.IdTimeNavigation.DateTimeLoad = DateTime.Now;
                        ef.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.ContentErrorText.ShowAsync();
                            mainWindow.text_debuger.Text = ex.ToString();
                        };
                    }
                }
            }

            btn_refresh_Click(sender, e);
        }

        private void btn_endload_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Shipments;

            if (current != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Сменить статус водителя " + current.Fio + " на Загрузка окончена?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using RegistrantCoreContext ef = new RegistrantCoreContext();
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                        if (shipment != null) 
                            shipment.IdTimeNavigation.DateTimeEndLoad = DateTime.Now;
                        ef.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.ContentErrorText.ShowAsync();
                            mainWindow.text_debuger.Text = ex.ToString();
                        }
                    }
                }
            }

            btn_refresh_Click(sender, e);
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Shipments;
            if (current != null)
            {
                Forms.AddOrEditShipment addOr = new Forms.AddOrEditShipment(current.IdShipment);
                addOr.ShowDialog();
            }
            btn_refresh_Click(sender, e);
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            switch (App.LevelAccess)
            {
                case "shipment":
                {
                    Forms.PrintShipments print = new Forms.PrintShipments();
                    print.ShowDialog();
                    break;
                }
                case "warehouse":
                {
                    Forms.PrintWarehouse print = new Forms.PrintWarehouse();
                    print.ShowDialog();
                    break;
                }
                default:
                {
                    MessageBoxResult? result = MessageBox.Show("Открыть окно вид для сбыта?", "Внимание",
                        MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (result == MessageBoxResult.Yes)
                    {
                        Forms.PrintShipments print = new Forms.PrintShipments();
                        print.ShowDialog();
                    }
                    else
                    {
                        Forms.PrintWarehouse print = new Forms.PrintWarehouse();
                        print.ShowDialog();
                    }
                    break;
                }
            }
        }
    }
}
