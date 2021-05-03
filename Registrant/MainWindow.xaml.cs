using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Registrant.DB;
using Registrant.Pages;

namespace Registrant
{
    public partial class MainWindow 
    {
        //Чтобы потом обратится при нажатие на кнопку
        private PageContragents _pageContragents;
        private PageKPP _pageKpp;
        private PageDrivers _pageDrivers;
        private PageShipments _pageShipments;
        private PageUser _pageUser;
        private PageAdmin _pageAdmin;

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

        /// Проверка существует ли вообще подключение к серверу
        void TestConnect()
        {
            //Thread.Sleep(2000);
            Dispatcher.Invoke(() => ContentWait.ShowAsync());
            
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var engines = ef.Engines.ToList();
                Dispatcher.Invoke(() => ContentWait.Hide());
                Dispatcher.Invoke(() => ContentAuth.ShowAsync());
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
                    Dispatcher.Invoke(() => _pageAdmin = new PageAdmin());
                    Dispatcher.Invoke(() => nav_admin.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageContragents = new PageContragents());
                    Dispatcher.Invoke(() => nav_contragents.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageDrivers = new PageDrivers());
                    Dispatcher.Invoke(() => nav_drivers.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageKpp = new PageKPP());
                    Dispatcher.Invoke(() => nav_jurnalkpp.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageShipments = new PageShipments());
                    Dispatcher.Invoke(() => nav_jurnalshipment.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => nav_userset.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => FrameContent.Content = _pageShipments);
                    break;
                case "reader":
                    Dispatcher.Invoke(() => _pageDrivers = new PageDrivers());
                    Dispatcher.Invoke(() => nav_drivers.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageShipments = new PageShipments());
                    Dispatcher.Invoke(() => nav_jurnalshipment.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => nav_userset.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => FrameContent.Content = _pageShipments);
                    break;
                case "warehouse":
                    Dispatcher.Invoke(() => _pageDrivers = new PageDrivers());
                    Dispatcher.Invoke(() => nav_drivers.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageShipments = new PageShipments());
                    Dispatcher.Invoke(() => nav_jurnalshipment.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => nav_userset.Visibility = Visibility.Visible);

                    //Dispatcher.Invoke(() => pageContragents = new Pages.PageContragents());

                    Dispatcher.Invoke(() => FrameContent.Content = _pageShipments);
                    break;
                case "shipment":
                    Dispatcher.Invoke(() => _pageShipments = new PageShipments());
                    Dispatcher.Invoke(() => nav_jurnalshipment.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageContragents = new PageContragents());
                    Dispatcher.Invoke(() => nav_contragents.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageDrivers = new PageDrivers());
                    Dispatcher.Invoke(() => nav_drivers.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => _pageKpp = new PageKPP());
                    Dispatcher.Invoke(() => nav_jurnalkpp.Visibility = Visibility.Visible);

                    Dispatcher.Invoke(() => nav_userset.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(() => FrameContent.Content = _pageShipments);
                    break;
                case "kpp":
                    Dispatcher.Invoke(() => nav_jurnalkpp.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(() => _pageKpp = new PageKPP());

                    Dispatcher.Invoke(() => nav_userset.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(() => FrameContent.Content = _pageKpp);
                    break;
                default:
                    MessageBox.Show("К такому меня жизнь не готовила, авторизация прошла, " +
                                    "а вот не смогли удостоверится кем вы являетесь по роли. Обратитесь к сис админму");
                    break;
            }
        }

        void CheckForUpdates()
        {
            Thread.Sleep(1500);
            Dispatcher.Invoke(() => ContentCheckingUpdate.ShowAsync());
            WebClient web = new WebClient();
            try
            {
                string actualVersionText = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualVer.txt");
                actualVersionText = actualVersionText.Replace("\n", "").Replace(".", ",");
                string actualDescription = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualTextDesc.txt");

                string appVersion = Settings.App.Default.AppVersion.Replace(".", ",");
                decimal currentVersion = decimal.Parse(appVersion);
                decimal actualVersion = decimal.Parse(actualVersionText);
                Dispatcher.Invoke(() => ContentCheckingUpdate.Hide());

                if (actualVersion > currentVersion)
                {
                    Dispatcher.Invoke(() => ContentUpdate.ShowAsync());
                    Dispatcher.Invoke(() => txt_currver.Text = currentVersion.ToString(CultureInfo.CurrentCulture));
                    Dispatcher.Invoke(() => txt_newver.Text = actualVersionText);
                    Dispatcher.Invoke(() => txt_desc.Text = actualDescription);

                    string canRefuse = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualVerCanRefuse.txt");
                    canRefuse = canRefuse.Replace("\n", "");

                    if (canRefuse == "No")
                    {
                        Dispatcher.Invoke(() => btn_updatelate.Visibility = Visibility.Hidden);
                        Dispatcher.Invoke(() => txt_desc.Text = txt_desc.Text + "\n\nЭто обновление нельзя отложить, т.к. содержит\nкритические правки в коде");
                        Dispatcher.Invoke(() => ContentUpdate.Background = new SolidColorBrush(Color.FromRgb(255, 195, 195)));
                    }
                }
                else
                {
                    TestConnect();
                }
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() => ContentCheckingUpdate.Hide());
                TestConnect();
            }
        }

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
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var user = ef.Users.FirstOrDefault(x => tb_login.Text == x.Login && tb_password.Password == x.Password);

                if (user != null)
                {
                    ContentAuth.Hide();
                    App.SetActiveUser(user.Name);
                    App.SetLevelAccess(user.LevelAccess);
                    NavUI.PaneTitle = "РЕГИСТРАНТ (" + user.Name + ")";
                    nav_userset.Content = user.Name;
                    _pageUser = new PageUser(user.IdUser);

                    Thread thread = new Thread(Verify);
                    thread.Start();
                }
                else
                {
                    MessageBox.Show("Логин и/или пароль неверный", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ContentWait.Hide());
                Dispatcher.Invoke(() => ContentError.ShowAsync());
                Dispatcher.Invoke(() => text_error.Text = ex.ToString());
            }
        }

        private void nav_jurnalkpp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageKpp;
        }

        private void nav_contragents_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageContragents;
        }

        private void nav_drivers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageDrivers;
        }

        private void nav_jurnalshipment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageShipments;
        }

        private void nav_admin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageAdmin;
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void nav_userset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameContent.Content = _pageUser;
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
