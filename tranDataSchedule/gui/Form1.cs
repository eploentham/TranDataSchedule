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
/**
 * 2017-07-21
 * 1. bug date taxi_meter ลงผิด format ลงเป็น 2560
 * 2017-07-30
 * 2. change connection to array
 * 2017-07-31
 * 3. แก้ query gpsbackup_xx ให้ดึงข้อมูล imei ตามค่า number
 */
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
            txtConnDaily.Text = tdsC.conn.connOnLine.ConnectionString.Replace("gpsonline","daily_report");

            //txtConGPSOnLIne.Text = "server=localhost;database=gpsonline;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
            //txtConnGPS01.Text = "server=localhost;database=gps_backup_01;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
            //txtConnDaily.Text = "server=localhost;database=daily_report;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";
            //this.Text = "Last Update 30-07-2560 1. bug date taxi_meter ลงผิด format ลงเป็น 2560";
            this.Text = "Last Update 31-07-2560 2. change connection to array";
        }
        private void showChkAuto()
        {
            if (chkAuto.Checked)
            {
                gbManual.Visible = false;
                txtAutoStart.Visible = true;
                label9.Visible = true;
            }
            else
            {
                gbManual.Visible = true;
                txtAutoStart.Visible = false;
                label9.Visible = false;
            }
        }
        
        private void selectCar(String dateStart, String dateEnd)
        {
            //String dateStart = "", dateEnd = "";
            String sql = "", carId="", day2="", err="", timeStart="", timeEnd="";
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sqlTrip = new StringBuilder();
            StringBuilder addr = new StringBuilder();
            StringBuilder bck = new StringBuilder();
            StringBuilder dtS = new StringBuilder();
            StringBuilder dtE = new StringBuilder();
            DataTable dtCar = new DataTable();
            MySqlConnection connDaily = new MySqlConnection();
            //MySqlConnection conn01 = new MySqlConnection();     //    -2
            MySqlConnection[] conn01 = new MySqlConnection[100];     //    +2
            MySqlCommand comDaily = new MySqlCommand();
            MySqlCommand com01 = new MySqlCommand();
            DataTable dt = new DataTable();
            int rowStart = 0, incomeTrip=0, incomeTripSum=0, TripCnt=0, connBck=0;
            
            Boolean stripStart = false;
            Boolean stripStartOld = false;
            Boolean stripEnd = false;
            Boolean insertTrip = false;
            
            connDaily.ConnectionString = txtConnDaily.Text;
            //conn01.ConnectionString = txtConnGPS01.Text;      //-2
            connDaily.Open();
            //conn01.Open();
            comDaily.Connection = connDaily;
            //com01.Connection = conn01;        //-2
            MySqlDataAdapter adap01 = new MySqlDataAdapter(com01);

            Double km = 0.0, distance = 0.0, distanceDay=0.0, distanceTripSum=0.0;
            DateTime dtStart, dtEnd;
            pB1.Show();
            pB1.Visible = true;
            pB1.Minimum = 0;

            //dateStart = (int.Parse(dateStart.Substring(0, 4)) + 543) + dateStart.Substring(4);
            //dateEnd = (int.Parse(dateEnd.Substring(0, 4)) + 543) + dateEnd.Substring(4);

            lB1.Items.Clear();
            lB1.Items.Add("ตรวจสอบข้อมูล");

            lB1.Items.Add("ประจำวันที่ " + dateStart + " ถึงวันที่ " + dateEnd);
            //MessageBox.Show("bck " );
            dtCar = tdsC.selectCarAll(txtConGPSOnLIne.Text);
            //MessageBox.Show("bck 111");
            pB1.Maximum = dtCar.Rows.Count;
            for (int i = 0; i < dtCar.Rows.Count; i++)// มีรถกี่คัน
            {
                sql1.Clear();
                
                stripStart = false;
                stripEnd = false;
                distanceDay = 0.0;
                distanceTripSum = 0.0;
                incomeTripSum = 0;
                TripCnt = 0;
                timeStart = tdsC.setTimeCurrent();
                //if (!dtCar.Rows[i]["imei"].ToString().Equals("58063983"))     //// for test
                //{
                //    continue;
                //}
                //if (!dtCar.Rows[i]["imei"].ToString().Equals("57032985"))     //// for test
                //{
                //    continue;
                //}
                bck.Clear();
                if (dtCar.Rows[i]["bck_server_id"].ToString().Length == 1)
                {
                    bck.Append("0").Append(dtCar.Rows[i]["bck_server_id"].ToString());
                }
                else
                {
                    bck.Append(dtCar.Rows[i]["bck_server_id"].ToString());
                }
                connBck = (int)dtCar.Rows[i]["bck_server_id"];      //  +2
                //if (conn01.State==ConnectionState.Open)   //  -2
                //{     //  -2
                //    conn01.Close();       //  -2
                //}     //  -2
                //bck.Clear();      //for test debug
                //bck.Append("01"); //for test debug
                if ((conn01[connBck] == null) || (conn01[connBck].State == ConnectionState.Closed))       //  +2
                {
                    try
                    {
                        //MessageBox.Show("bck 22");
                        conn01[connBck] = new MySqlConnection();
                        conn01[connBck].ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318;Connection Timeout = 300;default command timeout=0;";
                        //conn01[connBck].ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";port = 3306;Connection Timeout = 300;default command timeout=0;"; // for test
                        conn01[connBck].Open();
                        com01.Connection = conn01[connBck];      //+2
                        //MessageBox.Show("bck 2222");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("bck " + bck.ToString() + " i = " + i + " error " + ex.Message.ToString());
                    }
                }

                //conn01.ConnectionString = txtConnGPS01.Text;
                sqlTrip.Clear();
                //sqlTrip.Append("Select imei, gps_date, gps_time, gps_input1, gps_speed From positionbackup Where imei = '")
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("' and gps_ign = 1 and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                //sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon, packet_arrived_time From positionbackup Where imei = '")     // -3
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("'  and gps_date = '").Append(dateStart).Append("' Order By gps_time");          //      -3
                sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon, packet_arrived_time From positionbackup Where imei = '")       //  +3
                    .Append(dtCar.Rows[i]["imei"].ToString()).Append("'  and gps_date = '").Append(dateStart).Append("' Order By gps_time");        //  +3
                com01.CommandText = sqlTrip.ToString();// ดึงรถตาม imei ดึงทั้งวัน
                dt.Rows.Clear();
                adap01.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    rowStart = 0;
                    distance = 0.0;
                    distanceTripSum = 0.0;
                    for (int j = 0; j < dt.Rows.Count; j++)// ดึงรถตาม imei ดึงทั้งวัน
                    {
                        if (j == 0) continue;
                        try
                        {
                            err = "1000";
                            if (dt.Rows[j]["imei"].ToString().Equals("56072558"))// test bug, error
                            {
                                if (dt.Rows[j]["gps_time"].ToString().Equals("11:00:03"))
                                {
                                    sql = "";
                                }
                                if (dt.Rows[j]["gps_time"].ToString().Equals("10:02:03"))
                                {
                                    sql = "";
                                }
                                err = "1001";
                            }// test bug, error
                            if (j == 1)// รับ trip มาก่อนเที่ยงคืน
                            {

                            }
                            err = "2000";
                            //คำนวณ ระยะทางทั้งหมด
                            distanceDay += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[j - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lon"]) / 1000000);

                            // หา trip
                            if (((Boolean)dt.Rows[j]["gps_input1"] == true) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == false))//trip start 
                            /**
                                * การหาtrip start
                                * จะมี ความคลาดเคลื่อน เนื่องจาก
                                **/
                            {
                                if ((int)dt.Rows[j]["gps_speed"] > 0)// กดmeter แล้วออกรถเลย
                                {
                                    stripStart = true;
                                    stripEnd = false;
                                    rowStart = j;
                                    //lB1.Items.Add("Trip Start "+ dt.Rows[j]["gps_time"]);
                                }
                                err = "2001";
                                if (((int)dt.Rows[j]["gps_speed"] == 0) && ((int)dt.Rows[j + 1]["gps_speed"] > 0))// กดmeter แล้วยังไม่ออกรถทันที
                                {
                                    stripStart = true;
                                    stripEnd = false;
                                    rowStart = j;
                                }
                            }
                            err = "3000";
                            if (((Boolean)dt.Rows[j]["gps_input1"] == false) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == true) && stripStart)//trip end จะคำนวรหา end trip ได้ต้อง stripStart = true ก่อน
                            /**
                            * การหาtrip end
                            * จะมี ความคลาดเคลื่อน เนื่องจาก กด meter หยุดแล้วแต่ รถยังมีความเร็ว แต่ความเร็วจะเป้น 0 ก็จะเป็นข้อมูลถัดไป
                            **/
                            {
                                if ((int)dt.Rows[j]["gps_speed"] == 0)// รถจอด
                                {
                                    stripEnd = true;
                                    insertTrip = true;

                                    //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                                }
                                if ((int)dt.Rows[j]["gps_speed"] <= txtGPSError.Value)// รถจอด แต่ gps ส่งข้อมูลเป็น นาที ทำให้อาจ จอดปุ้บ แล้วรับคนใหม่ ทันที
                                {
                                    stripEnd = true;
                                    insertTrip = true;
                                    //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                                }
                            }
                            err = "4000";
                            if (stripStart && stripEnd && insertTrip)
                            {
                                distance = 0.0;
                                incomeTrip = 0;
                                
                                for (int k = rowStart; k < j; k++)//คำนวณหา ระยะทาง ระหว่างtrip
                                {
                                    distance += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[k - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lon"]) / 1000000);
                                }
                                err = "4002";
                                incomeTrip = tdsC.sql.PriceDay1(distance);
                                distanceTripSum += distance;
                                incomeTripSum += incomeTrip;
                                TripCnt++;
                                //String aa = "", bb="", cc="", dd="", ee="", ff="", gg="",hh="",ii="";
                                //addr.Clear();     // google address
                                //addr.Append(geo.ReverseGeoLoc((Convert.ToDouble(dt.Rows[j]["gps_lon"]) / 1000000).ToString(), (Convert.ToDouble(dt.Rows[rowStart]["gps_lat"]) / 1000000).ToString(), out aa, out bb, out cc, out dd, out ee, out ff, out gg, out hh, out ii));
                                err = "5000";
                                sql1.Clear();
                                
                                try
                                {
                                    dtStart = (DateTime)dt.Rows[rowStart]["gps_date_time"];     // +1.
                                    dtS.Clear();// +1.
                                    dtS.Append(dtStart.Year + "-" + dtStart.ToString("MM-dd HH:mm:ss"));// +1.
                                    dtEnd = (DateTime)dt.Rows[j]["gps_date_time"];      // +1.
                                    dtE.Clear();// +1.
                                    dtE.Append(dtEnd.Year + "-" + dtEnd.ToString("MM-dd HH:mm:ss"));// +1.
                                    //sql1.Append("Insert Into taxi_meter(t_imei, t_start_time, t_start_gps_lat, t_start_gps_lon")
                                    //.Append(", t_off_time, t_off_gps_lat, t_off_gps_lon, t_distance, t_taxi_fare ) ")
                                    //.Append("Values('").Append(dtCar.Rows[i]["imei"].ToString()).Append("','").Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[rowStart]["gps_date_time"])).Append("','").Append(dt.Rows[rowStart]["gps_lat"].ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lon"].ToString()).Append("'")
                                    //.Append(",'").Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[j]["gps_date_time"])).Append("','")
                                    //.Append(dt.Rows[j]["gps_lat"].ToString()).Append("','").Append(dt.Rows[j]["gps_lon"].ToString()).Append("',")
                                    //.Append(distance).Append(",").Append(incomeTrip).Append(") ");        // -1.
                                    sql1.Append("Insert Into taxi_meter(t_imei, t_start_time, t_start_gps_lat, t_start_gps_lon")
                                    .Append(", t_off_time, t_off_gps_lat, t_off_gps_lon, t_distance, t_taxi_fare ) ")
                                    .Append("Values('").Append(dtCar.Rows[i]["imei"].ToString()).Append("','").Append(dtS.ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lat"].ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lon"].ToString()).Append("'")
                                    .Append(",'").Append(dtE.ToString()).Append("','").Append(dt.Rows[j]["gps_lat"].ToString()).Append("','")
                                    .Append(dt.Rows[j]["gps_lon"].ToString()).Append("',").Append(distance).Append(",")
                                    .Append(incomeTrip).Append(") ");       // +1.
                                    comDaily.CommandText = sql1.ToString();

                                    comDaily.ExecuteNonQuery();
                                    stripStart = false;
                                    //throw ();
                                }
                                catch (Exception ex)
                                {
                                    lB1.Items.Add(ex.Message.ToString());
                                }
                                finally
                                {

                                }
                                lB1.Items.Add(" imei " + dt.Rows[rowStart]["imei"] + " Trip Start " + dt.Rows[rowStart]["gps_time"]
                                    + " Trip End " + dt.Rows[j]["gps_time"] + " ระยะทาง " + distance + " ที่อยู่ " + addr.ToString());
                                insertTrip = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            //MessageBox.Show("Error "+err+" row "+i+"\n" + ex.Message.ToString());
                            lB1.Items.Add(ex.Message.ToString());
                        }
                        
                    }
                }
                //carId = dtCar.Rows[i]["car_id"].ToString();
                //lB1.Items.Add(carId);
                //km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
                //    km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConGPSOnLIne.Text);
                //day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), tdsC.conn.conn01.ConnectionString);
                
                //conn01.ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318";
                //MessageBox.Show("bck 33 " + "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318");
                //day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318");
                //MessageBox.Show("bck 3333");
                sql1.Clear();
                
                try
                {
                    sql1.Append("Insert Into car_daily(car_daily_id, car_id, imei, daily_date, distance, income, trip_cnt, trip_distance, time_start, time_end, time_schedule, date_start) ")
                    .Append("Values(UUID()").Append(",'").Append(dtCar.Rows[i]["car_id"].ToString()).Append("','").Append(dtCar.Rows[i]["imei"].ToString())
                    .Append("','").Append(dateStart).Append("','").Append(distanceDay)
                    .Append("',").Append(incomeTripSum).Append(",'").Append(TripCnt).Append("','").Append(distanceTripSum).Append("','").Append(timeStart).Append("','").Append(tdsC.setTimeCurrent()).Append("','").Append(txtAutoStart.Text).Append("', now())");
                    comDaily.CommandText = sql1.ToString();

                    comDaily.ExecuteNonQuery();
                    //this.Invalidate();
                }
                catch (Exception ex)
                {
                    lB1.Items.Add(ex.Message.ToString());
                }
                finally
                {

                }
                
                //lB1.Items.Add(dtCar.Rows[i]["car_id"].ToString()+"["+ dtCar.Rows[i]["imei"].ToString() + "] ระยะทาง " 
                //    + tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text) + " จำนวนรับผู้โดยสาร " + day2[0] 
                //    + " รายได้ " + day2[1] + " ระยะทางรับผู้โดยสาร " + day2[2]+ " distanceDay " + distanceDay+" distanceTripSum "+distanceTripSum);
                lB1.Items.Add(dtCar.Rows[i]["car_id"].ToString() + "[" + dtCar.Rows[i]["imei"].ToString() + "] ระยะทาง "
                    + distanceDay + " จำนวนรับผู้โดยสาร " + TripCnt
                    + " รายได้ " + incomeTripSum + " ระยะทางรับผู้โดยสาร " + distanceTripSum + " distanceDay " + distanceDay );
                //lB1.Refresh();
                pB1.Value = i;
                //conn01.Close();       //-2
                this.Refresh();
                
            }
            connDaily.Close();
            for(int i = 0; i < 100; i++)//  +2
            {
                if ((conn01[i] != null) && (conn01[i].State == ConnectionState.Open))       //  +2
                {
                    conn01[connBck].Close();
                }
            }
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
            if (txtTimeCurrent.Text.Equals(txtAutoStart.Text))
            {
                txtTimeStart.Text = tdsC.setTimeCurrent();
                DateTime startDate = Convert.ToDateTime(System.DateTime.Now).AddDays(-1);
                selectCar(startDate.Year.ToString() + "-" + startDate.ToString("MM-dd"), startDate.Year.ToString() + "-" + startDate.ToString("MM-dd"));
                txtTimeEnd.Text = tdsC.setTimeCurrent();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtTimeStart.Text = tdsC.setTimeCurrent();
            //String aaa = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.ToString("MM-dd");
            //aaa = "aa";
            selectCar(txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"), txtDateManual.Value.Year.ToString() + "-" + txtDateManual.Value.ToString("MM-dd"));
            txtTimeEnd.Text = tdsC.setTimeCurrent();
        }
    }
}
