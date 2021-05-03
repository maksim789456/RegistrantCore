using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;

namespace Registrant.Pages
{
    public partial class PageContragents
    {
        public PageContragents()
        {
            InitializeComponent();
            FirstLoad();

            Thread thread = new Thread(RefreshThread);
            thread.Start();
        }

        /// <summary>
        /// Обновление в потоке
        /// </summary>
        void RefreshThread()
        {
            while (true)
            {
                Thread.Sleep(Settings.App.Default.RefreshContent);
                try
                {
                        var temp = ef.Contragents.Where(x => x.Active != "0").OrderBy(x => x.Name).ToList();
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    var contragents = ef.Contragents.Where(x => x.Active != "0")
                        .OrderByDescending(x => x.IdContragent).ToList();
                    Dispatcher.Invoke(() => DataGrid_Contragents.ItemsSource = contragents);
                    Dispatcher.Invoke(() => DataGrid_Contragents.Items.Refresh());
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.ContentErrorText.ShowAsync();
                            mainWindow.text_debuger.Text = ex.ToString();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Первый старт, подгрузка данных
        /// </summary>
        void FirstLoad()
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var contragents = ef.Contragents
                    .Where(x => x.Active != "0")
                    .OrderByDescending(x => x.IdContragent).ToList();
                DataGrid_Contragents.ItemsSource = contragents;
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

        /// <summary>
        /// Кнопка закрыть из диалг окна добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAdd.Hide();
        }

        /// <summary>
        /// Кнопка редактиировать из таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Contragent;

            if (current != null)
            {
                try 
                {
                    text_editnamecontragent.Text = $"Редактирование элемента {current.Name}";
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    var contragent = ef.Contragents.FirstOrDefault(x => x.IdContragent == current.IdContragent);
                    if (contragent != null)
                    {
                        tb_idcontragent.Text = contragent.IdContragent.ToString();
                        tb_edit_name.Text = contragent.Name;
                    }
                    ContentEdit.ShowAsync();
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

        //Кнопка удалить
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (tb_idcontragent != null)
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    var temp = ef.Contragents.FirstOrDefault(x => x.IdContragent == Convert.ToInt32(tb_idcontragent.Text));
                    if (temp != null) 
                        temp.Active = "0";
                    ef.SaveChanges();
                    ContentEdit.Hide();
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
            //Кнопа перенесена в другоме место,
            /*var bt = e.OriginalSource as Button;
            var current = bt.DataContext as DB.Contragent;

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var contragent = ef.Contragents.FirstOrDefault(x => x.IdContragent == current.IdContragent);
                if (contragent != null)
                {
                    contragent.Active = "0";
                    contragent.ServiceInfo = $"{contragent.ServiceInfo}\n{DateTime.Now} {App.ActiveUser} изменил удалил";
                }
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
            }*/
        }

        //Обновить
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var contragents = ef.Contragents
                    .Where(x => x.Active != "0")
                    .OrderByDescending(x => x.IdContragent).ToList();
                DataGrid_Contragents.ItemsSource = null;
                DataGrid_Contragents.ItemsSource = contragents;
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

        //Добавить контрагента, диалог окно
        private void btn_addcontragent_Click(object sender, RoutedEventArgs e)
        {
            ContentAdd.ShowAsync();
            tb_namecontragent.Text = "";
        }

        /// <summary>
        /// Кнопка добавление, реального добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                Contragent contragent = new Contragent
                {
                    Name = tb_namecontragent.Text,
                    ServiceInfo = $"{DateTime.Now} {App.ActiveUser} добавил контрагента",
                    Active = "1"
                };
                ef.Add(contragent);
                ef.SaveChanges();
                ContentAdd.Hide();
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

        /// <summary>
        /// Кнопка закрытия в добавление окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add_close_Click_1(object sender, RoutedEventArgs e)
        {
            ContentAdd.Hide();
        }

        /// <summary>
        /// Кнопка сохранить изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var contragent = ef.Contragents.FirstOrDefault(x => x.IdContragent == Convert.ToInt32(tb_idcontragent.Text));
                if (contragent != null)
                {
                    contragent.Name = tb_edit_name.Text;
                    contragent.ServiceInfo =
                        $"{contragent.ServiceInfo}\n {DateTime.Now} {App.ActiveUser} изменил название контрагента";
                }
                ef.SaveChanges();
                btn_refresh_Click(sender, e);
                ContentEdit.Hide();
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

        //Кнопка закрыть окна редактирования
        private void btn_edit_close_Click(object sender, RoutedEventArgs e)
        {
            ContentEdit.Hide();
        }

        //Закрыть инф окно
        private void btn_info_close_Click(object sender, RoutedEventArgs e)
        {
            ContentInfo.Hide();
        }

        /// <summary>
        /// Показать инфу о контр агенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_info_Click(object sender, RoutedEventArgs e)
        {
            ContentInfo.ShowAsync();
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as Contragent;

            if (current != null)
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    var contragent = ef.Contragents.FirstOrDefault(x => x.IdContragent == current.IdContragent);
                    if (contragent != null)
                    {
                        text_namecontragent.Text = contragent.Name;
                        text_infocontragent.Text = contragent.ServiceInfo;
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
    }
}
