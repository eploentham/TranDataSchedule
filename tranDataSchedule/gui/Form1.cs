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
 * 2017-08-03
 * 4. เอา connection array ออก   un comment 2. และเพิ่ม +4
 * Test ใช้ array แล้ว ข้อมูลไม่มา หาสาเหต ไม่เจอ คือ query ข้อมูลไม่ขึ้น ได้บ้าง ไม่ได้บ้าง แต่ได้น้อยมาก query บันทัด 188
 * อาจต้องแก้ ให้ com01 = new MySqlCommand(); บันทัด 162 +4 อาจแก้ปัญหาได้ แต่ยังไม่ได้ทำ
 * 5. ปรับโปรแรกม เพิ่ม Field daily_report.customer_id และ taxi_meter.customer_id
 * 6. ปรับโปรแกรม เปลี่ยน database ไป aws rds
 * 7. Bug customer_id
 * 8. แก้เรื่อง จุดทศนิยม ให้มีแค่ 2 จุด   ยังไม่ได้ทำ  60-08-07 รับแจ้ง 60-08-05 เจ้าของแจ้ง 
 * 9. แก้เรื่อง charractor set ผิด แก้ไข และต้อง convert update 
 * 10. ceil ทำการคำนวณหา ฐานนิยม โดยการเพิ่ม field เก็บ ceil คือการปัดให้เป็น หลักร้อย        2017-08-14
 * 11. แก้เรื่อง รถจอด แล้วรับผู้โดยสาร ทันที จอดรถแล้ว ยังไม่กดmeter รับผู้โดยสารใหม่ แล้วค่อย กด meter ปิด แล้วเปิด  2017-08-20
 * 12. เรื่อง รับผู้โดยสาร ก่อนเที่ยงคืน แล้วเลยเที่ยงคืน ยังส่งผู้โดบสารไม่เสร็จ  2017-08-22
 * 13. เรื่อง กดmeter แล้วจอดรถ        2017-08-22
 * 14. ปรับให้ connection กลับมาใช้ server3 2017-09-09
 * 15. ปรับ start trip ให้รับแค่ input1 เท่านั้น   2017-09-14
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
            txtConnDaily.Text = tdsC.conn.connOnLine.ConnectionString.Replace("gpsonline", "daily_report");
            txtAutoStart.Text = "0300";
            //txtConnDaily.Text = "server=taxidashboard.ccegjxy0bmku.ap-southeast-1.rds.amazonaws.com;database=daily_report;user id=oriscom;password=mocsiro1*;port=3306;Character Set=utf8";


            //txtConGPSOnLIne.Text = "server=localhost;database=gpsonline;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";      //for test
            //txtConnGPS01.Text = "server=localhost;database=gps_backup_01;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";      //for test
            //txtConnDaily.Text = "server=localhost;database=daily_report;user id=root;password='';port=3306;Connection Timeout = 300;default command timeout=0;";      //for test

            txtConnGPS01.Enabled = false;
            //this.Text = "Last Update 30-07-2560 1. bug date taxi_meter ลงผิด format ลงเป็น 2560";
            this.Text = "Last Update 26-08-2560 12. 13.";
            pB1.Visible = false;
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
            String sql = "", carId = "", day2 = "", err = "", timeStart = "", timeEnd = "", sBeforeMidnight = "";
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sqlTrip = new StringBuilder();
            StringBuilder addr = new StringBuilder();
            StringBuilder bck = new StringBuilder();
            StringBuilder dtS = new StringBuilder();
            StringBuilder dtE = new StringBuilder();
            StringBuilder gpsLatBeforeMidnight = new StringBuilder();
            StringBuilder gpsLonBeforeMidnight = new StringBuilder();
            DataTable dtCar = new DataTable();
            MySqlConnection connDaily = new MySqlConnection();
            MySqlConnection conn01 = new MySqlConnection();     //    -2
            //MySqlConnection[] conn01 = new MySqlConnection[100];     //    +2
            MySqlCommand comDaily = new MySqlCommand();
            MySqlCommand com01 = new MySqlCommand();
            MySqlCommand comlast = new MySqlCommand();      //  +12
            DataTable dt = new DataTable();
            DataTable dtLast = new DataTable();     //  +12
            int rowStart = 0, incomeTrip = 0, incomeTripSum = 0, TripCnt = 0, connBck = 0, gpsSpeed0Input1ON = 0;

            Boolean stripStart = false;
            Boolean stripStartOld = false, tripBeforeMidnight = false;
            Boolean stripEnd = false;
            Boolean insertTrip = false;
            bool[] noInsert = new bool[20];
            int noInsertMax = (int)txtGPSErrorNoInsert.Value;          //+13
            //MessageBox.Show("bck 11");
            connDaily.ConnectionString = txtConnDaily.Text;
            //conn01.ConnectionString = txtConnGPS01.Text;      //-2
            //MessageBox.Show("bck 22");
            connDaily.Open();
            //MessageBox.Show("bck 33");
            //conn01.Open();
            comDaily.Connection = connDaily;
            com01.Connection = conn01;        //-2
            MySqlDataAdapter adap01 = new MySqlDataAdapter(com01);
            MySqlDataAdapter adapLast = new MySqlDataAdapter(comlast);      //  +12
            //MessageBox.Show("bck 11");
            Double km = 0.0, distance = 0.0, distanceDay = 0.0, distanceTripSum = 0.0, ceilIncome = 0.0, distanceTripBeforeMidnight = 0.0;
            //long gpsLatBeforeMidnight = 0, gpsLonBeforeMidnight = 0, gpsLatBeforeMidnightEnd = 0, gpsLonBeforeMidnightEnd = 0;
            DateTime dtStart, dtEnd, dtStartBeforeMidnight;
            pB1.Show();
            pB1.Visible = true;
            pB1.Minimum = 0;
            //MessageBox.Show("bck 22");
            //dateStart = (int.Parse(dateStart.Substring(0, 4)) + 543) + dateStart.Substring(4);
            //dateEnd = (int.Parse(dateEnd.Substring(0, 4)) + 543) + dateEnd.Substring(4);

            lB1.Items.Clear();
            lB1.Items.Add("ตรวจสอบข้อมูล");
            //MessageBox.Show("bck 33");
            lB1.Items.Add("ประจำวันที่ " + dateStart + " ถึงวันที่ " + dateEnd);
            //MessageBox.Show("bck " );
            dtCar = tdsC.selectCarAll(txtConGPSOnLIne.Text);
            //MessageBox.Show("bck 111");
            comDaily.CommandText = "Delete From car_daily Where daily_date = '" + dateStart + "'";
            comDaily.ExecuteNonQuery();
            //comDaily.CommandText = "Delete From taxi_meter Where t_start_time >= '" + dateStart + " 00:00:00' and t_start_time <= '" + dateStart + " 23:59:59'";
            comDaily.CommandText = "Delete From taxi_meter Where daily_date >= '" + dateStart + " 00:00:00' and daily_date <= '" + dateStart + " 23:59:59'";
            comDaily.ExecuteNonQuery();
            pB1.Maximum = dtCar.Rows.Count;

            for (int i = 0; i < dtCar.Rows.Count; i++)// มีรถกี่คัน
            {
                sql1.Clear();
                /**
                 * เตรียม noInsert เพื่อ กรณี จอดรถ แต่กด meter ไว้
                 */
                //for (int k = 0; k < 20; k++)    //   //+13
                //{
                //    noInsert[k] = false;
                //}
                stripStart = false;
                stripEnd = false;
                tripBeforeMidnight = false;
                sBeforeMidnight = "";
                distanceDay = 0.0;
                distanceTripSum = 0.0;
                gpsSpeed0Input1ON = 0;
                incomeTripSum = 0;
                distanceTripBeforeMidnight = 0.0;
                TripCnt = 0;
                timeStart = tdsC.setTimeCurrent();
                //if (!dtCar.Rows[i]["imei"].ToString().Equals("60016378"))     //// for test
                //{
                //    continue;
                //}
                //if (!dtCar.Rows[i]["imei"].ToString().Equals("59034945"))     //// for test
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
                if (conn01.State == ConnectionState.Open)   //  -2
                {     //  -2
                    conn01.Close();       //  -2
                }     //  -2
                //MessageBox.Show("bck "+bck.ToString());
                conn01.ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318;Connection Timeout = 300;default command timeout=0;";       //-2
                //conn01.ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";port = 3306;Connection Timeout = 300;default command timeout=0;"; // for test conn01.Open();       //-2
                conn01.Open();  // -2
                //MessageBox.Show("bck 222");
                com01 = new MySqlCommand();         // +4
                com01.Connection = conn01;          //+4
                //MessageBox.Show("bck 333");
                //bck.Clear();      //for test debug
                //bck.Append("01"); //for test debug
                //if ((conn01[connBck] == null) || (conn01[connBck].State == ConnectionState.Closed))       //  +2
                //{
                //    try
                //    {
                //        //MessageBox.Show("bck 22");
                //        conn01[connBck] = new MySqlConnection();
                //        conn01[connBck].ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";Pwd=" + tdsC.conn.passwordDB + ";port = 6318;Connection Timeout = 300;default command timeout=0;";
                //        //conn01[connBck].ConnectionString = "Server=" + tdsC.conn.hostDB + ";Database=gps_backup_" + bck.ToString() + ";Uid=" + tdsC.conn.userDB + ";port = 3306;Connection Timeout = 300;default command timeout=0;"; // for test
                //        conn01[connBck].Open();
                //        com01.Connection = conn01[connBck];      //+2
                //        //MessageBox.Show("bck 2222");
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show("bck " + bck.ToString() + " i = " + i + " error " + ex.Message.ToString());
                //    }
                //}

                //conn01.ConnectionString = txtConnGPS01.Text;
                sqlTrip.Clear();
                //sqlTrip.Append("Select imei, gps_date, gps_time, gps_input1, gps_speed From positionbackup Where imei = '")
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("' and gps_ign = 1 and gps_date = '").Append(dateStart).Append("' Order By gps_time");
                //sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon, packet_arrived_time From positionbackup Where imei = '")     // -3
                //    .Append(dtCar.Rows[i]["imei"].ToString()).Append("'  and gps_date = '").Append(dateStart).Append("' Order By gps_time");          //      -3
                sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon, packet_arrived_time From positionbackup Where imei = ")       //  +3
                    .Append(dtCar.Rows[i]["imei"].ToString()).Append("  and gps_date = '").Append(dateStart).Append("' Order By gps_time");        //  +3
                com01.CommandText = sqlTrip.ToString();// ดึงรถตาม imei ดึงทั้งวัน
                dt.Rows.Clear();
                //MessageBox.Show("bck 2222 i" + i);
                //MessageBox.Show("bck 3333 " + sqlTrip.ToString());
                adap01 = new MySqlDataAdapter(com01);       //+7
                adap01.Fill(dt);
                //MessageBox.Show("bck 4444 i" + i);
                if (dt.Rows.Count > 0)
                {
                    rowStart = 0;
                    distance = 0.0;
                    distanceTripSum = 0.0;
                    gpsSpeed0Input1ON = 0;
                    gpsLatBeforeMidnight.Clear();
                    gpsLonBeforeMidnight.Clear();
                    for (int j = 0; j < dt.Rows.Count; j++)// ดึงรถตาม imei ดึงทั้งวัน
                    {
                        if (j == 0)
                        {
                            //continue;     //on +12
                            if (((Boolean)dt.Rows[j]["gps_input1"] == true) && ((int)dt.Rows[j]["gps_speed"] > 0))  //  +12 รับ trip มาก่อนเที่ยงคืน
                            {
                                sqlTrip.Clear();        //  +12
                                sqlTrip.Append("Select imei, gps_date_time, gps_date, gps_time, gps_input1, gps_speed, gps_lat, gps_lon, packet_arrived_time From positionbackup Where imei = ")       //  +12
                                    .Append(dtCar.Rows[i]["imei"].ToString()).Append("  and gps_date = date_add('").Append(dateStart).Append("',INTERVAL -1 day) Order By gps_time desc");        //  +12
                                comlast = new MySqlCommand();                   // +12
                                comlast.Connection = conn01;                    // +12
                                comlast.CommandText = sqlTrip.ToString();       // +12
                                adapLast = new MySqlDataAdapter(comlast);       //  +12
                                adapLast.Fill(dtLast);
                                for (int m = 0; m < dtLast.Rows.Count; m++)
                                {
                                    //if (m == 0)
                                    //{
                                    //    gpsLatBeforeMidnightEnd = (long)dtLast.Rows[m]["gps_lat"];
                                    //    gpsLonBeforeMidnightEnd = (long)dtLast.Rows[m]["gps_lon"];
                                    //}
                                    if ((Boolean)dtLast.Rows[m]["gps_input1"] == false)
                                    {
                                        gpsLatBeforeMidnight.Clear();
                                        gpsLonBeforeMidnight.Clear();
                                        gpsLatBeforeMidnight.Append(dtLast.Rows[m - 1]["gps_lat"]);
                                        gpsLonBeforeMidnight.Append(dtLast.Rows[m - 1]["gps_lon"]);
                                        stripStart = true;
                                        stripEnd = false;
                                        tripBeforeMidnight = true;
                                        dtStartBeforeMidnight = (DateTime)dtLast.Rows[m - 1]["gps_date_time"];
                                        sBeforeMidnight = dtStartBeforeMidnight.Year + "-" + dtStartBeforeMidnight.ToString("MM-dd HH:mm:ss");
                                        distanceTripBeforeMidnight += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dtLast.Rows[m - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dtLast.Rows[m - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dtLast.Rows[0]["gps_lat"]) / 1000000, Convert.ToDouble(dtLast.Rows[0]["gps_lon"]) / 1000000);
                                        break;
                                    }
                                }
                            }
                        }
                        try
                        {
                            err = "1000";
                            if (dt.Rows[j]["imei"].ToString().Equals("60016364"))// test bug, error
                            {
                                if (dt.Rows[j]["gps_time"].ToString().Equals("08:39:33"))
                                {
                                    sql = "";
                                }
                                if (dt.Rows[j]["gps_time"].ToString().Equals("08:42:51"))
                                {
                                    sql = "";
                                }
                                err = "1001";
                            }// test bug, error
                            //if (j == 1)// รับ trip มาก่อนเที่ยงคืน
                            //{

                            //}
                            err = "2000";
                            //คำนวณ ระยะทางทั้งหมด
                            if (j != 0)
                            {
                                distanceDay += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[j - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[j]["gps_lon"]) / 1000000);
                            }
                            

                            // หา trip
                            if ((j > 0) && (j < dt.Rows.Count) && ((Boolean)dt.Rows[j]["gps_input1"] == true) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == false))//trip start 
                            /**
                                * การหาtrip start
                                * จะมี ความคลาดเคลื่อน เนื่องจาก
                                **/
                            {
                                //if ((int)dt.Rows[j]["gps_speed"] > 0)// กดmeter แล้วออกรถเลย       //-15
                                //{
                                //    stripStart = true;
                                //    stripEnd = false;
                                //    rowStart = j;
                                //    //lB1.Items.Add("Trip Start "+ dt.Rows[j]["gps_time"]);
                                //}
                                //err = "2001";
                                //if ((j<dt.Rows.Count) && ((int)dt.Rows[j]["gps_speed"] == 0) && ((int)dt.Rows[j + 1]["gps_speed"] > 0))// กดmeter แล้วยังไม่ออกรถทันที       //-15
                                //{
                                //    stripStart = true;
                                //    stripEnd = false;
                                //    rowStart = j;
                                //}
                                stripStart = true;      //  +15
                                stripEnd = false;      //  +15
                                rowStart = j;      //  +15
                            }
                            err = "2020";
                            if ((j >= 0)&&(j <= 20)){       //+13
                                if (((Boolean)dt.Rows[j]["gps_input1"] == true) && ((int)dt.Rows[j]["gps_speed"] == 0))
                                {
                                    gpsSpeed0Input1ON++;
                                }
                            }
                            err = "3000";
                            if ((j!=0) && ((Boolean)dt.Rows[j]["gps_input1"] == false) && ((Boolean)dt.Rows[j - 1]["gps_input1"] == true) && stripStart)//trip end จะคำนวรหา end trip ได้ต้อง stripStart = true ก่อน
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
                                if ((int)dt.Rows[j - 1]["gps_speed"] == 0)// รถจอด เหมือนกัน    11+
                                {
                                    stripEnd = true;
                                    insertTrip = true;
                                }
                                if ((int)dt.Rows[j]["gps_speed"] <= txtGPSError.Value)// รถจอด แต่ gps ส่งข้อมูลเป็น นาที ทำให้อาจ จอดปุ้บ แล้วรับคนใหม่ ทันที
                                {
                                    stripEnd = true;
                                    insertTrip = true;
                                    //lB1.Items.Add("Trip End " + dt.Rows[j]["gps_time"]);
                                }
                            }
                            /**
                             * noInsert
                             * 
                             */
                            //if (gpsSpeed0Input1ON >= txtGPSErrorNoInsert.Value)   //+13
                            //{
                            //    insertTrip = false;
                            //}
                            /*
                             *  check insertTrip
                             */
                            //int c = 0;
                            //for(int m = 0; m < noInsertMax; m++)   //+13
                            //{
                            //    if (noInsert[m] == false)
                            //    {
                            //        c++;
                            //    }
                            //}
                            err = "4000";
                            if (stripStart && stripEnd && insertTrip)
                            {
                                distance = 0.0;
                                incomeTrip = 0;

                                for (int k = rowStart; k < j; k++)//คำนวณหา ระยะทาง ระหว่างtrip
                                {
                                    if (k!=0)
                                    {
                                        distance += tdsC.sql.CalcDistanceKilo(Convert.ToDouble(dt.Rows[k - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[k]["gps_lon"]) / 1000000);
                                    }
                                }
                                if (tripBeforeMidnight) //+12 รับ trip มาก่อนเที่ยงคืน
                                {
                                    distance += distanceTripBeforeMidnight;
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
                                    if (tripBeforeMidnight) //+12 รับ trip มาก่อนเที่ยงคืน
                                    {
                                        dtS.Clear();// +1.
                                        dtS.Append(sBeforeMidnight);
                                        sql1.Append("Insert Into taxi_meter(t_imei, t_start_time, t_start_gps_lat, t_start_gps_lon")
                                        .Append(", t_off_time, t_off_gps_lat, t_off_gps_lon, t_distance, t_taxi_fare, customer_id, daily_date ) ")
                                        .Append("Values('").Append(dtCar.Rows[i]["imei"].ToString()).Append("','").Append(dtS.ToString()).Append("','").Append(gpsLatBeforeMidnight).Append("','").Append(gpsLonBeforeMidnight).Append("'")
                                        .Append(",'").Append(dtE.ToString()).Append("','").Append(dt.Rows[j]["gps_lat"].ToString()).Append("','")
                                        .Append(dt.Rows[j]["gps_lon"].ToString()).Append("',").Append(Math.Round(distance, 2)).Append(",")
                                        .Append(incomeTrip).Append(",'").Append(dtCar.Rows[i]["customer_id"].ToString()).Append("','").Append(dateStart).Append("') ");       // +1.
                                    }
                                    else
                                    {
                                        sql1.Append("Insert Into taxi_meter(t_imei, t_start_time, t_start_gps_lat, t_start_gps_lon")
                                        .Append(", t_off_time, t_off_gps_lat, t_off_gps_lon, t_distance, t_taxi_fare, customer_id, daily_date ) ")
                                        .Append("Values('").Append(dtCar.Rows[i]["imei"].ToString()).Append("','").Append(dtS.ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lat"].ToString()).Append("','").Append(dt.Rows[rowStart]["gps_lon"].ToString()).Append("'")
                                        .Append(",'").Append(dtE.ToString()).Append("','").Append(dt.Rows[j]["gps_lat"].ToString()).Append("','")
                                        .Append(dt.Rows[j]["gps_lon"].ToString()).Append("',").Append(Math.Round(distance, 2)).Append(",")
                                        .Append(incomeTrip).Append(",'").Append(dtCar.Rows[i]["customer_id"].ToString()).Append("','").Append(dateStart).Append("') ");       // +1.
                                    }
                                    comDaily.CommandText = sql1.ToString();
                                    //MessageBox.Show("sql  " + sql1.ToString());
                                    comDaily.ExecuteNonQuery();
                                    stripStart = false;
                                    gpsSpeed0Input1ON = 0;
                                    tripBeforeMidnight = false;
                                    //throw ();
                                }
                                catch (Exception ex)
                                {
                                    lB1.Items.Add("Insert Into taxi_meter err line [" + err+"]" + ex.Message.ToString());
                                }
                                finally
                                {

                                }
                                lB1.Items.Add(" imei " + dt.Rows[rowStart]["imei"] + " Trip Start " + dt.Rows[rowStart]["gps_time"]
                                    + " Trip End " + dt.Rows[j]["gps_time"] + " ระยะทาง " + distance + " ที่อยู่ " + addr.ToString());
                                insertTrip = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Error "+err+" row "+i+"\n" + ex.Message.ToString());
                            lB1.Items.Add("for (int j = 0; j < dt.Rows.Count; j++) err line [" + err + "]" + ex.Message.ToString());
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
                    //sql1.Append("Insert Into car_daily(car_daily_id, car_id, imei, daily_date, distance, income, trip_cnt, trip_distance, time_start, time_end, time_schedule, bck_server_id, car_Sql, car_cnt, customer_id, date_start) ")  -10
                    //.Append("Values(UUID()").Append(",'").Append(dtCar.Rows[i]["car_id"].ToString()).Append("','").Append(dtCar.Rows[i]["imei"].ToString())
                    //.Append("','").Append(dateStart).Append("','").Append(Math.Round(distanceDay, 2))
                    //.Append("',").Append(incomeTripSum).Append(",'").Append(TripCnt).Append("','").Append(Math.Round(distanceTripSum, 2))
                    //.Append("','").Append(timeStart).Append("','").Append(tdsC.setTimeCurrent()).Append("','").Append(txtAutoStart.Text)
                    //.Append("','").Append(connBck).Append("','").Append(sqlTrip.ToString().Replace("'","''")).Append("','").Append(dt.Rows.Count.ToString()).Append("','").Append(dtCar.Rows[i]["customer_id"].ToString()).Append("', now())");//connBck
                    //ceilIncome = Math.Ceiling(Double.Parse(incomeTripSum.ToString()) / 100) * 100;
                    ceilIncome = 0;
                    sql1.Append("Insert Into car_daily(car_daily_id, car_id, imei, daily_date, distance, income, trip_cnt, trip_distance, time_start, time_end, time_schedule, bck_server_id, car_Sql, car_cnt, customer_id, ceil_income, date_start) ")     //+10
                    .Append("Values(UUID()").Append(",'").Append(dtCar.Rows[i]["car_id"].ToString()).Append("','").Append(dtCar.Rows[i]["imei"].ToString())
                    .Append("','").Append(dateStart).Append("','").Append(Math.Round(distanceDay, 2))
                    .Append("',").Append(incomeTripSum).Append(",'").Append(TripCnt).Append("','").Append(Math.Round(distanceTripSum, 2))
                    .Append("','").Append(timeStart).Append("','").Append(tdsC.setTimeCurrent()).Append("','").Append(txtAutoStart.Text)
                    .Append("','").Append(connBck).Append("','").Append(sqlTrip.ToString().Replace("'", "''"))
                    .Append("','").Append(dt.Rows.Count.ToString()).Append("','").Append(dtCar.Rows[i]["customer_id"].ToString()).Append("',").Append(ceilIncome).Append(", now())");
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
                    + " รายได้ " + incomeTripSum + " ระยะทางรับผู้โดยสาร " + distanceTripSum + " distanceDay " + distanceDay);
                //lB1.Refresh();
                pB1.Value = i;
                conn01.Close();       //-2
                this.Refresh();

            }
            connDaily.Close();
            //for(int i = 0; i < 100; i++)//  +2
            //{
            //    if ((conn01[i] != null) && (conn01[i].State == ConnectionState.Open))       //  +2
            //    {
            //        conn01[connBck].Close();
            //    }
            //}
            pB1.Visible = false;
        }
        private void updateCustomerID()
        {
            /*
             *60-08-06 แก้โปรแกรม ให้update customer_id ใน table car_daily, taxi_meter
             */
            MySqlConnection connDaily = new MySqlConnection();
            MySqlCommand comDaily = new MySqlCommand();
            DataTable dtCar = new DataTable();
            DataTable dt = new DataTable();
            String sql = "";
            pB1.Show();
            pB1.Visible = true;
            pB1.Minimum = 0;
            //MySqlDataAdapter adapDaily = new MySqlDataAdapter(comDaily);
            //connDaily.ConnectionString = txtConnDaily.Text;
            try
            {
                connDaily.ConnectionString = txtConnDaily.Text;
                connDaily.Open();
                comDaily.Connection = connDaily;
                dtCar = tdsC.selectCarAll(txtConGPSOnLIne.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message.ToString());
                return;
            }

            pB1.Maximum = dtCar.Rows.Count;
            for (int i = 0; i < dtCar.Rows.Count; i++)// มีรถกี่คัน
            {
                sql = "Update car_daily Set customer_id = '" + dtCar.Rows[i]["customer_id"].ToString() + "' Where imei = '" + dtCar.Rows[i]["imei"].ToString() + "'";
                comDaily.CommandText = sql;
                comDaily.ExecuteNonQuery();

                sql = "Update taxi_meter Set customer_id = '" + dtCar.Rows[i]["customer_id"].ToString() + "' Where t_imei = " + dtCar.Rows[i]["imei"].ToString();
                comDaily.CommandText = sql;
                comDaily.ExecuteNonQuery();
                pB1.Value = i;
            }
            connDaily.Close();
            pB1.Visible = false;
        }
        private void updateCarID()
        {
            /*
             *60-08-06 แก้โปรแกรม ให้update car_id ใน table car_daily 06,07,08  salee แจ้ง 60-08-08
             * 
             */
            pB1.Visible = true;
            MySqlConnection connDaily = new MySqlConnection();
            MySqlCommand comDaily = new MySqlCommand();
            MySqlDataAdapter adap01 = new MySqlDataAdapter(comDaily);
            DataTable dt = new DataTable();
            DataTable dtCar = new DataTable();
            String sql = "";
            String imei = "";
            lB1.Items.Add("updateCarID");
            try
            {
                connDaily.ConnectionString = txtConnDaily.Text;
                connDaily.Open();
                comDaily.Connection = connDaily;
                sql = "select imei from car_daily where daily_date in ('2017-08-06', '2017-08-07','2017-08-08') group by imei";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message.ToString());
                return;
            }
            lB1.Items.Add("ConnectionString");
            dtCar = tdsC.selectCarAll(txtConGPSOnLIne.Text);
            lB1.Items.Add("selectCarAll");
            comDaily.CommandText = sql;
            adap01 = new MySqlDataAdapter(comDaily);
            adap01.Fill(dt);
            pB1.Maximum = dt.Rows.Count;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                imei = dt.Rows[i]["imei"].ToString();
                for (int j = 0; j < dtCar.Rows.Count; j++)
                {
                    if (imei.Equals(dtCar.Rows[j]["imei"].ToString()))
                    {
                        sql = "Update car_daily Set car_id = '" + dtCar.Rows[j]["car_id"].ToString() + "' Where imei =" + imei + "  and daily_date in ('2017-08-06', '2017-08-07','2017-08-08')";
                        comDaily.CommandText = sql;
                        comDaily.ExecuteNonQuery();
                        break;
                    }
                }
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

        private void btnCheckData_Click(object sender, EventArgs e)
        {
            //updateCustomerID();     //5.
            updateCarID();          //9.    60-08-09
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MySqlConnection connDaily = new MySqlConnection();
            connDaily.ConnectionString = txtConnDaily.Text;
            try
            {
                connDaily.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message.ToString());
                return;
            }
            MessageBox.Show("Connection OK ");
            connDaily.Close();
        }
    }
}
