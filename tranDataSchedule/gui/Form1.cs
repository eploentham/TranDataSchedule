using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tranDataSchedule.Control;
using tranDataSchedule.object1;

namespace tranDataSchedule
{
    public partial class Form1 : Form
    {
        TranDataScheduleControl tdsC;
        SQL sql;
        ReverseGeoLookup geo;
        public Form1()
        {
            InitializeComponent();
            initConfig();
        }
        private void initConfig()
        {
            tdsC = new TranDataScheduleControl();
            sql = new SQL();
            geo = new ReverseGeoLookup();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            chkAuto.Checked = true;
            showChkAuto();
            txtTimeCurrent.Text = tdsC.setTimeCurrent();
            timer1.Interval = 1000 * 60;
            timer1.Start();
            txtConGPSOnLIne.Text = tdsC.conn.connOnLine.ConnectionString;//server=localhost;database=gpsonline;user id=root;password=-;port=6318
            txtConnGPS01.Text = tdsC.conn.conn01.ConnectionString;
            txtConGPSOnLIne.Text = "server=localhost;database=gpsonline;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
            txtConnGPS01.Text = "server=localhost;database=gps_backup_01;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
            txtConnDaily.Text = "server=localhost;database=daily_report;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
        }
        private void showChkAuto()
        {
            if (chkAuto.Checked)
            {
                gbManual.Visible = false;
            }
            else
            {
                gbManual.Visible = true;
            }
        }
        
