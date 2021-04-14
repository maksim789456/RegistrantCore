﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = ModernWpf.MessageBox;

namespace Registrant
{
    public partial class MainWindow 
    {
        //Чтобы потом обратится при нажатие на кнопку
        Pages.PageContragents pageContragents;
        Pages.PageKPP pageKPP;
        Pages.PageDrivers pageDrivers;
        Pages.PageShipments pageShipments;
        Pages.PageUser pageUser;
        Pages.PageAdmin pageAdmin;

        public MainWindow()
        {
            InitializeComponent();

            //Подгрузка данных из настроек
            tb_login.Text = Settings.User.Default.login;
            text_verson.Text = Settings.App.Default.AppVersion;

            //Поток наа 1 старт чтобы при старте не тормозилось
            Thread thread = new Thread(CheckForUpdates);
            thread.Start();
        }

        /// <summary>
        /// Открытие дебага
        private void btn_debug_Click(object sender, RoutedEventArgs e)
        {
            text_error.Visibility = Visibility.Visible;
        }

        
        //Кнопка повторить попытку
        private void btn_tryconnect_Click(object sender, RoutedEventArgs e)
        {
            ContentError.Hide();
            Thread thread1 = new Thread(TestConnect);
            thread1.Start();
        }

        /// <summary>
        /// Проверка существует ли вообще подключение к серверу
        void TestConnect()
        {
            Thread.Sleep(2000);
            Dispatcher.Invoke(() => ContentWait.ShowAsync());

            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    var engines = ef.Engines.ToList();
                    Dispatcher.Invoke(() => ContentWait.Hide());
                    Dispatcher.Invoke(() => ContentAuth.ShowAsync());
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ContentWait.Hide());
                Dispatcher.Invoke(() => ContentError.ShowAsync());
                Dispatcher.Invoke(() => text_error.Text = ex.ToString());
            }
        }

        /// Кнопка с редактированием настроек подключения
        private void btn_opensettings_Click(object sender, RoutedEventArgs e)
        {
            Forms.EditConnect edit = new Forms.EditConnect();
            edit.ShowDialog();
        }

        /// Действие на нажатие на кнопку Войти
        private void btn_enter_Click(object sender, RoutedEventArgs e)
        {
            Settings.User.Default.login = tb_login.Text;
            Settings.User.Default.Save();

            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    var user = ef.Users.FirstOrDefault(x => tb_login.Text == x.Login && tb_password.Password == x.Password);

                    if (user != null)
                    {
                        ContentAuth.Hide();
                        App.SetActiveUser(user.Name);
                        App.SetLevelAccess(user.LevelAccess);
                        NavUI.PaneTitle = "РЕГИСТРАНТ (" + user.Name + ")";
                        nav_userset.Content = user.Name;
                        pageUser = new Pages.PageUser(user.IdUser);
                        Verify();
                    }

                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ContentWait.Hide());
                Dispatcher.Invoke(() => ContentError.ShowAsync());
                Dispatcher.Invoke(() => text_error.Text = ex.ToString());
            }
        }

        /// Проверяем кто он по масти
        void Verify()
        {
            switch (App.LevelAccess)
            {
                case "admin":
                    nav_admin.Visibility = Visibility.Visible;
                    nav_contragents.Visibility = Visibility.Visible;
                    nav_drivers.Visibility = Visibility.Visible;
                    nav_jurnalkpp.Visibility = Visibility.Visible;
                    nav_jurnalshipment.Visibility = Visibility.Visible;
                    nav_userset.Visibility = Visibility.Visible;

                    //Иниципализация нужных страниц под ролей
                    pageKPP = new Pages.PageKPP();
                    pageContragents = new Pages.PageContragents();
                    pageDrivers = new Pages.PageDrivers();
                    pageShipments = new Pages.PageShipments();
                    pageAdmin = new Pages.PageAdmin();

                    FrameContent.Content = pageShipments;
                    break;
                case "reader":
                    nav_drivers.Visibility = Visibility.Visible;
                    nav_jurnalshipment.Visibility = Visibility.Visible;
                    nav_userset.Visibility = Visibility.Visible;

                    pageDrivers = new Pages.PageDrivers();
                    pageShipments = new Pages.PageShipments();

                    FrameContent.Content = pageShipments;
                    break;
                case "warehouse":
                    nav_drivers.Visibility = Visibility.Visible;
                    nav_jurnalshipment.Visibility = Visibility.Visible;
                    nav_userset.Visibility = Visibility.Visible;

                    pageContragents = new Pages.PageContragents();
                    pageDrivers = new Pages.PageDrivers();
                    pageShipments = new Pages.PageShipments();

                    FrameContent.Content = pageShipments;
                    break;
                case "shipment":
                    nav_jurnalshipment.Visibility = Visibility.Visible;
                    nav_contragents.Visibility = Visibility.Visible;
                    nav_drivers.Visibility = Visibility.Visible;
                    nav_userset.Visibility = Visibility.Visible;

                    pageContragents = new Pages.PageContragents();
                    pageDrivers = new Pages.PageDrivers();
                    pageShipments = new Pages.PageShipments();

                    FrameContent.Content = pageShipments;
                    break;
                case "kpp":
                    nav_jurnalkpp.Visibility = Visibility.Visible;
                    nav_userset.Visibility = Visibility.Visible;
                    pageKPP = new Pages.PageKPP();

                    FrameContent.Content = pageKPP;
                    break;
            }
        }

        void CheckForUpdates()
        {
            WebClient web = new WebClient();
            try
            {
                string act = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualVer.txt");
                string actualText = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualTextDesc.txt");
                act = act.Replace("\n", "");
                act = act.Replace(".", ",");

                string currentVersion = Settings.App.Default.AppVersion;
                currentVersion = currentVersion.Replace(".", ",");
                decimal current = decimal.Parse(currentVersion);
                decimal actual = decimal.Parse(act);

                if (actual > current)
                {
                    Dispatcher.Invoke(() => ContentUpdate.ShowAsync());
                    Dispatcher.Invoke(() => txt_currver.Text = current.ToString(CultureInfo.CurrentCulture));
                    Dispatcher.Invoke(() => txt_newver.Text = act.ToString());
                    Dispatcher.Invoke(() => txt_desc.Text = actualText);

                    string canRefuse = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualVerCanRefuse.txt");
                    canRefuse = canRefuse.Replace("\n", "");

                    if (canRefuse == "No")
                    {
                        Dispatcher.Invoke(() => btn_updatelate.Visibility = Visibility.Hidden);
                        Dispatcher.Invoke(() => txt_desc.Text = txt_desc.Text + "\n\nЭто обновление нельзя отложить, т.к. содержит\nкритические правки в коде");
                        Dispatcher.Invoke(() => ContentUpdate.Background = new SolidColorBrush(Color.FromRgb(255, 140, 140)));
                    }
                }
                else
                {
                    TestConnect();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(() => txt_desc.Text = "");
                TestConnect();
            }
        }

        private void nav_jurnalkpp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageKPP;
        }

        private void nav_contragents_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageContragents;
        }

        private void nav_drivers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageDrivers;
        }

        private void nav_jurnalshipment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageShipments;
        }

        private void nav_admin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageAdmin;
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void nav_userset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = pageUser;
        }

        private void btn_about_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAppVer.Hide();
        }

        private void nav_aboutpo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentAppVer.ShowAsync();
        }

        private void btn_updatenow_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("CoreUpdater.exe");
            Environment.Exit(0);
        }

        private void btn_updatelate_Click(object sender, RoutedEventArgs e)
        {
            ContentUpdate.Hide();
            Thread thread = new Thread(TestConnect);
            thread.Start();
        }

        private void btn_debugger_close_Click(object sender, RoutedEventArgs e)
        {
            ContentErrorText.Hide();
        }
    }
}
