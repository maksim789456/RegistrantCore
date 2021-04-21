using System;
using System.Collections.Generic;
using System.Linq;
using Registrant.Models;
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
        public List<KppShipments> GetShipments(DateTime date)
        {
            DriverShipments.Clear();

            try
            {
                using DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext();
                //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                var shipments = ef.Shipments.Where(x =>
                    (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date ||
                     x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date)
                    && x.IdTimeNavigation.DateTimeLeft == null
                    && x.IdTimeNavigation.DateTimeFactRegist != null
                    && x.Active != "0");

                foreach (var item in shipments)
                {
                    var temp = ef.Shipments.Where(x => ((x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date) && x.IdTimeNavigation.DateTimeLeft == null && x.IdTimeNavigation.DateTimeFactRegist != null && x.Active != "0")).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
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
    }
}
