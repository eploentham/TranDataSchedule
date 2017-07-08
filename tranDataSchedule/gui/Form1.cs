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

namespace tranDataSchedule
{
    public partial class Form1 : Form
    {
        TranDataScheduleControl tdsC;
        public Form1()
        {
            InitializeComponent();
            initConfig();
        }
        private void initConfig()
        {
            tdsC = new TranDataScheduleControl();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            chkAuto.Checked = true;
            showChkAuto();
            setTimeCurrent();
            timer1.Interval = 1000 * 60;
            timer1.Start();
            txtConGPSOnLIne.Text = tdsC.conn.connOnLine.ConnectionString;
            txtConnGPS01.Text = tdsC.conn.conn01.ConnectionString;
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
        private void setTimeCurrent()
        {
            txtTimeCurrent.Text = String.Format("{0:hhmm}",System.DateTime.Now);
        }
        private void selectCar(String dateStart, String dateEnd)
        {
            //String dateStart = "", dateEnd = "";
            String sql = "", carId="", day2="";
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sqlTrip = new StringBuilder();
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


            connDaily.ConnectionString = txtConnDaily.Text;
            conn01.ConnectionString = txtConnGPS01.Text;
            connDaily.Open();
            conn01.Open();
            comDaily.Connection = connDaily;
            com01.Connection = conn01;
            MySqlDataAdapter adap01 = new MySqlDataAdapter(com01);

            Double km = 0.0;
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
            for (int i = 0; i < dtCar.Rows.Count; i++)
            {
                sql1.Clear();
                sqlTrip.Clear();
                stripStart = false;
                stripEnd = false;
                //sqlTrip.Append("Select imei, gps_date, gps_time, gps_input1, gps_speed From positionbackup Where imei = '")
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("' and gps_ign = 1 and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                sqlTrip.Append("Select imei, gps_date, gps_time, gps_input1, gps_speed From positionbackup Where imei = '")
                    .Append(dtCar.Rows[i]["imei"].ToString()).Append("'  and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                com01.CommandText = sqlTrip.ToString();
                adap01.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    rowStart = 0;
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (j == 0) continue;
                        if (dt.Rows[rowStart]["imei"].ToString().Equals("56072558"))
                        {
                            if (dt.Rows[rowStart]["gps_time"].ToString().Equals("06:13:31"))
                            {
                                sql = "";
                            }
                        }
                        if (((Boolean)dt.Rows[j]["gps_input1"] == true) && ((Boolean)dt.Rows[j-1]["gps_input1"] == false))//trip start
                        {
                            if ((int)dt.Rows[j]["gps_speed"]>0)
                            {
                                stripStart = true;
                                rowStart = j;
                                //lB1.Items.Add("Trip Start "+ dt.Rows[j]["gps_time"]);
                            }
                        }
                        if(((Boolean)dt.Rows[j]["gps_input1"] == false) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == true))//trip end
                        {
                            if ((int)dt.Rows[j]["gps_speed"] == 0)
                            {
                                stripEnd = true;
                                //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                            }
                        }
                        if(stripStart && stripEnd)
                        {
                            lB1.Items.Add(" imei " + dt.Rows[rowStart]["imei"] +" Trip Start " + dt.Rows[rowStart]["gps_time"]+ " Trip End " + dt.Rows[j]["gps_time"]);
                        }
                    }
                }
                //carId = dtCar.Rows[i]["car_id"].ToString();
                //lB1.Items.Add(carId);
                //km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
                //    km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConGPSOnLIne.Text);
                //day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), tdsC.conn.conn01.ConnectionString);
                day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
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
                lB1.Items.Add(dtCar.Rows[i]["car_id"].ToString() + " ระยะทาง " + tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text) + " จำนวนรับผู้โดยสาร " + day2[0] + " รายได้ " + day2[1] + " ระยะทางรับผู้โดยสาร " + day2[2]);
                lB1.Refresh();
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
            setTimeCurrent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtTimeStart.Text = System.DateTime.Now.ToShortTimeString();
            selectCar(txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"), txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"));
            txtTimeEnd.Text = System.DateTime.Now.ToShortTimeString();
        }
    }
}
