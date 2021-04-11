using System;
using System.Collections.Generic;
using System.Linq;
using ModernWpf;
using Registrant.Models;

namespace Registrant.Controllers
{
    public class DriversController
    {
        List<Drivers> Driver { get; set; }

        public DriversController()
        {
            Driver = new List<Drivers>();
        }

        public List<Drivers> GetDrivers()
        {
            Driver.Clear();
            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    var drivers = ef.Drivers.Where(x => x.Active != "0").OrderByDescending(x => x.IdDriver);

                    foreach (var item in drivers)
                    {
                        Drivers driver = new Drivers(item);
                        Driver.Add(driver);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
            return Driver;
        }

        public List<Drivers> GetDriversAll()
        {
            Driver.Clear();
            try
            {
                using (DB.RegistrantCoreContext ef = new DB.RegistrantCoreContext())
                {
                    var drivers = ef.Drivers.OrderByDescending(x => x.IdDriver);

                    foreach (var item in drivers)
                    {
                        Drivers driver = new Drivers(item);
                        Driver.Add(driver);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
            return Driver;
        }

    }
}
