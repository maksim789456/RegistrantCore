using System;
using System.Collections.Generic;
using System.Linq;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class KppShipmentsController
    {
        List<KppShipments> DriverShipments { get; set; }

        public KppShipmentsController()
        {
            DriverShipments = new List<KppShipments>();
        }

        public List<KppShipments> GetShipments(DateTime date)
        {
            DriverShipments.Clear();

            using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
            {
                //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date && x.IdTimeNavigation.DateTimeFactRegist.Value == null);
                var shipments = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                   && x.IdTimeNavigation.DateTimeLeft == null 
                                                   && x.IdTimeNavigation.DateTimeFactRegist != null 
                                                   && x.Active != "0");
                
                foreach (var item in shipments)
                {
                    KppShipments shipment = new KppShipments(item);
                    DriverShipments.Add(shipment);
                }
            }
            return DriverShipments;
        }
    }
}
