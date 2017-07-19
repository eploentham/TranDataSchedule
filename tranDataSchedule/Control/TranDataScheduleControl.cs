using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tranDataSchedule.objdb;
using tranDataSchedule.object1;

namespace tranDataSchedule.Control
{
    public class TranDataScheduleControl
    {
        public ConnectDB conn;
        public ComboBox cbo;
        public CarDB carDb;
        public Car car;
        public SQL sql;
        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }

        public TranDataScheduleControl()
        {
            conn = new ConnectDB();
            car = new Car();
            sql = new SQL();

            carDb = new CarDB();
        }
        public DataTable selectCarAll()
        {
            DataTable dt = new DataTable();

            dt = carDb.selectAll();

            return dt;
        }
        public DataTable selectCarAll(String conString)
        {
            DataTable dt = new DataTable();

            dt = carDb.selectAll(conString);

            return dt;
        }
        public Double selectDistinct()
        {
            Double km = 0.0;
            
            return km;
        }
        public String setTimeCurrent()
        {
            return String.Format("{0:HHmm}", System.DateTime.Now);
        }
    }
}
