using System;
using System.Globalization;

namespace Registrant.Models
{
    public class PrintShipments : DB.Shipment
    {
        public string DatePlan { get; set; }
        public string FioTelephone { get; set; }

        public string Contragent { get; set; }
        public string PlanDate { get; set; }
        public string PlanTime { get; set; }
        public string DateFact { get; set; }
        public string DateLeft { get; set; }
        public string DateArrive { get; set; }
        public string DateLoad { get; set; }
        public string DateEndLoad { get; set; }

        public string Sklad { get; set; }
        public string Attorney { get; set; }
        public string Auto { get; set; }

        /// <summary>
        /// Для склада
        /// </summary>
        public string Family { get; set; }
        public string NumAuto { get; set; }
        public string TimeLoad { get; set; }
        public string TimeEnd { get; set; }
        public string TimeTotal { get; set; }

        public PrintShipments(DB.Shipment shipment)
        {
            IdShipment = shipment.IdShipment;
            if (shipment.IdDriverNavigation != null)
                FioTelephone = $"{shipment.IdDriverNavigation.Family} {shipment.IdDriverNavigation.Name} {shipment.IdDriverNavigation.Patronymic} {shipment.IdDriverNavigation.Phone}";

            DatePlan = shipment.IdTimeNavigation.DateTimePlanRegist.HasValue ? shipment.IdTimeNavigation.DateTimePlanRegist.Value.ToString(CultureInfo.CurrentCulture) : "";
            DateArrive = shipment.IdTimeNavigation.DateTimeArrive.HasValue ? shipment.IdTimeNavigation.DateTimeArrive.Value.ToString(CultureInfo.CurrentCulture) : "";
            DateLoad = shipment.IdTimeNavigation.DateTimeLoad.HasValue ? shipment.IdTimeNavigation.DateTimeLoad.Value.ToString(CultureInfo.CurrentCulture) : "";
            DateEndLoad = shipment.IdTimeNavigation.DateTimeEndLoad.HasValue ? shipment.IdTimeNavigation.DateTimeEndLoad.Value.ToString(CultureInfo.CurrentCulture) : "";

            Contragent = shipment.IdContragentNavigation?.Name;
            Attorney = shipment.IdDriverNavigation?.Attorney;
            Auto = shipment.IdDriverNavigation?.Auto + " " + shipment.IdDriverNavigation?.AutoNumber;

            PlanDate = DatePlan;
            //PlanTime = DatePlan.ToShortTimeString();

            DateFact = shipment.IdTimeNavigation.DateTimeFactRegist.ToString();
            DateLeft = shipment.IdTimeNavigation.DateTimeLeft.ToString();
            Sklad = "МВП";

            NumRealese = shipment.NumRealese;
            PacketDocuments = shipment.PacketDocuments;
            TochkaLoad = shipment.TochkaLoad;
            ServiceInfo = shipment.ServiceInfo;

            
            Destination = shipment.Destination;
            Nomenclature = shipment.Nomenclature;
            Size = shipment.Size;
            CountPodons = shipment.CountPodons;
            Family = shipment.IdDriverNavigation?.Family;
            NumAuto = shipment.IdDriverNavigation?.AutoNumber;

            TimeLoad = shipment.IdTimeNavigation.DateTimeLoad?.ToShortTimeString();
            TimeEnd = shipment.IdTimeNavigation.DateTimeEndLoad?.ToShortTimeString();

            if (shipment.IdTimeNavigation.DateTimeLoad != null && shipment.IdTimeNavigation.DateTimeEndLoad != null)
            {
                DateTime date1 = shipment.IdTimeNavigation.DateTimeLoad.Value;
                DateTime date2 = shipment.IdTimeNavigation.DateTimeEndLoad.Value;
                var res = date2 - date1;
                TimeTotal = res.ToString(@"hh\:mm");
            }

            StoreKeeper = shipment.StoreKeeper;
            TypeLoad = shipment.TypeLoad;
            Description = shipment.Description;
        }
    }
}
