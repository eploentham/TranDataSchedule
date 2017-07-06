using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tranDataSchedule.object1;

namespace tranDataSchedule.objdb
{
    public class CarDB
    {
        public ConnectDB conn;
        public Car car;
        public CarDB()
        {
            initConfig();
        }
        private void initConfig()
        {
            conn = new ConnectDB();
            car = new Car();
            car.pkField = "ID";
            car.table = "car";

            car.carId = "id";
            car.gpsId = "gps_id";
            car.ID = "id";
            car.imei = "imei";
            car.custId = "customer_id";
        }
        public DataTable selectAll()
        {
            DataTable dt = new DataTable();
            String sql = "";
            sql = "Select * " +
                "From " + car.table + " " ;
            dt = conn.selectData(sql);
            return dt;
        }
        public DataTable selectAll(String conString)
        {
            DataTable dt = new DataTable();
            String sql = "";
            sql = "Select * " +
                "From " + car.table + " ";
            dt = conn.selectData(sql, conString);
            return dt;
        }
        private Car setData(Car p, DataTable dt)
        {
            p.carId = dt.Rows[0][car.carId].ToString();
            p.gpsId = dt.Rows[0][car.gpsId].ToString();
            p.ID = dt.Rows[0][car.ID].ToString();
            p.imei = dt.Rows[0][car.imei].ToString();
            p.custId = dt.Rows[0][car.custId].ToString();
            
            return p;
        }
    }
}
