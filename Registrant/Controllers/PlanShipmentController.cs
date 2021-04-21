using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class PlanShipmentController
    {
        List<PlanShipment> PlanShipments { get; set; }

        public PlanShipmentController()
        {
            PlanShipments = new List<PlanShipment>();
        }

        //Для кпп по датам
        public List<PlanShipment> GetPlanShipments(DateTime date)
        {
            PlanShipments.Clear();
            
            date = date.Date;
            try
            {
                using DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext();
                var shipments = ef.Shipments.Where(x =>
                    x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date
                    && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                foreach (var item in shipments)
                {
                    var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date && x.IdTimeNavigation.DateTimeFactRegist.Value == null).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
                    PlanShipment shipment = new PlanShipment(item);
                    PlanShipments.Add(shipment);
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
            return PlanShipments;
        }
    }
}
