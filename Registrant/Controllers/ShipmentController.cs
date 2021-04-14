using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Registrant.DB;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class ShipmentController
    {
        List<Shipments> Shipments { get; set; }

        public ShipmentController()
        {
            Shipments = new List<Shipments>();
        }

        public List<Shipments> GetShipments(DateTime date)
        {
            Shipments.Clear();

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipments = ef.Shipments.Where(x => 
                        (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                        && x.Active != "0")
                    .OrderByDescending(x => x.IdTimeNavigation.DateTimePlanRegist);
                
                foreach (var item in shipments)
                {
                    Shipments shipment = new Shipments(item);
                    Shipments.Add(shipment);
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
            return Shipments;
        }

        /// <summary>
        /// Получение всего
        /// </summary>
        /// <returns></returns>
        public List<Shipments> GetShipmentsAll()
        {
            Shipments.Clear();
            
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipments = ef.Shipments.OrderByDescending(x => x.IdTimeNavigation.DateTimePlanRegist);
                foreach (var item in shipments)
                {
                    Shipments shipment = new Shipments(item);
                    Shipments.Add(shipment);
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
            return Shipments;
        }
        /// <summary>
        /// Только кто фактически реганулся
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Shipments> GetShipmentsFactReg(DateTime date)
        {
            Shipments.Clear();

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipments = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                   && x.IdTimeNavigation.DateTimeLeft == null 
                                                   && x.IdTimeNavigation.DateTimeArrive == null && x.Active != "0")
                    .OrderByDescending(x => x.IdTimeNavigation.DateTimePlanRegist);
                
                foreach (var item in shipments)
                {
                    Shipments shipment = new Shipments(item);
                    Shipments.Add(shipment);
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
            return Shipments;
        }
        
        /// <summary>
        /// Только прибывшие
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Shipments> GetShipmentsArrive(DateTime date)
        {
            Shipments.Clear();

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipments = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                   && x.IdTimeNavigation.DateTimeArrive != null 
                                                   && x.IdTimeNavigation.DateTimeLeft == null 
                                                   && x.Active != "0")
                    .OrderByDescending(x => x.IdTimeNavigation.DateTimePlanRegist);
                
                foreach (var item in shipments)
                {
                    Shipments shipment = new Shipments(item);
                    Shipments.Add(shipment);
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
            return Shipments;
        }
        /// <summary>
        /// Покинули
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Shipments> GetShipmentsLeft(DateTime date)
        {
            Shipments.Clear();

            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var shipments = ef.Shipments.Where(x => (x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date.Date) 
                                                        && x.IdTimeNavigation.DateTimeLeft != null 
                                                        && x.Active != "0")
                    .OrderByDescending(x => x.IdTimeNavigation.DateTimePlanRegist);
                
                foreach (var item in shipments)
                {
                    Shipments shipment = new Shipments(item);
                    Shipments.Add(shipment);
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
            return Shipments;
        }
    }
}
