using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;

namespace Registrant.Forms
{
    public partial class AddOrEditShipment
    {
        /// Новая отгрузка
        public AddOrEditShipment()
        {
            InitializeComponent();
            
            text_title.Text = "Добавление отгрузки";
            btn_edit.Visibility = Visibility.Collapsed;
            LoadDriversBox();
        }

        /// Выбор водителя
        private void cb_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_drivers.SelectedItem is Driver current)
            {
                try
                {
                    using (RegistrantCoreContext ef = new RegistrantCoreContext())
                    {
                        var driver = ef.Drivers.FirstOrDefault(x => x.IdDriver == current.IdDriver);
                        if (driver != null)
                        {
                            tb_contragent.Text = driver.IdContragentNavigation.Name;
                            tb_phone.Text = driver.Phone;
                            tb_autonum.Text = driver.AutoNumber; 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                tb_contragent.Text = "";
                tb_phone.Text = "";
                tb_autonum.Text = "";
            }
            
        }

        /// Подгрузка водителей
        void LoadDriversBox()
        {
            try
            {
                using (RegistrantCoreContext ef = new RegistrantCoreContext())
                {
                    Controllers.DriversController driver = new Controllers.DriversController();

                    cb_drivers.ItemsSource = driver.GetDriversСurrent();

                }
            }
            catch (Exception exception)
            {
                ModernWpf.MessageBox.Show(exception.Message, "Ошибка!");
            }
        }



        /// Редактирование отгрузок
        public AddOrEditShipment(int id)
        {
            InitializeComponent();
            //LoadDriversBox();
            btn_add.Visibility = Visibility.Collapsed;
            idcont.Text = id.ToString();

            if (App.LevelAccess == "shipment")
            {
                dt_load.IsEnabled = false;
                dt_endload.IsEnabled = false;
                tb_CountPodons.IsEnabled = false;
                tb_size.IsEnabled = false;
                tb_nomencluture.IsEnabled = false;
                tb_Destination.IsEnabled = false;
                tb_typeload.IsEnabled = false;
                tb_descript.IsEnabled = false;
            }
            else if (App.LevelAccess == "warehouse")
            {
                dt_plan.IsEnabled = false;
                dt_fact.IsEnabled = false;
                dt_arrive.IsEnabled = false;
                dt_left.IsEnabled = false;

                tb_numrealese.IsEnabled = false;
                tb_packetdoc.IsEnabled = false;
                tb_tochkaload.IsEnabled = false;
            }
            else if (App.LevelAccess == "admin")
            {
            }

            try
            {
                using (RegistrantCoreContext ef = new RegistrantCoreContext())
                {
                    var temp = ef.Shipments.FirstOrDefault(x => x.IdShipment == id);

                    Controllers.DriversController driver = new Controllers.DriversController();

                    cb_drivers.ItemsSource = driver.GetDriversСurrent((int)temp.IdDriver);
                    cb_drivers.SelectedItem = driver.Driver.FirstOrDefault(x => x.IdDriver == temp.IdDriver);

                    //ЗАПРЕТ НА РЕДАКТИРОВАНИЕ ЕСЛИ НАЧАЛАСЬ ЗАГРУЗКА
                    if (temp.IdTimeNavigation.DateTimeLoad != null)
                    {
                        if (App.LevelAccess != "admin")
                        {
                            cb_drivers.IsEnabled = false;
                        }
                    }

                        if (shipment.IdTimeNavigation.DateTimeLoad != null)
                        {
                            cb_drivers.IsEnabled = false;
                        }

                        dt_plan.Value = shipment.IdTimeNavigation?.DateTimePlanRegist;
                        dt_fact.Value = shipment.IdTimeNavigation?.DateTimeFactRegist;
                        dt_arrive.Value = shipment.IdTimeNavigation?.DateTimeArrive;
                        dt_load.Value = shipment.IdTimeNavigation?.DateTimeLoad;
                        dt_endload.Value = shipment.IdTimeNavigation?.DateTimeEndLoad;
                        dt_left.Value = shipment.IdTimeNavigation?.DateTimeLeft;

                        tb_numrealese.Text = shipment.NumRealese;
                        tb_packetdoc.Text = shipment.PacketDocuments;
                        tb_tochkaload.Text = shipment.TochkaLoad;

                        tb_CountPodons.Text = shipment.CountPodons;
                        tb_nomencluture.Text = shipment.Nomenclature;
                        tb_size.Text = shipment.Size;
                        tb_Destination.Text = shipment.Destination;
                        tb_typeload.Text = shipment.TypeLoad;
                        tb_descript.Text = shipment.Description;
                        tb_storekeeper.Text = shipment.StoreKeeper;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error); }

            if (dt_plan.Value != null)
            {
                text_title.Text = $"Отгрузка №{id} от {dt_plan.Value}";
            }
            else
            {
                text_title.Text = $"Отгрузка №{id}";
            }
        }

        /// Кнопка редактировать

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (App.LevelAccess == "shipment")
            {
                try
                {
                    using (RegistrantCoreContext ef = new RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));

                        if (shipment != null)
                        {
                            if (cb_drivers.SelectedItem != null)
                            {
                                var current = cb_drivers.SelectedItem as Driver;
                                shipment.IdDriver = current?.IdDriver;
                            }

                            if (dt_plan.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimePlanRegist = dt_plan.Value;
                            }
                            if (dt_fact.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeFactRegist = dt_fact.Value;
                            }
                            if (dt_arrive.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeArrive = dt_arrive.Value;
                            }

                            if (dt_left.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeLeft = dt_left.Value;
                            }

                            if (!string.IsNullOrWhiteSpace(tb_numrealese.Text))
                            {
                                shipment.NumRealese = tb_numrealese.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                            {
                                shipment.PacketDocuments = tb_packetdoc.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_typeload.Text))
                            {
                                shipment.TypeLoad = tb_typeload.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_tochkaload.Text))
                            {
                                shipment.TochkaLoad = tb_tochkaload.Text;
                            }

                            shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser + " внес изменения в отгрузку";
                            ef.SaveChanges();
                        }
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (App.LevelAccess == "warehouse")
            {
                try
                {
                    using (RegistrantCoreContext ef = new RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));

                        if (shipment != null)
                        {
                            if (dt_load.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeLoad = dt_load.Value;
                            }
                            if (dt_endload.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeEndLoad = dt_endload.Value;
                            }

                            if (!string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                            {
                                shipment.CountPodons = tb_CountPodons.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                            {
                                shipment.Nomenclature = tb_nomencluture.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_size.Text))
                            {
                                shipment.Size = tb_size.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_Destination.Text))
                            {
                                shipment.Destination = tb_Destination.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_typeload.Text))
                            {
                                shipment.TypeLoad = tb_typeload.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_descript.Text))
                            {
                                shipment.Description = tb_descript.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                            {
                                shipment.StoreKeeper = tb_storekeeper.Text;
                            }

                            shipment.StoreKeeper = App.ActiveUser;
                            shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser + " внес изменения в отгрузку";
                            ef.SaveChanges();
                        }
                        Close();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else if (App.LevelAccess == "admin")
            {
                try
                {
                    using (RegistrantCoreContext ef = new RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));

                        if (shipment != null)
                        {
                            if (cb_drivers.SelectedItem != null)
                            {
                                var current = cb_drivers.SelectedItem as Driver;
                                shipment.IdDriver = current?.IdDriver;
                            }

                            if (dt_plan.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimePlanRegist = dt_plan.Value;
                            }
                            if (dt_fact.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeFactRegist = dt_fact.Value;
                            }
                            if (dt_arrive.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeArrive = dt_arrive.Value;
                            }
                            if (dt_load.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeLoad = dt_load.Value;
                            }
                            if (dt_endload.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeEndLoad = dt_endload.Value;
                            }
                            if (dt_left.Value != null)
                            {
                                shipment.IdTimeNavigation.DateTimeLeft = dt_left.Value;
                            }

                            if (!string.IsNullOrWhiteSpace(tb_numrealese.Text))
                            {
                                shipment.NumRealese = tb_numrealese.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                            {
                                shipment.PacketDocuments = tb_packetdoc.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_typeload.Text))
                            {
                                shipment.TochkaLoad = tb_typeload.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                            {
                                shipment.CountPodons = tb_CountPodons.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                            {
                                shipment.Nomenclature = tb_nomencluture.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_size.Text))
                            {
                                shipment.Size = tb_size.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_Destination.Text))
                            {
                                shipment.Destination = tb_Destination.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_typeload.Text))
                            {
                                shipment.TypeLoad = tb_typeload.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_descript.Text))
                            {
                                shipment.Description = tb_descript.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                            {
                                shipment.StoreKeeper = tb_storekeeper.Text;
                            }
                            
                            shipment.Active = "1";
                            shipment.ServiceInfo = $"{shipment.ServiceInfo}\n" +
                                                   $"{DateTime.Now} {App.ActiveUser} внес изменения в отгрузку";
                            ef.SaveChanges();
                        }
                        if (tb_packetdoc.Text != null)
                        {
                            shipment.PacketDocuments = tb_packetdoc.Text;
                        }
                        if (tb_tochkaload.Text != null)
                        {
                            shipment.TochkaLoad = tb_tochkaload.Text;
                        }
                        if (tb_CountPodons.Text != null)
                        {
                            shipment.CountPodons = tb_CountPodons.Text;
                        }
                        if (tb_nomencluture.Text != null)
                        {
                            shipment.Nomenclature = tb_nomencluture.Text;
                        }
                        if (tb_size.Text != null)
                        {
                            shipment.Size = tb_size.Text;
                        }
                        if (tb_Destination.Text != null)
                        {
                            shipment.Destination = tb_Destination.Text;
                        }
                        if (tb_typeload.Text != null)
                        {
                            shipment.TypeLoad = tb_typeload.Text;
                        }
                        if (tb_descript.Text != null)
                        {
                            shipment.Description = tb_descript.Text;
                        }
                        if (tb_storekeeper.Text != null)
                        {
                            shipment.StoreKeeper = tb_storekeeper.Text;
                        }

                        shipment.Active = "1";
                        shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser + " внес изменения в отгрузку";
                        ef.SaveChanges();
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// Добавить соответственно
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (RegistrantCoreContext ef = new RegistrantCoreContext())
                {
                    Shipment shipment = new Shipment();

                    if (cb_drivers.SelectedItem != null)
                    {
                        var current = cb_drivers.SelectedItem as Driver;
                        shipment.IdDriver = current?.IdDriver;
                    }
                    Time time = new Time();

                    if (dt_plan.Value != null)
                    {
                        time.DateTimePlanRegist = dt_plan.Value;
                    }
                    if (dt_fact.Value != null)
                    {
                        time.DateTimeFactRegist = dt_fact.Value;
                    }
                    if (dt_arrive.Value != null)
                    {
                        time.DateTimeArrive = dt_arrive.Value;
                    }
                    if (dt_load.Value != null)
                    {
                        time.DateTimeLoad = dt_load.Value;
                    }
                    if (dt_endload.Value != null)
                    {
                        time.DateTimeEndLoad = dt_endload.Value;
                    }
                    if (dt_left.Value != null)
                    {
                        time.DateTimeLeft = dt_left.Value;
                    }
                    shipment.IdTimeNavigation = time;

                    if (!string.IsNullOrWhiteSpace(tb_numrealese.Text))
                    {
                        shipment.NumRealese = tb_numrealese.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                    {
                        shipment.PacketDocuments = tb_packetdoc.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_tochkaload.Text))
                    {
                        shipment.TochkaLoad = tb_tochkaload.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                    {
                        shipment.CountPodons = tb_CountPodons.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                    {
                        shipment.Nomenclature = tb_nomencluture.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_size.Text))
                    {
                        shipment.Size = tb_size.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_Destination.Text))
                    {
                        shipment.Destination = tb_Destination.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_typeload.Text))
                    {
                        shipment.TypeLoad = tb_typeload.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_descript.Text))
                    {
                        shipment.Description = tb_descript.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                    {
                        shipment.StoreKeeper = tb_storekeeper.Text;
                    }

                    shipment.Active = "1";
                    shipment.ServiceInfo = $"{DateTime.Now} {App.ActiveUser} добавил отгрузку";

                    ef.Add(shipment);
                    ef.SaveChanges();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
