﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Registrant.Models
{
    public class Shipments : DB.Shipment
    {
        //Водитель
        public string FIO { get; set; }
        public string Family { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Contragent { get; set; }
        public DateTime DateTimePlanRegist { get; set; }
        public DateTime DateTimeFactRegist { get; set; }
        public DateTime DateTimeArrive { get; set; }
        public DateTime DateTimeLoad { get; set; }
        public DateTime DateTimeEndLoad { get; set; }
        public DateTime DateTimeLeft { get; set; }

        public string TextStatus { get; set; }

        public string btn_load { get; set; }
        public string btn_endload { get; set; }

        public Shipments(DB.Shipment shipment)
        {
            IdShipment = shipment.IdShipment;
            IdDriver = shipment.IdDriver;
            NumRealese = shipment.NumRealese;
            CountPodons = shipment.CountPodons;
            PacketDocuments = shipment.PacketDocuments;
            TochkaLoad = shipment.TochkaLoad;
            Nomenclature = shipment.Nomenclature;
            Size = shipment.Size;
            Destination = shipment.Destination;
            TypeLoad = shipment.TypeLoad;
            Description = shipment.Description;
            StoreKeeper = shipment.StoreKeeper;
            ServiceInfo = shipment.ServiceInfo;
            Contragent = shipment.IdDriverNavigation?.IdContragentNavigation?.Name;

            Family = shipment.IdDriverNavigation?.Family;
            Name = shipment.IdDriverNavigation?.Name;
            Patronymic = shipment.IdDriverNavigation?.Patronymic;
            FIO = $"{Family} {Name} {Patronymic}";
            Phone = shipment.IdDriverNavigation?.Phone;

            if (shipment.IdTimeNavigation.DateTimePlanRegist.HasValue)
            {
                DateTimePlanRegist = shipment.IdTimeNavigation.DateTimePlanRegist.Value;
            }

            if (shipment.IdTimeNavigation.DateTimeFactRegist.HasValue)
            {
                DateTimeFactRegist = shipment.IdTimeNavigation.DateTimeFactRegist.Value;
            }

            if (shipment.IdTimeNavigation.DateTimeArrive.HasValue)
            {
                DateTimeArrive = shipment.IdTimeNavigation.DateTimeArrive.Value;
            }

            if (shipment.IdTimeNavigation.DateTimeArrive.HasValue)
            {
                DateTimeArrive = shipment.IdTimeNavigation.DateTimeArrive.Value;
            }
            if (shipment.IdTimeNavigation.DateTimeLoad.HasValue)
            {
                DateTimeLoad = shipment.IdTimeNavigation.DateTimeLoad.Value;
            }
            if (shipment.IdTimeNavigation.DateTimeEndLoad.HasValue)
            {
                DateTimeEndLoad = shipment.IdTimeNavigation.DateTimeEndLoad.Value;
            }
            if (shipment.IdTimeNavigation.DateTimeLeft.HasValue)
            {
                DateTimeLeft = shipment.IdTimeNavigation.DateTimeLeft.Value;
            }

            if (shipment.IdTimeNavigation.DateTimeLeft != null)
            {
                TextStatus = $"Покинул склад ({DateTimeLeft})";
                if (App.LevelAccess == "admin" || App.LevelAccess == "warehouse")
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
                else
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
            }
            else if (shipment.IdTimeNavigation?.DateTimeLeft == null && shipment.IdTimeNavigation?.DateTimeEndLoad != null)
            {
                TextStatus = $"Отгрузка завершена ({DateTimeEndLoad})";
                if (App.LevelAccess == "admin" || App.LevelAccess == "warehouse")
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
                else
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
            }
            else if (shipment.IdTimeNavigation?.DateTimeEndLoad == null && shipment.IdTimeNavigation?.DateTimeLoad != null)
            {
                TextStatus = $"Отгрузка ({DateTimeLoad})";
                if (App.LevelAccess == "admin" || App.LevelAccess == "warehouse")
                {
                    btn_endload = "Visible";
                    btn_load = "Collapsed";
                }
                else
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
            }
            else if (shipment.IdTimeNavigation?.DateTimeLoad == null && shipment.IdTimeNavigation?.DateTimeArrive != null)
            {
                TextStatus = $"На территории склада ({DateTimeArrive})";
                if (App.LevelAccess == "admin" || App.LevelAccess == "warehouse")
                {
                    btn_endload = "Collapsed";
                    btn_load = "Visible";
                }
                else
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
            }
            else if (shipment.IdTimeNavigation?.DateTimeArrive == null && shipment.IdTimeNavigation?.DateTimeFactRegist != null)
            {
                TextStatus = $"Зарегистрирован ({DateTimeFactRegist})";
                if (App.LevelAccess == "admin" || App.LevelAccess == "warehouse")
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }
                else
                {
                    btn_endload = "Collapsed";
                    btn_load = "Collapsed";
                }

            }
            else if (shipment.IdTimeNavigation?.DateTimeFactRegist == null && shipment.IdTimeNavigation?.DateTimePlanRegist != null)
            {
                TextStatus = "";
                btn_endload = "Collapsed";
                btn_load = "Collapsed";
            }
            else if (shipment.IdTimeNavigation?.DateTimePlanRegist == null && shipment.IdTimeNavigation?.DateTimeFactRegist != null)
            {
                TextStatus = $"Зарегистрирован ({DateTimeFactRegist})";
                btn_endload = "Collapsed";
                btn_load = "Collapsed";
            }

        }
    }
}
