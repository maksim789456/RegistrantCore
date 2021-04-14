using System;
using System.Collections.Generic;
using System.Linq;
using Registrant.Models;
using System.Text;
using System.Windows;

namespace Registrant.Controllers
{
    public class KppShipmentsController
    {
        List<KppShipments> DriverShipments { get; set; }

        public KppShipmentsController()
        {
            DriverShipments = new List<KppShipments>();
        }

        //Получение списка отгрузок для КПП
        public List<Models.KPPShipments> GetShipments(DateTime date)
        {
            DriverShipments.Clear();

            date = date.Date;
            try
            {
                //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                var shipments = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                   && x.IdTimeNavigation.DateTimeLeft == null 
                                                   && x.IdTimeNavigation.DateTimeFactRegist != null 
                                                   && x.Active != "0");
                
                foreach (var item in shipments)
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    KppShipments shipment = new KppShipments(item);
                    DriverShipments.Add(shipment);
                }
            }
            catch (Exception ex)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).ContentErrorText.ShowAsync();
                ((MainWindow)System.Windows.Application.Current.MainWindow).text_debuger.Text = ex.ToString();
            }
            return DriverShipments;
        }
    }
}
