using System;
using System.Collections.Generic;
using System.Linq;
using Registrant.Models;
using System.Windows;
using Registrant.DB;

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
        public List<KppShipments> GetShipments(DateTime date)
        {
            DriverShipments.Clear();

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                var shipments = ef.Shipments.Where(x =>
                    (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date ||
                     x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date)
                    && x.IdTimeNavigation.DateTimeLeft == null
                    && x.IdTimeNavigation.DateTimeFactRegist != null
                    && x.Active != "0")
                    .OrderBy(x => x.IdDriverNavigation.Family);

                foreach (var item in shipments)
                {
                    KppShipments shipment = new KppShipments(item);
                    DriverShipments.Add(shipment);
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
            return DriverShipments;
        }

        public List<Models.KPPShipments> GetShipments()
        {
            DriverShipments.Clear();

            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    var temp = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimeLeft == null && x.IdTimeNavigation.DateTimeFactRegist != null && x.Active != "0")).OrderBy(x => x.IdDriverNavigation.Family);
                    foreach (var item in temp)
                    {
                        Models.KPPShipments shipment = new Models.KPPShipments(item);
                        DriverShipments.Add(shipment);
                    }
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
