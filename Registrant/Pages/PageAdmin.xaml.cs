using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Registrant.DB;

namespace Registrant.Pages
{
    public partial class PageAdmin
    {
        public PageAdmin()
        {
            InitializeComponent();
            LoadUser();
        }

        void LoadUser()
        {
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                DataGrid_Users.ItemsSource = ef.Users.OrderBy(x => x.IdUser).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Программное исключене", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_deluser_Click(object sender, RoutedEventArgs e)
        {
            var bt = e.OriginalSource as Button;
            var current = bt?.DataContext as User;

            if (current != null)
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    ef.Remove(current);
                    ef.SaveChanges();
                    LoadUser();
                    ContentSave.ShowAsync();
                }
                catch (Exception ex)
                {
                    ((MainWindow)Application.Current.MainWindow).ContentErrorText.ShowAsync();
                    ((MainWindow)Application.Current.MainWindow).text_debuger.Text = ex.ToString();
                }
            }
        }

        private void btn_info_close_Click(object sender, RoutedEventArgs e)
        {
            ContentSave.Hide();
        }

        private void btn_add_add_Click(object sender, RoutedEventArgs e)
        {
            ContentAddUser.Hide();
            if (tb_login.Text != "")
            {
                try
                {
                    using RegistrantCoreContext ef = new RegistrantCoreContext();
                    User user = new User
                    {
                        Name = tb_name.Text,
                        Login = tb_login.Text,
                        Password = tb_pass.Text
                    };
                    user.LevelAccess = cb_access.SelectedIndex switch
                    {
                        0 => "kpp",
                        1 => "reader",
                        2 => "warehouse",
                        3 => "shipment",
                        4 => "admin",
                        _ => user.LevelAccess
                    };
                    ef.Add(user);
                    ef.SaveChanges();
                    LoadUser();
                }
                catch (Exception ex)
                {
                    ((MainWindow)Application.Current.MainWindow).ContentErrorText.ShowAsync();
                    ((MainWindow)Application.Current.MainWindow).text_debuger.Text = ex.ToString();
                }
                ContentSave.ShowAsync();
            }
        }

        private void btn_add_close_Click(object sender, RoutedEventArgs e)
        {
            ContentAddUser.Hide();
        }


        private void btn_showaddwindow_Click(object sender, RoutedEventArgs e)
        {
            tb_login.Text = "";
            tb_pass.Text = "";
            tb_name.Text = "";
            ContentAddUser.ShowAsync();
        }
    }
}
