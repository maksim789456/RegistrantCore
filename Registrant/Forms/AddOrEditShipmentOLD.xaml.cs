﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;
using Registrant.Models;

namespace Registrant.Forms
{
    public partial class AddOrEditShipmentOLD
    {
        /// Новая отгрузка
        public AddOrEditShipmentOLD()
        {
            InitializeComponent();
            
            text_title.Text = "Добавление отгрузки";
            btn_edit.Visibility = Visibility.Collapsed;
            LoadDriversBox();

        }

        /// Выбор водителя
        private void cb_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = cb_drivers.SelectedItem as Drivers;

            if (current != null)
            {
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
            Controllers.DriversController driver = new Controllers.DriversController();
            cb_drivers.ItemsSource = driver.GetDriversCurrent();
        }

        /// Редактирование отгрузок
        public AddOrEditShipmentOLD(int id)
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
                    tb_descript.IsEnabled = false;
                    break;
                case "warehouse":
                    dt_plan.IsEnabled = false;
                    dt_fact.IsEnabled = false;
                    dt_arrive.IsEnabled = false;
                    dt_left.IsEnabled = false;

                    tb_numrealese.IsEnabled = false;
                    tb_packetdoc.IsEnabled = false;
                    tb_tochkaload.IsEnabled = false;
                    break;
                case "admin":
                    break;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var temp = ef.Shipments.FirstOrDefault(x => x.IdShipment == id);

                Controllers.DriversController driver = new Controllers.DriversController();

                if (temp != null)
                {
                    cb_drivers.ItemsSource = driver.GetDriversCurrent(temp.IdDriver ?? -1);
                    cb_drivers.SelectedItem = driver.Driver.FirstOrDefault(x => x.IdDriver == temp.IdDriver);

                    //ЗАПРЕТ НА РЕДАКТИРОВАНИЕ ЕСЛИ НАЧАЛАСЬ ЗАГРУЗКА
                    if (temp.IdTimeNavigation.DateTimeLoad != null)
                    {
                        if (App.LevelAccess != "admin")
                        {
                            cb_drivers.IsEnabled = false;
                        }
                    }

                    dt_plan.Value = temp.IdTimeNavigation?.DateTimePlanRegist;
                    dt_fact.Value = temp.IdTimeNavigation?.DateTimeFactRegist;
                    dt_arrive.Value = temp.IdTimeNavigation?.DateTimeArrive;
                    dt_load.Value = temp.IdTimeNavigation?.DateTimeLoad;
                    dt_endload.Value = temp.IdTimeNavigation?.DateTimeEndLoad;
                    dt_left.Value = temp.IdTimeNavigation?.DateTimeLeft;

                    tb_numrealese.Text = temp.NumRealese;
                    tb_packetdoc.Text = temp.PacketDocuments;
                    tb_tochkaload.Text = temp.TochkaLoad;


                    tb_CountPodons.Text = temp.CountPodons;
                    tb_nomencluture.Text = temp.Nomenclature;
                    tb_size.Text = temp.Size;
                    tb_Destination.Text = temp.Destination;
                    tb_typeload.Text = temp.TypeLoad;
                    tb_descript.Text = temp.Description;
                    tb_storekeeper.Text = temp.StoreKeeper;
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
            if (App.LevelAccess == "shipment")
            {
                try
                {
                    using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt64(idcont.Text));

                        if (cb_drivers.SelectedItem != null)
                        {
                            var test = cb_drivers as ComboBox;
                            var current = test.SelectedItem as DB.Driver;
                            shipment.IdDriver = current.IdDriver;
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

                        if (tb_numrealese.Text != null)
                        {
                            shipment.NumRealese = tb_numrealese.Text;
                        }
                        if (tb_packetdoc.Text != null)
                        {
                            shipment.PacketDocuments = tb_packetdoc.Text;
                        }
                        if (tb_typeload.Text != null)
                        {
                            shipment.TypeLoad = tb_typeload.Text;
                        }
                        if (tb_tochkaload.Text != null)
                        {
                            shipment.TochkaLoad = tb_tochkaload.Text;
                        }

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
            else if (App.LevelAccess == "warehouse")
            {
                try
                {
                    using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt64(idcont.Text));

                        
                        if (dt_load.Value != null)
                        {
                            shipment.IdTimeNavigation.DateTimeLoad = dt_load.Value;
                        }
                        if (dt_endload.Value != null)
                        {
                            shipment.IdTimeNavigation.DateTimeEndLoad = dt_endload.Value;
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

                        shipment.StoreKeeper = App.ActiveUser;
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
            else if (App.LevelAccess == "admin")
            {
                try
                {
                    using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                    {
                        var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == Convert.ToInt64(idcont.Text));

                        if (cb_drivers.SelectedItem != null)
                        {
                            var test = cb_drivers as ComboBox;
                            var current = test.SelectedItem as Models.Drivers;
                            shipment.IdDriver = current.IdDriver;
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

                        if (tb_numrealese.Text != null)
                        {
                            shipment.NumRealese = tb_numrealese.Text;
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
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    DB.Shipment shipment = new DB.Shipment();

                    if (cb_drivers.SelectedItem != null)
                    {
                        var test = cb_drivers as ComboBox;
                        var current = test.SelectedItem as Models.Drivers;
                        shipment.IdDriver = current.IdDriver;
                    }

                    DB.Time time = new DB.Time();
                    shipment.IdTimeNavigation = time;

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

                    if (tb_numrealese.Text != null)
                    {
                        shipment.NumRealese = tb_numrealese.Text;
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
                    shipment.ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил отгрузку";


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
            this.Close();
        }
    }
}
