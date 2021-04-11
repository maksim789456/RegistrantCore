using System;
using System.Windows;
using Registrant.DB;

namespace Registrant.Forms
{
    public partial class KPPAddNew
    {
        public KPPAddNew()
        {
            InitializeComponent();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            using (RegistrantCoreContext ef = new RegistrantCoreContext())
            {
                Shipment shipment = new Shipment
                {
                    IdDriverNavigation = new Driver
                    {
                        Family = tb_family.Text,
                        Name = tb_name.Text,
                        Patronymic = tb_patronymic.Text,
                        Phone = tb_phone.Text,
                        AutoNumber = tb_autonum.Text,
                        Passport = tb_passport.Text,
                        Info = tb_info.Text,
                        ServiceInfo = $"{DateTime.Now} {App.ActiveUser} добавил карточку водителя"
                    }, 
                    IdTimeNavigation = new Time
                    {
                        DateTimeFactRegist = DateTime.Now
                    },
                    ServiceInfo = $"{DateTime.Now} {App.ActiveUser} каскадное добавление с карточкой водителя"
                };

                ef.Add(shipment);
                ef.SaveChanges();
                Close();
            }
        }
    }
}
