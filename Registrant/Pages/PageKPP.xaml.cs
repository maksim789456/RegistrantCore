﻿using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ModernWpf.MessageBox;
using Registrant.Models;

namespace Registrant.Pages
{
    public partial class PageKPP
    {
        Controllers.KppShipmentsController kPP;
        Controllers.PlanShipmentController plan;

        public PageKPP()
        {
            InitializeComponent();
            kPP = new Controllers.KppShipmentsController();
            plan = new Controllers.PlanShipmentController();

            DatePicker.SelectedDate = DateTime.Now;

            Thread thread = new Thread(RefreshThread);
            thread.Start();
        }

        void RefreshThread()
        {
            while (true)
            {
                Thread.Sleep(Settings.App.Default.RefreshContent);
                Dispatcher.Invoke(() => DataGrid_Plan.ItemsSource = plan.GetPlanShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate ?? default)));
                Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = kPP.GetShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate ?? default)));
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

            Dispatcher.Invoke(() => DataGrid_Plan.ItemsSource = plan.GetPlanShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate ?? default)));
            Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = kPP.GetShipments(Dispatcher.Invoke(() => DatePicker.SelectedDate ?? default)));
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_refresh_Click(sender, e);
        }

        private void btn_arrive_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt.DataContext as KppShipments;

            if (current != null)
            {
                MessageBoxResult? result = ModernWpf.MessageBox.Show(
                    "Сменить статус водителя " + current.FIO + " на Прибыл?", "Внимание", MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                        {
                            var temp = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                            if (temp != null) 
                                temp.IdTimeNavigation.DateTimeArrive = DateTime.Now;
                            ef.SaveChanges();
                            btn_refresh_Click(sender, e);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Ошибка!");
                    }
                }
            }
        }

        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as KppShipments;
            if (current != null)
            {
                MessageBoxResult? result =
                    MessageBox.Show("Сменить статус водителя " + current.FIO + " на Покинул склад?", "Внимание",
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                        {
                            var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                            if (shipment != null) 
                                shipment.IdTimeNavigation.DateTimeLeft = DateTime.Now;
                            ef.SaveChanges();
                            btn_refresh_Click(sender, e);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Ошибка!");
                    }
                }
            }
        }

        private void btn_adddriver_Click(object sender, RoutedEventArgs e)
        {
            ClearTextboxes();
            ContentAdd.ShowAsync();
        }

        void ClearTextboxes()
        {
            tb_autonum.Clear();
            tb_family.Clear();
            tb_info.Clear();
            tb_name.Clear();
            tb_passport.Clear();
            tb_patronymic.Clear();
            tb_phone.Clear();
        }

        private void btn_registration_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as PlanShipment;
            if (current !=null)
            {
                MessageBoxResult? result =
                    MessageBox.Show("Сменить статус водителя " + current.FIO + " на Зарегистрирован?", "Внимание",
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                        {
                            var shipment = ef.Shipments.FirstOrDefault(x => x.IdShipment == current.IdShipment);
                            if (shipment != null) 
                                shipment.IdTimeNavigation.DateTimeFactRegist = DateTime.Now;
                            ef.SaveChanges();
                            btn_refresh_Click(sender, e);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Ошибка!");
                    }
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
            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    DB.Shipment shipment = new DB.Shipment
                    {
                        IdDriverNavigation = new DB.Driver
                        {
                            Family = tb_family.Text,
                            Name = tb_name.Text,
                            Patronymic = tb_patronymic.Text,
                            Phone = tb_phone.Text,
                            AutoNumber = tb_autonum.Text,
                            Passport = tb_passport.Text,
                            Info = tb_info.Text,
                            ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил карточку водителя"
                        }, 
                        IdTimeNavigation = new DB.Time
                        {
                            DateTimeFactRegist = DateTime.Now
                        },
                        ServiceInfo = DateTime.Now + " " + App.ActiveUser + " каскадное добавление с карточкой водителя"
                    };

                    ef.Add(shipment);
                    ef.SaveChanges();
                    ContentAdd.Hide();
                    btn_refresh_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
    }
}
