using System;
using System.Globalization;
using System.Windows;

namespace Registrant.Models
{
    public class KppShipments
    {
        public int IdShipment { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public DateTime PlanDateFact { get; set; }
        public string PlanDateFactString { get; set; }
        public string TextStatus { get; set; }

        public string NumAuto { get; set; }
        public string btn_left { get; set; }
        public string btn_arrive { get; set; }

        public KppShipments(DB.Shipment shipment)
        {
            IdShipment = shipment.IdShipment;
            Fio = $"{shipment.IdDriverNavigation?.Family} {shipment.IdDriverNavigation?.Name} {shipment.IdDriverNavigation?.Patronymic}";
            Phone = shipment.IdDriverNavigation?.Phone;
            if (shipment.IdTimeNavigation.DateTimeFactRegist.HasValue)
            {
                PlanDateFact = shipment.IdTimeNavigation.DateTimeFactRegist.Value;
                PlanDateFactString = PlanDateFact.ToString(CultureInfo.GetCultureInfo("ru-ru"));
            }

            if (shipment.IdDriverNavigation.AutoNumber != null)
            {
                NumAuto = shipment.IdDriverNavigation.AutoNumber;
            }

            if (shipment.IdTimeNavigation?.DateTimeLeft != null)
            {
                TextStatus = "Покинул склад";
                ArriveButtonVisibility = Visibility.Collapsed;
                LeftButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimeLeft == null && shipment.IdTimeNavigation?.DateTimeEndLoad != null)
            {
                TextStatus = "Отгрузка завершена";
                LeftButtonVisibility = Visibility.Visible;
                ArriveButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimeEndLoad == null && shipment.IdTimeNavigation?.DateTimeLoad != null)
            {
                TextStatus = "Отгрузка";
                ArriveButtonVisibility = Visibility.Collapsed;
                LeftButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimeLoad == null && shipment.IdTimeNavigation?.DateTimeArrive != null)
            {
                TextStatus = "На территории склада";
                LeftButtonVisibility = Visibility.Visible;
                ArriveButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimeArrive == null && shipment.IdTimeNavigation?.DateTimeFactRegist != null)
            {
                TextStatus = "Зарегистрирован";
                ArriveButtonVisibility = Visibility.Visible;
                LeftButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimeFactRegist == null && shipment.IdTimeNavigation?.DateTimePlanRegist != null)
            {
                TextStatus = "";
                ArriveButtonVisibility = Visibility.Collapsed;
                LeftButtonVisibility = Visibility.Collapsed;
            }
            else if (shipment.IdTimeNavigation?.DateTimePlanRegist == null && shipment.IdTimeNavigation?.DateTimeFactRegist != null)
            {
                TextStatus = "Зарегистрирован";
                ArriveButtonVisibility = Visibility.Visible;
                LeftButtonVisibility = Visibility.Collapsed;
            } 
        }
    }
}
