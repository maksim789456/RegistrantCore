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
    public partial class PageKPP
    {
        private ShipmentController _shipmentController;
        private KppShipmentsController _kppShipmentsController;
        private PlanShipmentController _planShipmentController;

        public PageKPP()
        {
            InitializeComponent();
            _kppShipmentsController = new KppShipmentsController();
            _planShipmentController = new PlanShipmentController();
            _shipmentController = new ShipmentController();

            DatePicker.SelectedDate = DateTime.Now;

            Thread thread = new Thread(RefreshThread);
            thread.Start();
        }

        void RefreshThread()
        {
            while (true)
            {
                Thread.Sleep(Settings.App.Default.RefreshContent);
                Dispatcher.Invoke(() => DataGrid_Plan.ItemsSource = _planShipmentController.GetPlanShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate.Value)));

                //Пока что так, так как охрана не умеет видимо пролистывать календарик
                //Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = kPP.GetShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate.Value)));

                //Но следующий метод просто выведет ВСЕ зарегистрированные текущие водители 
                Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = _kppShipmentsController.GetShipments());

                Dispatcher.Invoke(() => DataGrid_Plan.Items.Refresh());
                Dispatcher.Invoke(() => DataGrid_Drivers.Items.Refresh());
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAdd.Hide();
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => DataGrid_Plan.ItemsSource = null);
            Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = null);

            Dispatcher.Invoke(() => DataGrid_Plan.ItemsSource = _planShipmentController.GetPlanShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate.Value)));

            //Пока что так, так как охрана не умеет видимо пролистывать календарик
            //Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = kPP.GetShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate.Value)));

            //Но следующий метод просто выведет ВСЕ зарегистрированные текущие водители 
            Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = _kppShipmentsController.GetShipments());
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_plan.Text = "Запланированные отгрузки за " + DatePicker.SelectedDate.Value.ToShortDateString();
            btn_refresh_Click(sender, e);
        }

        private void btn_arrive_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as KppShipments;
            if (current == null) return;
            MessageBoxResult? result = MessageBox.Show(
                "Сменить статус водителя " + current.Fio + " на Прибыл?", "Внимание", MessageBoxButton.YesNo,
                MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                if (shipment != null)
                    shipment.IdTimeNavigation.DateTimeArrive = DateTime.Now;
                ef.SaveChanges();
                btn_refresh_Click(sender, e);
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

        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as KppShipments;
            if (current == null) return;
            MessageBoxResult? result =
                MessageBox.Show("Сменить статус водителя " + current.Fio + " на Покинул склад?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                if (shipment != null)
                    shipment.IdTimeNavigation.DateTimeLeft = DateTime.Now;
                ef.SaveChanges();
                btn_refresh_Click(sender, e);
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

        private void btn_adddriver_Click(object sender, RoutedEventArgs e)
        {
            LoadDrvAndContragents();
            ClearTextboxes();
            cb_drivers.Text = "";
            ContentAdd.ShowAsync();
        }

        void ClearTextboxes()
        {
            tb_autonum.Clear();
            tb_info.Clear();
            tb_passport.Clear();
            tb_phone.Clear();
            tb_attorney.Clear();
        }

        private void btn_registration_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as PlanShipment;
            if (current == null) return;
            MessageBoxResult? result =
                MessageBox.Show("Сменить статус водителя " + current.Fio + " на Зарегистрирован?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                if (shipment != null)
                    shipment.IdTimeNavigation.DateTimeFactRegist = DateTime.Now;
                ef.SaveChanges();
                btn_refresh_Click(sender, e);
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

        private void btn_add_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAdd.Hide();
            btn_refresh_Click(sender, e);
        }

        private void btn_add_add_Click(object sender, RoutedEventArgs e)
        {
            if (cb_drivers.Text == "")
            {
                return;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                Shipment shipment = new Shipment();

                if (cb_drivers.SelectedItem != null)
                {
                    var current = cb_drivers.SelectedItem as Drivers;
                    shipment.IdDriver = current?.IdDriver;
                }
                else
                {
                    //Если водителя нет в списках
                    var splitNames = SplitNames(cb_drivers.Text + " ");
                    Driver driver = new Driver
                    {
                        Name = splitNames.name.Replace(" ", ""),
                        Family = splitNames.family.Replace(" ", ""),
                        Patronymic = splitNames.patronomyc.Replace(" ", ""),
                        AutoNumber = tb_autonum.Text,
                        Attorney = tb_attorney.Text,
                        Phone = tb_phone.Text,
                        Passport = tb_passport.Text,
                        Active = "1",
                        ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил водителя"
                    };
                    shipment.IdDriverNavigation = driver;
                }

                Time time = new Time
                {
                    DateTimeFactRegist = DateTime.Now
                };
                shipment.IdTimeNavigation = time;

                shipment.Description = tb_info.Text;
                shipment.Active = "1";
                shipment.ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил отгрузку";

                ef.Add(shipment);
                ef.SaveChanges();
                ContentAdd.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Разбив фио
        static (string family, string name, string patronomyc) SplitNames(string FullName)
        {
            var partsName = FullName.Split(' ');
            return (partsName[0], partsName[1], partsName[2]);
        }

        private void cb_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = cb_drivers.SelectedItem as Drivers;

            if (current != null)
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    var temp = ef.Drivers.FirstOrDefault(x => x.IdDriver == current.IdDriver);

                    //tb_contragent.Text = temp.IdContragentNavigation?.Name;
                    tb_phone.Text = temp.Phone;
                    tb_autonum.Text = temp.AutoNumber;
                    tb_passport.Text = temp.Passport;
                    tb_attorney.Text = temp.Attorney;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                ClearTextboxes();
            }

        }

        void LoadDrvAndContragents()
        {
            using RegistrantCoreContext ef = new RegistrantCoreContext();
            DriversController driver = new DriversController();
            cb_drivers.ItemsSource = driver.GetDriversCurrent();
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            ContentSeach.ShowAsync();
        }

        private void tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_search.Text == "")
            {
                datagrid_search.ItemsSource = null;
                return;
            }
            
            try
            {
                datagrid_search.ItemsSource = null;

                var temp = _shipmentController.GetShipmentsWhoNoLeft();
                var data = temp.Where(t => t.Fio.ToUpper().StartsWith(tb_search.Text.ToUpper())).ToList();
                var sDOP = temp.Where(t => t.Fio.ToUpper().Contains(tb_search.Text.ToUpper())).ToList();
                data.AddRange(sDOP);
                var noDupes = data.Distinct().ToList();
                datagrid_search.ItemsSource = noDupes;

                if (noDupes.Count == 0)
                {
                    //Ничекго не нашел
                }
                else
                {
                    // Нашел
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_search_close_Click(object sender, RoutedEventArgs e)
        {
            ContentSeach.Hide();
        }
    }
}