        private void selectCar(String dateStart, String dateEnd)
        {
            //String dateStart = "", dateEnd = "";
            String sql = "", carId="", day2="";
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sqlTrip = new StringBuilder();
            StringBuilder addr = new StringBuilder();
            DataTable dtCar = new DataTable();
            MySqlConnection connDaily = new MySqlConnection();
            MySqlConnection conn01 = new MySqlConnection();
            MySqlCommand comDaily = new MySqlCommand();
            MySqlCommand com01 = new MySqlCommand();
            DataTable dt = new DataTable();
            int rowStart = 0;
            
            Boolean stripStart = false;
            Boolean stripStartOld = false;
            Boolean stripEnd = false;
            Boolean insertTrip = false;
            
            connDaily.ConnectionString = txtConnDaily.Text;
            conn01.ConnectionString = txtConnGPS01.Text;
            connDaily.Open();
            conn01.Open();
            comDaily.Connection = connDaily;
            com01.Connection = conn01;
            MySqlDataAdapter adap01 = new MySqlDataAdapter(com01);

            Double km = 0.0, distance = 0.0, distanceDay=0.0;
            pB1.Show();
            pB1.Visible = true;
            pB1.Minimum = 0;

            //dateStart = (int.Parse(dateStart.Substring(0, 4)) + 543) + dateStart.Substring(4);
            //dateEnd = (int.Parse(dateEnd.Substring(0, 4)) + 543) + dateEnd.Substring(4);

            lB1.Items.Clear();
            lB1.Items.Add("ตรวจสอบข้อมูล");

            lB1.Items.Add("ประจำวันที่ " + dateStart + " ถึงวันที่ " + dateEnd);

            dtCar = tdsC.selectCarAll(txtConGPSOnLIne.Text);
            pB1.Maximum = dtCar.Rows.Count;
            for (int i = 0; i < dtCar.Rows.Count; i++)// มีรถกี่คัน
            {
                sql1.Clear();
                sqlTrip.Clear();
                stripStart = false;
                stripEnd = false;
                if (!dtCar.Rows[i]["imei"].ToString().Equals("58063983"))
                {
                    continue;
                }
                //sqlTrip.Append("Select imei, gps_date, gps_time, gps_input1, gps_speed From positionbackup Where imei = '")
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("' and gps_ign = 1 and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon From positionbackup Where imei = '")
                    .Append(dtCar.Rows[i]["imei"].ToString()).Append("'  and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                com01.CommandText = sqlTrip.ToString();// ดึงรถตาม imei ดึงทั้งวัน
                dt.Rows.Clear();
                adap01.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    rowStart = 0;
                    distance = 0.0;
                    for (int j = 0; j < dt.Rows.Count; j++)// ดึงรถตาม imei ดึงทั้งวัน
                    {
                        if (j == 0) continue;
                        if (dt.Rows[j]["imei"].ToString().Equals("58063983"))// test bug, error
                        {
                            if (dt.Rows[j]["gps_time"].ToString().Equals("09:37:22"))
                            {
                                sql = "";
                            }
                            if (dt.Rows[j]["gps_time"].ToString().Equals("11:22:22"))
                            {
                                sql = "";
                            }
                        }// test bug, error

                        //คำนวณ ระยะทางทั้งหมด
                        distanceDay += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[j - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lon"]) / 1000000);

                        // หา trip
                        if (((Boolean)dt.Rows[j]["gps_input1"] == true) && ((Boolean)dt.Rows[j-1]["gps_input1"] == false))//trip start 
                        {
                            if ((int)dt.Rows[j]["gps_speed"]>0)// กดmeter แล้วออกรถเลย
                            {
                                stripStart = true;
                                stripEnd = false;
                                rowStart = j;
                                //lB1.Items.Add("Trip Start "+ dt.Rows[j]["gps_time"]);
                            }
                            if (((int)dt.Rows[j]["gps_speed"] == 0) && ((int)dt.Rows[j + 1]["gps_speed"] > 0))// กดmeter แล้วยังไม่ออกรถทันที
                            {
                                stripStart = true;
                                stripEnd = false;
                                rowStart = j;
                            }
                        }
                        if(((Boolean)dt.Rows[j]["gps_input1"] == false) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == true))//trip end
                        {
                            if ((int)dt.Rows[j]["gps_speed"] == 0)// รถจอด
                            {
                                stripEnd = true;
                                insertTrip = true;
                                //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                            }
                            if ((int)dt.Rows[j]["gps_speed"] >= txtGPSError.Value)// รถจอด แต่ gps ส่งข้อมูลเป็น นาที ทำให้อาจ จอดปุ้บ แล้วรับคนใหม่ ทันที
                            {
                                stripEnd = true;
                                insertTrip = true;
                                //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                            }
                        }
                        if(stripStart && stripEnd && insertTrip)
                        {
                            distance = 0.0;
                            for (int k = rowStart; k < j; k++)//คำนวณหา ระยะทาง ระหว่างtrip
                            {
                                distance += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[k - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lon"]) / 1000000);
                            }

                            //String aa = "", bb="", cc="", dd="", ee="", ff="", gg="",hh="",ii="";
                            //addr.Clear();     // google address
                            //addr.Append(geo.ReverseGeoLoc((Convert.ToDouble(dt.Rows[j]["gps_lon"]) / 1000000).ToString(), (Convert.ToDouble(dt.Rows[rowStart]["gps_lat"]) / 1000000).ToString(), out aa, out bb, out cc, out dd, out ee, out ff, out gg, out hh, out ii));

                            sql1.Clear();
                            sql1.Append("Insert Into taxi_meter(t_imei, t_start_time, t_start_gps_lat, t_start_gps_lon")
                                .Append(", t_off_time, t_off_gps_lat, t_off_gps_lon, t_distance ) ")
                                .Append("Values('").Append(dtCar.Rows[i]["imei"].ToString()).Append("','").Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[rowStart]["gps_date_time"])).Append("','").Append(dt.Rows[rowStart]["gps_lat"].ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lon"].ToString()).Append("'")
                                .Append(",'").Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[j]["gps_date_time"])).Append("','").Append(dt.Rows[j]["gps_lat"].ToString()).Append("','").Append(dt.Rows[j]["gps_lon"].ToString()).Append("',").Append(distance).Append(") ");
                            comDaily.CommandText = sql1.ToString();
                            try
                            {
                                comDaily.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                lB1.Items.Add(ex.Message.ToString());
                            }
                            finally
                            {

                            }
                            lB1.Items.Add(" imei " + dt.Rows[rowStart]["imei"] +" Trip Start " + dt.Rows[rowStart]["gps_time"]
                                + " Trip End " + dt.Rows[j]["gps_time"]+" ระยะทาง "+ distance + " ที่อยู่ "+addr.ToString());
                            insertTrip = false;
                        }
                    }
                }
                //carId = dtCar.Rows[i]["car_id"].ToString();
                //lB1.Items.Add(carId);
                //km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
                //    km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConGPSOnLIne.Text);
                //day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), tdsC.conn.conn01.ConnectionString);
                day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
                sql1.Clear();
                sql1.Append("Insert Into car_daily(car_daily_id, car_id, imei, daily_date, distance, income, trip_cnt, trip_distance) ")
                .Append("Values(UUID()").Append(",'").Append(dtCar.Rows[i]["car_id"].ToString()).Append("','").Append(dtCar.Rows[i]["imei"].ToString())
                .Append("','").Append(dateStart).Append("','").Append(tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text))
                .Append("',").Append(day2.ToString().Split(':')[1]).Append(",'").Append(day2.ToString().Split(':')[0]).Append("','").Append(day2.ToString().Split(':')[2]).Append("')");
                comDaily.CommandText = sql1.ToString();
                try
                {
                    comDaily.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    lB1.Items.Add(ex.Message.ToString());
                }
                finally
                {

                }

                lB1.Items.Add(dtCar.Rows[i]["car_id"].ToString()+"["+ dtCar.Rows[i]["imei"].ToString() + "] ระยะทาง " 
                    + tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text) + " จำนวนรับผู้โดยสาร " + day2[0] 
                    + " รายได้ " + day2[1] + " ระยะทางรับผู้โดยสาร " + day2[2]+ " distanceDay " + distanceDay);
                //lB1.Refresh();
                pB1.Value = i;
            }
            connDaily.Close();
            pB1.Visible = false;
        }

        private void chkManual_Click(object sender, EventArgs e)
        {
            showChkAuto();
        }

        private void chkAuto_Click(object sender, EventArgs e)
        {
            showChkAuto();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtTimeCurrent.Text = tdsC.setTimeCurrent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtTimeStart.Text = tdsC.setTimeCurrent();
            selectCar(txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"), txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"));
            txtTimeEnd.Text = tdsC.setTimeCurrent();
        }
    }
}
