using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        //Получит список для печати СБЫТ
        public List<Models.PrintShipments> GetShipmentsDate(DateTime date)
        {
            PlanShipments.Clear();

            try
            {
                using DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext();
                var shipments =
                    ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date.Date || 
                                            x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date)
                        .OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
                foreach (var item in shipments)
                {
                    //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date);  на это выбивает null
                    //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.HasValue ? x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date : false || x.IdTimeNavigation.DateTimeFactRegist.HasValue ? x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date : false);
                    //var temp = ef.Shipments.Where(x => ((x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date) && x.Active != "0")).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
                    var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);

                    foreach (var item in temp)
                    {
                        Models.PrintShipments shipment = new Models.PrintShipments(item);
                        PlanShipments.Add(shipment);
                    }
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

        //Получит список для печати СКЛАД
        public List<Models.PrintShipments> GetShipmentsDateSklad(DateTime date)
        {
            PlanShipments.Clear();

            date = date.Date;
            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date);  на это выбивает null
                    //var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.HasValue ? x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date : false || x.IdTimeNavigation.DateTimeFactRegist.HasValue ? x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date : false);
                    //var temp = ef.Shipments.Where(x => ((x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date) && x.Active != "0")).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
                    var temp = ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Date == date || x.IdTimeNavigation.DateTimeFactRegist.Value.Date == date).OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist).OrderBy(x => x.IdTimeNavigation.DateTimeFactRegist);

                    foreach (var item in temp)
                    {
                        Models.PrintShipments shipment = new Models.PrintShipments(item);
                        PlanShipments.Add(shipment);
                    }
                }
            }
            catch (Exception ex)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).ContentErrorText.ShowAsync();
                ((MainWindow)System.Windows.Application.Current.MainWindow).text_debuger.Text = ex.ToString();
            }
            return PlanShipments;
        }


       public List<PrintShipments> GetShipmentsMonth(DateTime date)
        {
            PlanShipments.Clear();

            try
            {
                using DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext();
                var shipments =
                    ef.Shipments.Where(x => x.IdTimeNavigation.DateTimePlanRegist.Value.Month == date.Month 
                                            || x.IdTimeNavigation.DateTimeFactRegist.Value.Month == date.Month)
                        .OrderBy(x => x.IdTimeNavigation.DateTimePlanRegist);
                foreach (var item in shipments)
                {
                    PrintShipments shipment = new PrintShipments(item);
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
