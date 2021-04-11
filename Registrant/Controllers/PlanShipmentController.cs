using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<PlanShipment> GetPlanShipments(DateTime date)
        {
            PlanShipments.Clear();
            
            using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
            {
                var shipments = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date
                                                        && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                foreach (var item in shipments)
                {
                    PlanShipment shipment = new PlanShipment(item);
                    PlanShipments.Add(shipment);
                }
            }
            return PlanShipments;
        }
    }
}
