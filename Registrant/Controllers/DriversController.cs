using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Registrant.DB;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class DriversController
    {
        public List<Drivers> Driver { get; set; }

        public DriversController()
        {
            Driver = new List<Drivers>();
        }

        public List<Drivers> GetDrivers()
        {
            Driver.Clear();
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var drivers = ef.Drivers.Where(x => x.Active != "0").OrderByDescending(x => x.IdDriver);

                foreach (var item in drivers)
                {
                    Drivers driver = new Drivers(item);
                    Driver.Add(driver);
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
            return Driver;
        }
        //Только выбранный и остальные не активные
        public List<Drivers> GetDriversCurrent(int id)
        {
            Driver.Clear();
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var drivers = ef.Drivers.Where(x => x.Active != "0" | x.IdDriver == id).OrderBy(x => x.Family);

                foreach (var item in drivers)
                {
                    Drivers driver = new Drivers(item);
                    Driver.Add(driver);
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
            return Driver;
        }
        
        //Только активные для чекбокса
        public List<Drivers> GetDriversCurrent()
        {
            Driver.Clear();
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var drivers = ef.Drivers.Where(x => x.Active != "0").OrderBy(x => x.Family);

                foreach (var item in drivers)
                {
                    Drivers driver = new Drivers(item);
                    Driver.Add(driver);
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
            return Driver;
        }

        //Все в том числе неактивные
        public List<Drivers> GetDriversAll()
        {
            Driver.Clear();
            try
            {
                using RegistrantCoreContext ef = new RegistrantCoreContext();
                var drivers = ef.Drivers.OrderByDescending(x => x.IdDriver);

                foreach (var item in drivers)
                {
                    Drivers driver = new Drivers(item);
                    Driver.Add(driver);
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
            return Driver;
        }

    }
}
