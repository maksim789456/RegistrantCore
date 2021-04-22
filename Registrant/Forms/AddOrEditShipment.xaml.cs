using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;
using Registrant.Models;

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
            btn_delete.Visibility = Visibility.Collapsed;
            LoadDrvAndContragents();
        }

        //Разбив фио
        static (string family, string name, string patronomyc) SplitNames(string fullName)
        {
            var partsName = fullName.Split(' ');
            return (partsName[0], partsName[1], partsName[2]);
        }

        /// Добавить соответственно
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (cb_drivers.Text == "")
            {
                MessageBox.Show("Водитель не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (cb_contragent.Text == "")
            {
                MessageBox.Show("Контрагент не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (dt_plan.Value == null)
            {
                MessageBox.Show("Дата плановой загрузки не заполнена!", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                        Active = "1",
                        ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил водителя"
                    };

                    shipment.IdDriverNavigation = driver;
                }

                if (cb_contragent.SelectedItem != null)
                {
                    var current = cb_contragent.SelectedItem as Contragent;
                    shipment.IdContragent = current?.IdContragent;
                }
                else
                {
                    Contragent contragent = new Contragent
                    {
                        Name = cb_contragent.Text,
                        Active = "1",
                        ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил контрагента"
                    };
                    shipment.IdContragentNavigation = contragent;
                }
                
                Time time = new Time
                {
                    //При добавлении можно сохранять прям DateTime? не боясь за null-ы
                    DateTimePlanRegist = dt_plan.Value,
                    DateTimeFactRegist = dt_fact.Value,
                    DateTimeArrive = dt_arrive.Value,
                    DateTimeLoad = dt_load.Value,
                    DateTimeEndLoad = dt_endload.Value,
                    DateTimeLeft = dt_left.Value
                };

                shipment.IdTimeNavigation = time;

                if (string.IsNullOrWhiteSpace(tb_numrealese.Text))
                {
                    shipment.NumRealese = tb_numrealese.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                {
                    shipment.PacketDocuments = tb_packetdoc.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_tochkaload.Text))
                {
                    shipment.TochkaLoad = tb_tochkaload.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                {
                    shipment.CountPodons = tb_CountPodons.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                {
                    shipment.Nomenclature = tb_nomencluture.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_size.Text))
                {
                    shipment.Size = tb_size.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_Destination.Text))
                {
                    shipment.Destination = tb_Destination.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_typeload.Text))
                {
                    shipment.TypeLoad = tb_typeload.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_descript.Text))
                {
                    shipment.Description = tb_descript.Text;
                }

                if (string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                {
                    shipment.StoreKeeper = tb_storekeeper.Text;
                }

                shipment.Active = "1";
                shipment.ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил отгрузку";

                ef.Add(shipment);
                ef.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// Выбор водителя
        private void cb_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = cb_drivers.SelectedItem as Drivers;
            if (current == null)
            {
                tb_phone.Text = "";
                tb_autonum.Text = "";
                return;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var driver = ef.Drivers.FirstOrDefault(x => x.IdDriver == current.IdDriver);

                if (driver != null)
                {
                    tb_phone.Text = driver.Phone;
                    tb_autonum.Text = driver.AutoNumber;
                    tb_attorney.Text = driver.Attorney;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Подгрузка водителей
        void LoadDrvAndContragents()
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                Controllers.DriversController driver = new Controllers.DriversController();

                cb_drivers.ItemsSource = driver.GetDriversCurrent();
                cb_contragent.ItemsSource = ef.Contragents.Where(x => x.Active != "0").OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Редактирование отгрузок
        public AddOrEditShipment(int id)
        {
            InitializeComponent();
            //LoadDriversBox();
            btn_add.Visibility = Visibility.Collapsed;
            idcont.Text = id.ToString();

            switch (App.LevelAccess)
            {
                case "shipment":
                    dt_load.IsEnabled = false;
                    dt_endload.IsEnabled = false;
                    tb_CountPodons.IsEnabled = false;
                    tb_size.IsEnabled = false;
                    tb_nomencluture.IsEnabled = false;
                    tb_Destination.IsEnabled = false;
                    tb_typeload.IsEnabled = false;
                    tb_storekeeper.IsEnabled = false;
                    tb_descript.IsEnabled = true;
                    break;
                case "warehouse":
                    dt_plan.IsEnabled = false;
                    dt_fact.IsEnabled = false;
                    dt_arrive.IsEnabled = false;
                    dt_left.IsEnabled = false;

                    tb_numrealese.IsEnabled = false;
                    tb_packetdoc.IsEnabled = false;
                    tb_tochkaload.IsEnabled = false;
                    tb_typeload.IsEnabled = true;

                    btn_delete.Visibility = Visibility.Collapsed;
                    break;
                case "admin":
                    break;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == id);

                Controllers.DriversController driver = new Controllers.DriversController();
                if (shipment != null)
                {
                    cb_drivers.ItemsSource = driver.GetDriversCurrent(shipment.IdDriver ?? -1);
                    cb_drivers.SelectedItem = driver.Driver.FirstOrDefault(x => x.IdDriver == shipment.IdDriver);

                    cb_contragent.ItemsSource = ef.Contragents
                        .Where(x => x.Active != "0" || x.IdContragent == shipment.IdContragent).OrderBy(x => x.Name)
                        .ToList();
                    cb_contragent.SelectedItem =
                        ef.Contragents.FirstOrDefault(x => x.IdContragent == shipment.IdContragent);

                    dt_plan.Value = shipment.IdTimeNavigation.DateTimePlanRegist;
                    dt_fact.Value = shipment.IdTimeNavigation.DateTimeFactRegist;
                    dt_arrive.Value = shipment.IdTimeNavigation.DateTimeArrive;
                    dt_load.Value = shipment.IdTimeNavigation.DateTimeLoad;
                    dt_endload.Value = shipment.IdTimeNavigation.DateTimeEndLoad;
                    dt_left.Value = shipment.IdTimeNavigation.DateTimeLeft;

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (dt_plan.Value != null)
            {
                text_title.Text = "Отгрузка №" + id + " от " + dt_plan.Value;
            }
            else
            {
                text_title.Text = "Отгрузка №" + id;
            }
        }

        /// Кнопка редактировать
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            switch (App.LevelAccess)
            {
                case "shipment":
                    EditByShipmentRole();
                    break;
                case "warehouse":
                    EditByWarehouseRole();
                    break;
                case "admin":
                    EditByAdminRole();
                    break;
            }
        }

        private void EditByShipmentRole()
        {
            if (cb_drivers.Text == "")
            {
                MessageBox.Show("Водитель не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cb_contragent.Text == "")
            {
                MessageBox.Show("Контрагент не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment =
                    ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));

                if (shipment != null)
                {
                    if (cb_drivers.SelectedItem != null)
                    {
                        var current = cb_drivers.SelectedItem as Drivers;
                        shipment.IdDriver = current?.IdDriver;
                    }
                    else
                    {
                        var splitNames = SplitNames(cb_drivers.Text + " ");
                        //Если водителя нет в списках
                        Driver driver = new Driver
                        {
                            Name = splitNames.name.Replace(" ", ""),
                            Family = splitNames.family.Replace(" ", ""),
                            Patronymic = splitNames.patronomyc.Replace(" ", ""),
                            AutoNumber = tb_autonum.Text,
                            Attorney = tb_attorney.Text,
                            Phone = tb_phone.Text,
                            Active = "1",
                            ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил водителя"
                        };
                        
                        shipment.IdDriverNavigation = driver;
                    }

                    if (cb_contragent.SelectedItem != null)
                    {
                        var current = cb_contragent.SelectedItem as Contragent;
                        shipment.IdContragent = current?.IdContragent;
                    }
                    else
                    {
                        Contragent contragent = new Contragent
                        {
                            Name = cb_contragent.Text,
                            Active = "1",
                            ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил контрагента"
                        };
                        shipment.IdContragentNavigation = contragent;
                    }

                    if (dt_plan.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimePlanRegist = dt_plan.Value;
                    }

                    if (dt_fact.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeFactRegist = dt_fact.Value;
                    }

                    if (dt_arrive.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeArrive = dt_arrive.Value;
                    }

                    if (dt_left.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeLeft = dt_left.Value;
                    }

                    if (string.IsNullOrWhiteSpace(tb_numrealese.Text))
                    {
                        shipment.NumRealese = tb_numrealese.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                    {
                        shipment.PacketDocuments = tb_packetdoc.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_typeload.Text))
                    {
                        shipment.TypeLoad = tb_typeload.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_tochkaload.Text))
                    {
                        shipment.TochkaLoad = tb_tochkaload.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_descript.Text))
                    {
                        shipment.Description = tb_descript.Text;
                    }


                    shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser +
                                           " внес изменения в отгрузку";
                    ef.SaveChanges();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void EditByWarehouseRole()
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));

                if (shipment != null)
                {
                    if (dt_load.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeLoad = dt_load.Value;
                    }

                    if (dt_endload.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeEndLoad = dt_endload.Value;
                    }

                    if (string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                    {
                        shipment.CountPodons = tb_CountPodons.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                    {
                        shipment.Nomenclature = tb_nomencluture.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_size.Text))
                    {
                        shipment.Size = tb_size.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_Destination.Text))
                    {
                        shipment.Destination = tb_Destination.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_typeload.Text))
                    {
                        shipment.TypeLoad = tb_typeload.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_descript.Text))
                    {
                        shipment.Description = tb_descript.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                    {
                        shipment.StoreKeeper = tb_storekeeper.Text;
                    }
                    else
                    {
                        shipment.StoreKeeper = App.ActiveUser;
                    }
                    
                    shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser +
                                           " внес изменения в отгрузку";
                    ef.SaveChanges();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditByAdminRole()
        {
            if (cb_drivers.Text == "")
            {
                MessageBox.Show("Водитель не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (cb_contragent.Text == "")
            {
                MessageBox.Show("Контрагент не выбран", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment =
                    ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt64(idcont.Text));

                if (shipment != null)
                {
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
                            Active = "1",
                            ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил водителя"
                        };
                        shipment.IdDriverNavigation = driver;
                    }

                    if (cb_contragent.SelectedItem != null)
                    {
                        var current = cb_contragent.SelectedItem as Contragent;
                        shipment.IdContragent = current?.IdContragent;
                    }
                    else
                    {
                        Contragent contragent = new Contragent
                        {
                            Name = cb_contragent.Text,
                            Active = "1",
                            ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил контрагента"
                        };
                        shipment.IdContragentNavigation = contragent;
                    }

                    if (dt_plan.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimePlanRegist = dt_plan.Value;
                    }

                    if (dt_fact.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeFactRegist = dt_fact.Value;
                    }

                    if (dt_arrive.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeArrive = dt_arrive.Value;
                    }

                    if (dt_load.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeLoad = dt_load.Value;
                    }

                    if (dt_endload.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeEndLoad = dt_endload.Value;
                    }

                    if (dt_left.Value.HasValue)
                    {
                        shipment.IdTimeNavigation.DateTimeLeft = dt_left.Value;
                    }

                    if (string.IsNullOrWhiteSpace(tb_numrealese.Text))
                    {
                        shipment.NumRealese = tb_numrealese.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_packetdoc.Text))
                    {
                        shipment.PacketDocuments = tb_packetdoc.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_tochkaload.Text))
                    {
                        shipment.TochkaLoad = tb_tochkaload.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_CountPodons.Text))
                    {
                        shipment.CountPodons = tb_CountPodons.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_nomencluture.Text))
                    {
                        shipment.Nomenclature = tb_nomencluture.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_size.Text))
                    {
                        shipment.Size = tb_size.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_Destination.Text))
                    {
                        shipment.Destination = tb_Destination.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_typeload.Text))
                    {
                        shipment.TypeLoad = tb_typeload.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_descript.Text))
                    {
                        shipment.Description = tb_descript.Text;
                    }

                    if (string.IsNullOrWhiteSpace(tb_storekeeper.Text))
                    {
                        shipment.StoreKeeper = tb_storekeeper.Text;
                    }

                    shipment.Active = "1";
                    shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser +
                                           " внес изменения в отгрузку";
                }
                ef.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            ContentConfirmDel.ShowAsync();
        }

        private void btn_del_yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt32(idcont.Text));
                if (shipment != null)
                {
                    shipment.Active = "0";
                    shipment.Description = shipment.Description + " " + tb_reasofordel.Text;
                    shipment.ServiceInfo = shipment.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser +
                                           " удалил отгрузку";
                }

                ef.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_del_close_Click(object sender, RoutedEventArgs e)
        {
            ContentConfirmDel.Hide();
        }
    }
}