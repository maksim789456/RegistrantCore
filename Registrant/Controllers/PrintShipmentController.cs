using System;
using System.Collections.Generic;
using System.Linq;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class PrintShipmentController
    {
        List<PrintShipments> PlanShipments { get; set; }

        public PrintShipmentController()
        {
            PlanShipments = new List<PrintShipments>();
        }

        public List<PrintShipments> GetShipmentsDate(DateTime date)
        {
            PlanShipments.Clear();

            using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
            {
                var shipments = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date);
                foreach (var item in shipments)
                {
                    PrintShipments shipment = new PrintShipments(item);
                    PlanShipments.Add(shipment);
                }
            }
            return PlanShipments;
        }

       public List<PrintShipments> GetShipmentsMonth(DateTime date)
        {
            PlanShipments.Clear();

            using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
            {
                var shipments = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Month == date.Month);
                foreach (var item in shipments)
                {
                    PrintShipments shipment = new PrintShipments(item);
                    PlanShipments.Add(shipment);
                }
            }
            return PlanShipments;
        }
    }
}
