﻿using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;
using Registrant.Models;

namespace Registrant.Pages
{
    public partial class PageDrivers
    {
        private Controllers.DriversController _controller;

        public PageDrivers()
        {
            InitializeComponent();
            _controller = new Controllers.DriversController();
            DataGrid_Drivers.ItemsSource = _controller.GetDrivers();

            if (App.LevelAccess == "reader")
            {
                btn_add_driver.Visibility = Visibility.Collapsed;
                btn_delete_30day.Visibility = Visibility.Collapsed;
            }

            Thread thread = new Thread(RefreshThread);
            thread.Start();
        }

        void RefreshThread()
        {
            while (true)
            {
                Thread.Sleep(Settings.App.Default.RefreshContent);
                if (Dispatcher.Invoke(() => tb_search.Text == ""))
                {
                    Dispatcher.Invoke(() => DataGrid_Drivers.ItemsSource = _controller.GetDrivers());
                    Dispatcher.Invoke(() => DataGrid_Drivers.Items.Refresh());
                }
            }
        }

        /// <summary>
        /// Кнопка обновить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (tb_search.Text == "")
            {
                //DataGrid_Drivers.ItemsSource = null;
                DataGrid_Drivers.ItemsSource = _controller.GetDrivers();
                DataGrid_Drivers.Items.Refresh();
            }
        }

        /// Кнопка закрытия диалогового окна с редактированием
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAddEdit.Hide();
            btn_edit.Visibility = Visibility.Collapsed;
            btn_add.Visibility = Visibility.Collapsed;
        }

        /// Кнопка добавления водителя
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (tb_Family.Text == "")
            {
                MessageBox.Show("Введите хотя бы фамилию водителя!", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                Driver driver = new Driver
                {
                    Family = tb_Family.Text,
                    Name = tb_name.Text,
                    Patronymic = tb_patronomyc.Text,
                    Phone = tb_phone.Text,
                    Attorney = tb_attorney.Text,
                    Auto = tb_auto.Text,
                    AutoNumber = tb_autonum.Text,
                    Passport = tb_passport.Text,
                    Info = tb_info.Text,
                    Active = "1",
                    ServiceInfo = DateTime.Now + " " + App.ActiveUser + " добавил водителя"
                };

                ef.Add(driver);
                ef.SaveChanges();
                btn_close_Click(sender, e);
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

        /// Кнопка открыть окно редактирования
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Models.Drivers;
            ClearTextbox();

            btn_edit.Visibility = Visibility.Visible;
            btn_add.Visibility = Visibility.Collapsed;
            btn_delete.Visibility = Visibility.Visible;
            if (current == null) return;
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var driver = ef.Drivers.FirstOrDefault(x => x.IdDriver == current.IdDriver);

                if (driver != null)
                {
                    text_namedriver.Text = driver.Family + " " + driver.Name + " " + driver.Patronymic;

                    tb_id.Text = driver.IdDriver.ToString();
                    tb_Family.Text = driver.Family;
                    tb_name.Text = driver.Name;
                    tb_patronomyc.Text = driver.Patronymic;
                    tb_phone.Text = driver.Phone;

                    tb_attorney.Text = driver.Attorney;
                    tb_auto.Text = driver.Auto;
                    tb_autonum.Text = driver.AutoNumber;
                    tb_passport.Text = driver.Passport;
                    tb_info.Text = driver.Info;
                }
                ContentAddEdit.ShowAsync();
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


        /// Кнопка удалить
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var driver = ef.Drivers.FirstOrDefault(x => x.IdDriver == Convert.ToInt32(tb_id.Text));
                if (driver != null)
                {
                    driver.Active = "0";
                    driver.ServiceInfo = driver.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser + " удалил водителя";
                }

                ef.SaveChanges();
                ContentAddEdit.Hide();
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

        /// Кнопка добавить водителя
        private void btn_add_driver_Click(object sender, RoutedEventArgs e)
        {
            ContentAddEdit.ShowAsync();
            ClearTextbox();
            text_namedriver.Text = "Добавить нового водителя";

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                btn_add.Visibility = Visibility.Visible;
                btn_delete.Visibility = Visibility.Collapsed;
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

        /// Очистка
        void ClearTextbox()
        {
            tb_id.Text = "";
            tb_Family.Text = "";
            tb_name.Text = "";
            tb_patronomyc.Text = "";
            tb_phone.Text = "";
            tb_attorney.Text = "";
            tb_auto.Text = "";
            tb_autonum.Text = "";
            tb_passport.Text = "";
            tb_info.Text = "";
        }

        /// Непосредственное редактирование
        private void btn_edit_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var driver = ef.Drivers.FirstOrDefault(x => x.IdDriver == Convert.ToInt32(tb_id.Text));
                if (driver != null)
                {
                    driver.Family = tb_Family.Text;
                    driver.Name = tb_name.Text;
                    driver.Patronymic = tb_patronomyc.Text;
                    driver.Phone = tb_phone.Text;

                    driver.Attorney = tb_attorney.Text;
                    driver.Auto = tb_auto.Text;
                    driver.AutoNumber = tb_autonum.Text;
                    driver.Passport = tb_passport.Text;
                    driver.Info = tb_info.Text;
                    driver.ServiceInfo = driver.ServiceInfo + "\n" + DateTime.Now + " " + App.ActiveUser + " внес изменения";
                    ef.SaveChanges();
                    btn_close_Click(sender, e);
                    ContentAddEdit.Hide();
                }

                ef.SaveChanges();
                btn_close_Click(sender, e);
                ContentAddEdit.Hide();
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

        /// Закрытия окна с информацией
        private void btn_info_close_Click(object sender, RoutedEventArgs e)
        {
            ContentInfo.Hide();
        }

        /// <summary>
        /// Открыть окно с расширенной информацией
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_info_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Drivers;

            if (current != null)
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    ContentInfo.ShowAsync();
                    ContentInfoGrid.DataContext = ef.Drivers.FirstOrDefault(x => x.IdDriver == current.IdDriver);
                    text_info_namedriver.Text = current.Fio;
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
                    DataGrid_Drivers.ItemsSource = null;

                    var allDrivers = _controller.GetDriversAll();

                    var data = allDrivers.Where(t => t.Fio.ToUpper().StartsWith(tb_search.Text.ToUpper())).ToList();
                    var sDop = allDrivers.Where(t => t.Fio.ToUpper().Contains(tb_search.Text.ToUpper())).ToList();
                    data.AddRange(sDop);
                    var noDupes = data.Distinct().ToList();
                    DataGrid_Drivers.ItemsSource = noDupes;

                    //TODO: WTF???
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

        private void btn_delete_30day_Click(object sender, RoutedEventArgs e)
        {
            Content30days.ShowAsync();
        }

        private void btn_30d_close_Click(object sender, RoutedEventArgs e)
        {
            Content30days.Hide();
        }

        private void btn_30d_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                DateTime last30 = DateTime.Now.Date.AddDays(-30);
                DateTime currentMonth = DateTime.Now.Date;
                //var temp = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimeLeft > last30) && x.IdDriverNavigation.Active != "0" && x.IdTimeNavigation.DateTimeFactRegist.Value.Month == currentMonth.Month);
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
}
