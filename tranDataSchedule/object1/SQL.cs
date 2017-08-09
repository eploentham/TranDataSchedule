using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tranDataSchedule.object1
{
    public class SQL
    {
        public static string RBT_Select;
        public static MySqlTransaction st;
        //public static string connectionString = WebConfigurationManager.ConnectionStrings["gpsonlineV2ConnectionString"].ToString();
        public static string connectionString = "";
        const double _P = 35.0, _B = 50.0;
        // public static string connectionString2 = "server= 122.155.165.29&122.155.165.28 ;Database = gpsonline; Uid = root ;Pwd = password; pooling = false; Allow Zero Datetime= true";
        public static string Encrypt(string strToEncrypt, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = ASCIIEncoding.UTF8.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch
            {
                return null;
            }
        }
        public static string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff, byteDecrypted;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                byteDecrypted = objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);
                objDESCrypto = null;
                string strDecrypted = ASCIIEncoding.UTF8.GetString(byteDecrypted, 0, byteDecrypted.Length);

                return strDecrypted;
            }
            catch
            {
                return null;
            }
        }
        public static DataTable ConnectDataAdapter(string strMysql)
        {
            try
            {
                using (MySqlConnection Con = new MySqlConnection(connectionString))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(strMysql, Con))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            Con.Close();
                            Con.Dispose();
                            return dt;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        public static void ConnectCommand(string strMysq)
        {

            using (MySqlConnection Conn = new MySqlConnection(connectionString))
            {
                Conn.Open();

                using (MySqlCommand com = new MySqlCommand(strMysq, Conn))
                {
                    com.CommandType = CommandType.Text;
                    com.ExecuteNonQuery();
                    Conn.Close();
                    Conn.Dispose();
                }
            }
        }
        public static string CheckUsernamePassword(string username, string password)//check username password
        {
            try
            {
                string user = Encrypt(username, password);
                string pass = Encrypt(password, username);
                string currentName;
                using (MySqlConnection Conn = new MySqlConnection(connectionString))
                {
                    Conn.Open();
                    string sql = "SELECT customer.username, customer.password FROM gpsonline.customer ";
                    sql += " WHERE (customer.username = @UserName) AND (customer.password = @Pasword)";
                    using (MySqlCommand com = new MySqlCommand(sql, Conn))
                    {
                        com.Parameters.AddWithValue("@UserName", user);
                        com.Parameters.AddWithValue("@Pasword", pass);
                        currentName = (string)com.ExecuteScalar();
                        Conn.Close();
                        return currentName;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static string CheckAlert(string username)//check username password
        {
            try
            {
                string sql = "SELECT flag from alert";
                sql += " WHERE (alert.username = '" + username + "')";
                DataTable dt = new DataTable();
                dt = ConnectDataAdapter(sql);

                if (dt.Rows.Count != 0)
                {
                    if (dt.Rows[0]["flag"].ToString() == "Y")
                    {
                        return ("Y");
                    }
                    else
                    {
                        return "N";
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch//Exception sqlError)        
            {
                //MessageBox.Show(sqlError.ToString());
                return "F";
            }
        }


        public static DataTable CarIdAndGpsId(string username, string password)//select ข้อมูลรถใช้หน้า Command.aspx
        {
            string sqlstr = "SELECT car.car_id as 'ทะเบียน', car.gps_id as 'รหัส GPS', car.imei , typebox.datail_typebox as 'ประเภทกล่อง' FROM  gpsonline.customer ";
            sqlstr += " INNER JOIN gpsonline.car_to_show  ON(customer.customer_id = car_to_show.customer_id)INNER JOIN gpsonline.car  ON (car.imei = car_to_show.imei)";
            sqlstr += " INNER JOIN typebox ON (typebox.id_boxtype = car.id_typebox) WHERE customer.username = '" + Encrypt(username, password) + "' AND customer.password = '" + Encrypt(password, username) + "';";
            return ConnectDataAdapter(sqlstr);
        }
        public static DataTable CarSelect(string username, string password)// ดึงข้อมูลรถทั้งหมดเพื่อแสดงผลในหน้า CarvalityGoogle.apsx
        {
            DataTable dtServer = new DataTable();
            string user = Encrypt(username, password);
            string pass = Encrypt(password, username);
            using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
            {
                using (MySqlCommand com98 = new MySqlCommand("SelectCar", Conn98))//เรียกใช้ store procedure "SelectCar"
                {
                    com98.CommandType = CommandType.StoredProcedure;
                    com98.Parameters.Add("username", MySqlDbType.TinyText).Value = user;//ส่ง username
                    com98.Parameters.Add("passwordc", MySqlDbType.TinyText).Value = pass;//ส่ง password
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                    at98.Fill(dtServer);
                    Conn98.Close();

                }
            }
            return dtServer;
        }

        public static DataTable CarSelect2(string username, string password)// ดึงข้อมูลรถทั้งหมดเพื่อแสดงผลในหน้า CarvalityGoogle.apsx
        {
            DataTable dtServer = new DataTable();
            string user = Encrypt(username, password);
            string pass = Encrypt(password, username);
            using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
            {
                using (MySqlCommand com98 = new MySqlCommand("SelectCar2", Conn98))//เรียกใช้ store procedure "SelectCar2"
                {
                    com98.CommandType = CommandType.StoredProcedure;
                    com98.Parameters.Add("username", MySqlDbType.TinyText).Value = user;//ส่ง username
                    com98.Parameters.Add("passwordc", MySqlDbType.TinyText).Value = pass;//ส่ง password
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                    at98.Fill(dtServer);
                    Conn98.Close();
                }
            }
            return dtServer;
        }

        public static DataTable BackUpRMC(string startDate, string endDate, string imei)//select ข้อมูลย้อนหลัง
        {
            try
            {
                DataTable dtServer = new DataTable();
                DataTable dtbackup = new DataTable();
                using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(dtServer);
                        Conn98.Close();

                    }
                }
                //string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = " + dtServer.Rows[0]["user_id"].ToString() + ";Pwd = " + dtServer.Rows[0]["user_password"].ToString() + ";";
                //strBckconString += " pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = root" + ";Pwd = FGNEOB" + ";";
                strBckconString += " port = 6318; pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                using (MySqlConnection Conn101 = new MySqlConnection(strBckconString))
                {
                    using (MySqlCommand com101 = new MySqlCommand("PlayBlack", Conn101))//PlayBlack คือ StoredProcedure ที่ใช้ดึงข้อมูลย้อนหลัง
                    {
                        com101.CommandType = CommandType.StoredProcedure;
                        com101.Parameters.Add("startDate", MySqlDbType.TinyText).Value = DateTime.Parse(startDate).ToString("yyyy-MM-dd HH:mm:ss");//ส่งวันเวลาเริ่มต้น
                        com101.Parameters.Add("endDate", MySqlDbType.TinyText).Value = DateTime.Parse(endDate).ToString("yyyy-MM-dd HH:mm:ss");//ส่งวันเวลาสิ้นสุด
                        com101.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei
                        MySqlDataAdapter at101 = new MySqlDataAdapter(com101);
                        at101.Fill(dtbackup);
                        Conn101.Close();
                        return dtbackup;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        public static DataTable ReportSpeed(string stdateValue, string endateValue, string imei, string speed)//รายงานการใช้ความเร็ว(กราฟ)
                                                                                                              //   public static DataTable ReportSpeed(string dateValue, string imei, string speed)//รายงานการใช้ความเร็ว(กราฟ)
        {
            DataTable dt = new DataTable("reportSpeed");
            DataTable dtServer = new DataTable();
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(dtServer);
                        Conn98.Close();
                    }
                }
                //string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = " + dtServer.Rows[0]["user_id"].ToString() + ";Pwd = " + dtServer.Rows[0]["user_password"].ToString() + ";";
                //strBckconString += " pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = root" + ";Pwd = FGNEOB" + ";";
                strBckconString += " port = 6318; pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                if (speed == null)//ถ้าไม่แสดงรายงานความเร็วเกิน
                {
                    //         string sqlstr = "SELECT  DATE_FORMAT(gps_time,'%H:%i:%s') AS gps_time ,gps_speed FROM positionbackup WHERE (gps_date = '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ")  ORDER BY gps_date_time ASC;";
                    string sqlstr = "SELECT CONCAT(DATE_FORMAT(gps_date_time,'%m/%d-%H:'), LPAD(if(CEILING(MINUTE(gps_date_time)/5)=12,0,CEILING(MINUTE(gps_date_time)/5))*5,2,'0'))  AS gps_time ,gps_speed FROM positionbackup WHERE (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ")   GROUP BY gps_time ORDER BY gps_time ASC;";
                    using (MySqlConnection Con = new MySqlConnection(strBckconString))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                        {
                            da.Fill(dt);
                            Con.Close();
                            Con.Dispose();
                            return dt;
                        }
                    }
                }
                else//ถ้าแสดงรายงานความเร็วเกิน
                {
                    //                string sqlstr = "SELECT DATE_FORMAT(gps_time,'%H:%i:%s') AS gps_time ,gps_speed FROM positionbackup WHERE (gps_date = '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ") AND (gps_speed > " + speed + ") ORDER BY gps_date_time ASC;";
                    string sqlstr = "SELECT CONCAT(DATE_FORMAT(gps_date_time,'%m/%d-%H:'), LPAD(if(CEILING(MINUTE(gps_date_time)/5)=12,0,CEILING(MINUTE(gps_date_time)/5))*5,2,'0')) AS gps_time ,gps_speed FROM positionbackup WHERE (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ") AND (gps_speed > " + speed + ")  GROUP BY gps_time ORDER BY gps_time ASC;";
                    using (MySqlConnection Con = new MySqlConnection(strBckconString))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                        {
                            da.Fill(dt);
                            Con.Close();
                            Con.Dispose();
                            return dt;
                        }
                    }
                }
            }
            catch
            {
                return dt;
            }
        }

        public static DataTable ReportFuel(string stdateValue, string endateValue, string imei, string speed)//รายงานปริมาณน้ำมัน(กราฟ)
        {
            DataTable dt = new DataTable("reportFuel");
            DataTable dtServer = new DataTable();
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(dtServer);
                        Conn98.Close();
                    }
                }
                //string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = " + dtServer.Rows[0]["user_id"].ToString() + ";Pwd = " + dtServer.Rows[0]["user_password"].ToString() + ";";
                //strBckconString += " pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                string strBckconString = "server=" + dtServer.Rows[0]["server_ip"].ToString() + ";Database = " + dtServer.Rows[0]["db_name"].ToString() + "; Uid = root" + ";Pwd = FGNEOB" + ";";
                strBckconString += " port = 6318; pooling=true;Min Pool Size=" + dtServer.Rows[0]["min_pool"].ToString() + ";Max Pool Size=" + dtServer.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0; Allow Zero Datetime= true;";

                if (speed == null)//ถ้าไม่แสดงรายงานความเร็วเกิน
                {
                    // string sqlstr = "SELECT CONCAT(DATE_FORMAT(gps_date_time,'%m/%d-%H:'), LPAD(if(CEILING(MINUTE(gps_date_time)/5)=12,0,CEILING(MINUTE(gps_date_time)/5))*5,2,'0')) AS gps_date_hr , IF(ROUND((77.2-AVG(gps_input8))*100/51.6,2)>100,100,IF(ROUND((77.2-AVG(gps_input8))*100/51.6,2)<0,0,-20+ROUND((77.2-(SELECT SQRT(AVG(POW(p2.gps_input8,2))) FROM positionbackup AS p2 WHERE p2.gps_date_time <= p1.gps_date_time AND (p2.imei = " + imei + ") AND (p2.gps_input8>=17 AND p2.gps_input8 <=86) ORDER BY p2.gps_date_time DESC LIMIT 5))*100/51.6,2))) AS fuelpct FROM positionbackup AS p1 WHERE (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ")  AND (gps_input8>=17 AND gps_input8 <=86) GROUP BY gps_date_hr ORDER BY gps_date_hr ASC;";
                    string sqlstr = "SELECT DATE_FORMAT(gps_date_time,'%m/%d-%H') AS gps_date_hr , IF(ROUND(AVG((77.2-gps_input8)*100/51.6),2)>100,100,IF(ROUND(AVG((77.2-gps_input8)*100/51.6),2)<0,0,ROUND(AVG((77.2-gps_input8)*100/51.6),2))) AS fuelpct FROM positionbackup WHERE (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ")  AND (gps_input8>=17 AND gps_input8 <=86) GROUP BY gps_date_hr ORDER BY gps_date_hr ASC;";
                    using (MySqlConnection Con = new MySqlConnection(strBckconString))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                        {
                            da.Fill(dt);
                            Con.Close();
                            Con.Dispose();
                            return dt;
                        }
                    }
                }
                else//ถ้าแสดงรายงานความเร็วเกิน
                {
                    //  string sqlstr = "SELECT CONCAT(DATE_FORMAT(gps_date_time,'%m/%d-%H:'), LPAD(if(CEILING(MINUTE(gps_date_time)/5)=12,0,CEILING(MINUTE(gps_date_time)/5))*5,2,'0')) AS gps_date_hr , IF(ROUND((77.2-AVG(gps_input8))*100/51.6,2)>100,100,IF(ROUND((77.2-AVG(gps_input8))*100/51.6,2)<0,0,ROUND((77.2-AVG(gps_input8))*100/51.6,2))) AS fuelpct FROM positionbackup WHERE (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ") AND (gps_speed > " + speed + ") AND (gps_input8>=10 AND gps_input8 <=82) GROUP BY gps_date_hr ORDER BY gps_date_hr ASC;";
                    string sqlstr = "SELECT DATE_FORMAT(gps_date_time,'%m/%d-%H') AS gps_date_hr , IF(ROUND(AVG((77.2-gps_input8)*100/51.6),2)>100,100,IF(ROUND(AVG((77.2-gps_input8)*100/51.6),2)<0,0,ROUND(AVG((77.2-gps_input8)*100/51.6),2))) AS fuelpct FROM positionbackup WHERE  (gps_date >= '" + DateTime.Parse(stdateValue).ToString("yyyy-MM-dd") + "' AND gps_date <= '" + DateTime.Parse(endateValue).ToString("yyyy-MM-dd") + "' ) AND (imei = " + imei + ") AND (gps_speed > " + speed + ") AND (gps_input8>=17 AND gps_input8 <=86) GROUP BY gps_date_hr ORDER BY gps_date_hr ASC;";
                    using (MySqlConnection Con = new MySqlConnection(strBckconString))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                        {
                            da.Fill(dt);
                            Con.Close();
                            Con.Dispose();
                            return dt;
                        }
                    }
                }
            }
            catch
            {
                return dt;
            }
        }

        public static DataTable ReportAvgPerDate(string startDate, string endDate, string imei)//รายงานภาพรวมการใช้ยานพาหนะ
        {

            DataTable backupid = new DataTable();
            DataTable reportspeed = new DataTable();
            DataRow newRow;
            using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
            {
                using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                {
                    com98.CommandType = CommandType.StoredProcedure;
                    com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                    at98.Fill(backupid);
                    Conn98.Close();

                }
            }
            //string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
            //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
            //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
            //TimeSpan ts;

            string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
            connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
            connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
            TimeSpan ts;

            if (endDate == "")
            {
                ts = Convert.ToDateTime(startDate).Subtract(DateTime.Parse(startDate));
            }
            else
            {
                ts = Convert.ToDateTime(endDate).Subtract(DateTime.Parse(startDate));
            }

            reportspeed.Columns.Add("วันที่", typeof(string));
            reportspeed.Columns.Add("ระยะทางรวม", typeof(double));
            reportspeed.Columns.Add("ความเร็วสูงสุด", typeof(double));
            reportspeed.Columns.Add("ความเร็วเฉลี่ย", typeof(string));
            reportspeed.Columns.Add("ระยะเวลาติดเครื่อง", typeof(string));

            for (int i = 0; (i <= ts.Days) && (i <= 30); i++)//จำกัดวันในการดูรายงานไม่เกิน 30 วัน
            {
                using (MySqlConnection Conn101 = new MySqlConnection(connectionBackup))
                {
                    using (MySqlCommand com101 = new MySqlCommand("ReportAvgPerDate", Conn101))//ReportAvgPerDate คือ StoredProcedure ที่ใช้ค้นหา วันที่  ระยะทางรวม ความเร็วสูงสุด ความเร็วเฉลี่ย ระยะเวลาติดเครื่อง  ของรถ(วันเดียว)
                    {

                        try
                        {
                            com101.CommandType = CommandType.StoredProcedure;
                            com101.Parameters.Add("dateCurrent", MySqlDbType.VarChar).Value = Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd");//ส่งวันที่ให้ StoredProcedure ReportAvgPerDate
                            com101.Parameters.Add("boxImei", MySqlDbType.Int64).Value = imei;//ส่ง imei ให้ StoredProcedure ReportAvgPerDate

                            MySqlParameter dateCurrents = com101.Parameters.Add("dateCurrents", MySqlDbType.VarChar);//return วันที่ 
                            dateCurrents.Direction = ParameterDirection.Output;
                            MySqlParameter sumdistances = com101.Parameters.Add("sumdistances", MySqlDbType.Double);// return ระยะทางรวม
                            sumdistances.Direction = ParameterDirection.Output;
                            MySqlParameter maxspeeds = com101.Parameters.Add("maxspeeds", MySqlDbType.Double);//return ความเร็วสูงสุด 
                            maxspeeds.Direction = ParameterDirection.Output;
                            MySqlParameter avgspeeds = com101.Parameters.Add("avgspeeds", MySqlDbType.Double);//return ความเร็วเฉลี่ย
                            avgspeeds.Direction = ParameterDirection.Output;
                            MySqlParameter totalusetime = com101.Parameters.Add("totalusetime", MySqlDbType.VarChar);//return  ระยะเวลาติดเครื่อง
                            totalusetime.Direction = ParameterDirection.Output;
                            MySqlDataAdapter at101 = new MySqlDataAdapter(com101);

                            Conn101.Open();
                            com101.ExecuteNonQuery();
                            Conn101.Close();

                            newRow = reportspeed.NewRow();
                            newRow["วันที่"] = Convert.ToDateTime(dateCurrents.Value).ToString("dd-MM-yyyy");
                            newRow["ระยะทางรวม"] = sumdistances.Value;
                            newRow["ความเร็วสูงสุด"] = maxspeeds.Value;

                            newRow["ระยะเวลาติดเครื่อง"] = totalusetime.Value;
                            if (avgspeeds.Value == "0")
                            {
                                newRow["ความเร็วเฉลี่ย"] = string.Empty;
                            }
                            else
                            {
                                newRow["ความเร็วเฉลี่ย"] = avgspeeds.Value;
                            }
                            reportspeed.Rows.Add(newRow);
                        }
                        catch
                        {
                            newRow = reportspeed.NewRow();
                            newRow["วันที่"] = DateTime.Parse(startDate).AddDays(i).ToString("yyyy-MM-dd");
                            reportspeed.Rows.Add(newRow);
                        }
                    }
                }


            }

            return reportspeed;
        }

        public static DataTable ReportEnginUseTimePerDay(string dateValue, string imei)//รายงานการใช้งานยานพาหนะ
        {
            string timestart = "";
            DataTable backupid = new DataTable();
            DataTable dtsumtime = new DataTable();
            DataTable dt = new DataTable();
            DataRow row;
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(backupid);
                        Conn98.Close();

                    }
                }
                //string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
                //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
                //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
                connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                string sqlstr = "select DATE_FORMAT(gps_time,'%H:%i:%s') AS gps_time ,gps_ign from positionbackup where (gps_date = '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "') and (imei = " + imei + ") and (gps_ign = 1);";
                using (MySqlConnection Con = new MySqlConnection(connectionBackup))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                    {
                        da.Fill(dt);
                        Con.Close();
                        Con.Dispose();
                    }
                }
                dtsumtime.Columns.Add("เริ่มใช้", typeof(string));
                dtsumtime.Columns.Add("ดับเครื่อง", typeof(string));
                dtsumtime.Columns.Add("เวลารวม", typeof(string));
                int i;
                TimeSpan ts;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    if (Boolean.Parse(dt.Rows[i]["gps_ign"].ToString()))
                    {
                        if (i == 0)
                        {
                            timestart = dt.Rows[0]["gps_time"].ToString();
                        }
                        else
                        {
                            ts = DateTime.Parse(dt.Rows[i]["gps_time"].ToString()).Subtract(DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()));
                            if (ts.TotalMinutes >= 1.5)
                            {

                                row = dtsumtime.NewRow();
                                row["เริ่มใช้"] = timestart;
                                row["ดับเครื่อง"] = dt.Rows[i - 1]["gps_time"].ToString();
                                ts = DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()).Subtract(DateTime.Parse(timestart));
                                row["เวลารวม"] = ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
                                timestart = dt.Rows[i]["gps_time"].ToString();
                                dtsumtime.Rows.Add(row);

                            }

                        }
                    }
                }

                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }

        public static DataTable ReportDLT(string dateValue, string dateValue2, string imei)//รายงานการรูดบัตร
        {
            //string timestart = "";
            string sqlstr;
            DataTable backupid = new DataTable();
            DataTable dtsumtime = new DataTable();
            DataTable dt = new DataTable();
            DataRow row;
            string RRW = "N";
            try
            {
                RRW = "E1";
                string connectionDLT = "Server=localhost" + ";Port = 6318;";
                connectionDLT += " Database =  gpsonline" + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                connectionDLT += "Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                //string sqlstr = "select gps_date_time AS gps_time ,license as License,gps_speed as speed from cartodlt where (gps_date_time = '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "') and (car_id = " + imei + ") and (gps_ign = 1);";
                //sqlstr = "select car_id,gps_date_time,license,gps_speed from cartodlt where (imei = '" + imei + "') and (gps_ign = '1');";
                //sqlstr = "select car_id,gps_date_time,license,gps_speed,gps_lat,gps_lon,login_status from cartodlt where (LEFT(gps_date_time,10) between '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "' and '" + DateTime.Parse(dateValue2).ToString("yyyy-MM-dd") + "') and (imei = '" + imei + "') and (gps_ign = '1');";
                ///sqlstr = "select car_id,gps_date_time,substring(trim(substring(license,3+locate(' 1 ',license),26)),6,9) as license,gps_speed,gps_lat,gps_lon,login_status from cartodlt where (LEFT(gps_date_time,10) between '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "' and '" + DateTime.Parse(dateValue2).ToString("yyyy-MM-dd") + "') and (imei = '" + imei + "') and (gps_ign = '1');";
                sqlstr = "select car_id,gps_date_time,substring(trim(substring(license,3+locate('?+ ',license))),1,locate('            ',license,32)) as license,gps_speed,gps_lat,gps_lon,login_status from cartodlt where (LEFT(gps_date_time,10) between '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "' and '" + DateTime.Parse(dateValue2).ToString("yyyy-MM-dd") + "') and (imei = '" + imei + "') and (gps_ign = '1');";
                using (MySqlConnection Con = new MySqlConnection(connectionDLT))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                    {
                        da.Fill(dt);
                        Con.Close();
                        Con.Dispose();
                    }
                }
                RRW = "E2";
                dtsumtime.Columns.Add("ทะเบียน", typeof(string));
                dtsumtime.Columns.Add("วันที่ เวลา", typeof(string));
                dtsumtime.Columns.Add("สถานะบัตร", typeof(string));
                dtsumtime.Columns.Add("หมายเลขบัตร", typeof(string));
                dtsumtime.Columns.Add("ผู้ขับ", typeof(string));
                //dtsumtime.Columns.Add("สถานที่", typeof(string));
                dtsumtime.Columns.Add("Lat", typeof(string));
                dtsumtime.Columns.Add("Lon", typeof(string));
                dtsumtime.Columns.Add("ตำแหน่งรถ", typeof(string));
                int i;
                string lid;
                string drvname;
                string drvx = "";
                string drvx2 = "";
                string latx;
                string lonx;
                Boolean ddup = false;
                string LastLogin = "0";
                //TimeSpan ts;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    // ------------ check last login status 
                    if (LastLogin == dt.Rows[i]["login_status"].ToString())
                    {
                        ddup = true;
                    }
                    else
                    {
                        ddup = false;
                    }

                    //--------------------------------------
                    string loginStr = "";
                    if (ddup == false)
                    {
                        if (dt.Rows[i]["login_status"].ToString() == "0")
                        {
                            loginStr = "No card";
                        }
                        if (dt.Rows[i]["login_status"].ToString() == "1")
                        {
                            loginStr = "Login";
                        }
                        if (dt.Rows[i]["login_status"].ToString() == "2")
                        {
                            loginStr = "Logout";
                        }

                        RRW = "E3";
                        row = dtsumtime.NewRow();
                        //row["ทะเบียน"] = dt.Rows[i]["car_id"].ToString();
                        row["ทะเบียน"] = dt.Rows[i]["car_id"].ToString();
                        row["วันที่ เวลา"] = dt.Rows[i]["gps_date_time"].ToString();// dt.Rows[i - 1]["car_id"]; //timestart;
                        if ((dt.Rows[i]["license"].ToString().Trim() == "") || (dt.Rows[i]["license"].ToString().Length <= 0))
                        {
                            row["สถานะบัตร"] = loginStr;// "ไม่รูดบัตร";
                            lid = "";
                            drvname = "";
                        }
                        else
                        {
                            row["สถานะบัตร"] = loginStr;// "รูดบัตร";// dt.Rows[i]["license"].ToString();
                                                        //if (dt.Rows[i]["license"].ToString().Length > 70)
                            if (dt.Rows[i]["license"].ToString().Length > 0)
                            {
                                int pos;
                                int pos2;
                                pos = 0;
                                pos = dt.Rows[i]["license"].ToString().IndexOf("?");
                                if (pos <= 0)
                                {
                                    drvname = "";
                                }
                                else
                                {
                                    //string nn;                       
                                    drvname = dt.Rows[i]["license"].ToString();
                                    //drvname = drvname.Substring(5, pos - 7);

                                    pos = 0;
                                    pos = drvname.ToString().IndexOf("$", pos);
                                    if (pos > 0)
                                    {
                                        drvx = drvname.Substring(0, pos);
                                        //drvname = drvx;

                                        //pos2 = pos + 1;
                                        pos2 = drvname.ToString().IndexOf("$", pos + 1);
                                        if (pos2 > 0)
                                        {
                                            drvx2 = drvname.Substring(pos + 1, (pos2 - pos) - 1);
                                            drvname = drvname.Substring(pos2 + 1, 3) + " " + drvx2 + " " + drvx;
                                        }
                                    }
                                }

                                pos = 0;
                                pos = dt.Rows[i]["license"].ToString().IndexOf("+");
                                if (pos <= 0)
                                {
                                    lid = "";
                                }
                                else
                                {
                                    string ll;
                                    lid = dt.Rows[i]["license"].ToString();
                                    ll = lid.Substring(pos + 2, 3);
                                    if (ll.Substring(2, 1) == " ")
                                    {
                                        lid = lid.Substring(pos + 7, 7);// +":" + pos.ToString();
                                    }
                                    else
                                    {
                                        lid = lid.Substring(pos + 9, 8);// +":" + pos.ToString();
                                    }
                                    //lid = ll;
                                }
                            }
                            else
                            {
                                lid = "";
                                drvname = "";
                            }
                        }
                        //row["License"] = dt.Rows[i - 1]["license"].ToString();
                        ////ts = DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()).AddMinutes(1).Subtract(DateTime.Parse(timestart));
                        ////row["เวลารวม"] = ts.Hours + ":" + ts.Minutes;
                        ////timestart = dt.Rows[i]["gps_time"].ToString();

                        latx = dt.Rows[i]["gps_lat"].ToString();
                        lonx = dt.Rows[i]["gps_lon"].ToString();
                        latx = latx.Substring(0, 2) + "." + latx.Substring(2, 6);
                        lonx = lonx.Substring(0, 3) + "." + lonx.Substring(3, 6);
                        row["หมายเลขบัตร"] = dt.Rows[i]["license"].ToString(); //lid;
                        row["ผู้ขับ"] = drvname;// dt.Rows[i]["license"].ToString().Substring(25, 7);
                        row["Lat"] = latx;// dt.Rows[i]["gps_lat"].ToString();// "ตำแหน่งเริ่ม";
                        row["Lon"] = lonx;// dt.Rows[i]["gps_lon"].ToString();
                        row["ตำแหน่งรถ"] = "";
                        dtsumtime.Rows.Add(row);
                    }// ddup = false
                }
                RRW = dt.Rows.Count.ToString();
                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }

        public static DataTable ReportInOutPlace(string dateStart, string dateEnd, string imei, string username, string password)//รายงานการเข้าออกสถานที่(ยังไม่ดี)
        {
            string timestart = "", namepositon = "", position = "";
            DataTable backupid = new DataTable();
            DataTable dt = new DataTable();
            DataTable dtInOut = new DataTable();
            DataRow row;
            using (MySqlConnection Conn98 = new MySqlConnection(SQL.connectionString))
            {
                using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                {
                    com98.CommandType = CommandType.StoredProcedure;
                    com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                    at98.Fill(backupid);
                    Conn98.Close();

                }
            }
            //string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
            //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
            //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

            string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
            connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
            connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";


            DataTable dtPosition = SQL.ConnectDataAdapter("SELECT polygon.postition,polygon.namepolygon FROM gpsonline.customer INNER JOIN gpsonline.polygon ON (customer.customer_id = polygon.customer_id) WHERE customer.username = '" + Encrypt(username, password) + "' AND customer.password = '" + Encrypt(password, username) + "';");
            string sqlstr = "select DATE_FORMAT(gps_date_time ,'%d-%m-%Y %H:%i:%s') as gps_date_time,gps_lat,gps_lon from positionbackup where ( gps_date_time BETWEEN '" + DateTime.Parse(dateStart).ToString("yyyy-MM-dd") + " 00:00:00' and '" + DateTime.Parse(dateEnd).ToString("yyyy-MM-dd") + " 23:59:00' ) and (imei = " + imei + ");";

            using (MySqlConnection Con = new MySqlConnection(connectionBackup))
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                {
                    da.Fill(dt);
                    Con.Close();
                    Con.Dispose();
                }
            }
            dtInOut.Columns.Add("เวลาเข้า", typeof(string));
            dtInOut.Columns.Add("เวลาออก", typeof(string));
            dtInOut.Columns.Add("สถานที่", typeof(string));
            dtInOut.Columns.Add("เวลาในสถานที่(ชั่วโมง:นาที)", typeof(string));
            for (int i = 0; i < dtPosition.Rows.Count; i++)
            {
                namepositon = dtPosition.Rows[i]["namepolygon"].ToString();
                position = dtPosition.Rows[i]["postition"].ToString();
                timestart = "";

                for (int j = 0; j < dt.Rows.Count; j++)
                {

                    if (SQL.containsLatLng(position, double.Parse(dt.Rows[j]["gps_lat"].ToString()) / 1000000, double.Parse(dt.Rows[j]["gps_lon"].ToString()) / 1000000) == false)
                    {

                        if (timestart == "")
                        {
                            timestart = dt.Rows[j]["gps_date_time"].ToString();
                        }
                        else
                        {
                            TimeSpan ts = DateTime.Parse(dt.Rows[j]["gps_date_time"].ToString()).Subtract(DateTime.Parse(timestart));
                            if (ts.TotalMinutes > 2)
                            {
                                if (SQL.containsLatLng(position, double.Parse(dt.Rows[j - 1]["gps_lat"].ToString()) / 1000000, double.Parse(dt.Rows[j - 1]["gps_lon"].ToString()) / 1000000) == true)
                                {
                                    row = dtInOut.NewRow();
                                    row["เวลาเข้า"] = DateTime.Parse(timestart).AddMinutes(1);
                                    row["เวลาออก"] = DateTime.Parse(dt.Rows[j]["gps_date_time"].ToString()).AddMinutes(-1);
                                    row["สถานที่"] = namepositon;
                                    ts = DateTime.Parse(dt.Rows[j]["gps_date_time"].ToString()).AddMinutes(-1).Subtract(DateTime.Parse(timestart).AddMinutes(1));
                                    row["เวลาในสถานที่(ชั่วโมง:นาที)"] = ts.Hours + ":" + ts.Minutes;
                                    dtInOut.Rows.Add(row);
                                }
                            }
                            timestart = dt.Rows[j]["gps_date_time"].ToString();
                        }


                    }


                }
            }
            return dtInOut;
        }
        public static bool containsLatLng(string position, double lat, double lng)//การเข้าออกพื้นที่
        {
            string[] positionArr = position.Split('*');
            int numPoints = positionArr.Length;
            bool inPoly = false;
            int j = numPoints - 1;
            string[] latlonAll1 = null, latlonAll2 = null;
            double lat1 = 0, lng1 = 0, lat2 = 0, lng2 = 0;
            for (int i = 0; i < numPoints; i++)
            {
                latlonAll1 = positionArr[i].Split(',');//vertex1
                lat1 = Convert.ToDouble(latlonAll1[0]);
                lng1 = Convert.ToDouble(latlonAll1[1]);

                latlonAll2 = positionArr[j].Split(',');//vertex2
                lat2 = Convert.ToDouble(latlonAll2[0]);
                lng2 = Convert.ToDouble(latlonAll2[1]);
                // Response.Write("latlonAll1 " + lat1 + " , " + lon1 + " latlonAll2 " + lat2 + " , " + lon2 + "<br />");
                if (lng1 < lng && lng2 >= lng || lng2 < lng && lng1 >= lng)
                {

                    if (lat1 + (lng - lng1) / (lng2 - lng1) * (lat2 - lat1) < lat)
                    {
                        inPoly = !inPoly;
                    }
                }
                j = i;
            }
            return inPoly;

        }
        public static DataTable Car(string username, string password)//select ทะเบียนรถ ใช้ในหน้า PlayBackGoogle.aspx และ Playblack.aspx
        {
            string data = "";
            if (username == "demo")
            {
                data = "select imei, car_id from gpsonline.car where car_level_id = 11;";
            }
            else
            {
                data = " SELECT car.car_id , car.imei FROM gpsonline.customer INNER JOIN gpsonline.car_to_show ON (customer.customer_id = car_to_show.customer_id)";
                data += " INNER JOIN gpsonline.car ON (car_to_show.imei = car.imei) WHERE (customer.username = '" + Encrypt(username, password) + "') and (customer.password = '" + Encrypt(password, username) + "');";
            }
            return ConnectDataAdapter(data);
        }
        public static int UserLevel(string username, string password)//select customer level
        {
            DataTable ds = ConnectDataAdapter("select customer_level_id from gpsonline.customer where  (customer.username = '" + Encrypt(username, password) + "') and (customer.password = '" + Encrypt(password, username) + "');");
            if (ds.Rows.Count > 0)
            {

                return int.Parse(ds.Rows[0]["customer_level_id"].ToString());
            }
            return 99999;
        }

        public static int CarLevel(string username, string password, string carId)//select car level
        {
            string data = " SELECT car_level_id FROM gpsonline.car INNER JOIN gpsonline.customer ON (car.customer_id = customer.customer_id)";
            data += " where (customer.username = '" + Encrypt(username, password) + "') and (customer.password = '" + Encrypt(password, username) + "') and (car.car_id = '" + carId + "');";
            if (ConnectDataAdapter(data).Rows.Count > 0)
            {
                return int.Parse(ConnectDataAdapter(data).Rows[0]["car_level_id"].ToString());
            }
            return 99999;

        }
        public static void InsertLogin(string username, string password)////เพิ่ม username password ของลูกค้าลง database
        {
            DataTable ds = ConnectDataAdapter("select customer_id from gpsonline.customer where  (customer.username = '" + Encrypt(username, password) + "') and (customer.password = '" + Encrypt(password, username) + "');");
            string data = "insert into gpsonline.login_log(customer_id,logTime)values(" + ds.Rows[0]["customer_id"].ToString() + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")) + "');";
            ConnectCommand(data);

        }

        public static bool RenameCarId(string oldId, string newId)//เปลี่ยนทะเบียนรถ
        {
            try
            {

                //string connectionString2 = "Server= 122.155.165.29;Database = gpsonline; Uid = root ;Pwd = password; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                ////string connectionString1 = "Server= 122.155.165.28;Database = gpsonline; Uid = root ;Pwd = password;  Allow Zero Datetime= true;";
                //string connectionString1 = "Server= 122.155.165.28;Database = gpsonline; Uid = root ;Pwd = password; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                //string connectionString3 = "Server= 122.155.180.83;Database = gpsonline; Uid = root ;Pwd = password; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";

                ////string connectionString1 = "Server= 122.155.165.28;Port = 6318; Database = gpsonline; Uid = root ;Pwd = FGNEOB; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                ////string connectionString2 = "Server= 122.155.165.29;Port = 6318; Database = gpsonline; Uid = root ;Pwd = FGNEOB; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                string connectionString2 = "Server= localhost;Port = 6318; Database = gpsonline; Uid = root ;Pwd = FGNEOB; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                ////string connectionString3 = "Server= 122.155.180.83;Port = 6318; Database = gpsonline; Uid = root ;Pwd = FGNEOB; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";

                //string connectionString1 = "Server= 122.155.165.28;Port = 3306;Database = gpsonline; Uid = root ;Pwd = password; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                //string connectionString2 = "Server= 122.155.165.29;Port = 3306;Database = gpsonline; Uid = root ;Pwd = password; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";
                //string connectionString3 = "Server= 122.155.180.83;Port = 6318;Database = gpsonline; Uid = root ;Pwd = FGNEOB; pooling=true;Min Pool size =0;Max Pool Size =1; connection timeout = 300;default command timeout = 20; Allow Zero Datetime= true";


                string data = "update gpsonline.car set car_id = '" + newId + "' where car_id = '" + oldId + "' ;";
                using (MySqlConnection conn = new MySqlConnection(connectionString2))
                {
                    if (conn.State == ConnectionState.Open)
                    { conn.Close(); }


                    conn.Open();

                    //MessageBox.Show("เปลี่ยนทะเบียน 110.164.217.158 : " + oldId + " >> " + newId);
                    st = conn.BeginTransaction();
                    using (MySqlCommand com = new MySqlCommand(data, conn, st))
                    {
                        com.CommandType = CommandType.Text;
                        com.ExecuteNonQuery();
                        st.Commit();
                        conn.Close();
                        conn.Dispose();
                    }
                }

                ////System.Threading.Thread.Sleep(500);
                ////using (MySqlConnection conn2 = new MySqlConnection(connectionString1))
                ////{
                ////    if (conn2.State == ConnectionState.Open)
                ////    { conn2.Close(); }


                ////    conn2.Open();

                ////    st = conn2.BeginTransaction();
                ////    using (MySqlCommand com = new MySqlCommand(data, conn2, st))
                ////    {
                ////        com.CommandType = CommandType.Text;
                ////        com.ExecuteNonQuery();
                ////        st.Commit();
                ////        conn2.Close();
                ////        conn2.Dispose();
                ////    }
                ////}

                ////System.Threading.Thread.Sleep(500);
                ////using (MySqlConnection conn2 = new MySqlConnection(connectionString3))
                ////{
                ////    if (conn2.State == ConnectionState.Open)
                ////    { conn2.Close(); }


                ////    conn2.Open();

                ////    st = conn2.BeginTransaction();
                ////    using (MySqlCommand com = new MySqlCommand(data, conn2, st))
                ////    {
                ////        com.CommandType = CommandType.Text;
                ////        com.ExecuteNonQuery();
                ////        st.Commit();
                ////        conn2.Close();
                ////        conn2.Dispose();
                ////    }
                ////}


                return true;
            }

            catch (Exception sqlError)
            {
                MessageBox.Show(sqlError.ToString());
                st.Rollback(); return false;
            }

        }
        public static bool CheckCarId(string carId, string username, string password)//check carid ซ้ำ
        {
            string user = Encrypt(username, password);
            string pass = Encrypt(password, username);
            string data = " SELECT car.car_id FROM gpsonline.car INNER JOIN gpsonline.customer ON (car.customer_id = customer.customer_id)";
            data += " WHERE car.car_id = '" + carId + "' AND customer.username = '" + user + "' AND customer.password = '" + pass + "';";
            DataTable dt = new DataTable();
            dt = ConnectDataAdapter(data);
            if (dt.Rows.Count == 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;
        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)//คำนวนระยะทางจาก lat lon
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = EarthRadiusInMiles;
            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = EarthRadiusInKilometers; }
            return radius * 2 * 1.075 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
        public double CalcDistanceKilo(double lat1, double lng1, double lat2, double lng2)
        {
            double radius = EarthRadiusInMiles;
            radius = EarthRadiusInKilometers;
            return radius * 2 * 1.075 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }
        //คำนวณรายได้
        public static DataTable AvgOfDay1(string dateValue, string timeStart, string timeEnd, string imei, double gas)
        {
            TimeSpan ts;
            string timestart = "";
            int c = 0;
            double sumdis1 = 0;
            DataTable backupid = new DataTable();
            DataTable dtsumtime = new DataTable();
            DataTable dt = new DataTable();
            DataRow row;
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(SQL.connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(backupid);
                        Conn98.Close();

                    }
                }
                //string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
                //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
                //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                string connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
                connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                string sqlstr = "select DATE_FORMAT(gps_time,'%H:%i:%s') AS gps_time ,gps_input1,gps_lat,gps_lon from positionbackup where (gps_date = '" + DateTime.Parse(dateValue).ToString("yyyy-MM-dd") + "') and (imei = " + imei + ") and (gps_input1 = 1) and (gps_time between '" + timeStart + "' and '" + timeEnd + "');";
                using (MySqlConnection Con = new MySqlConnection(connectionBackup))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                    {
                        da.Fill(dt);
                        Con.Close();
                        Con.Dispose();
                    }
                }
                dtsumtime.Columns.Add("เริ่มรับ", typeof(string));
                dtsumtime.Columns.Add("ส่ง", typeof(string));
                dtsumtime.Columns.Add("ระยะทาง", typeof(double));
                dtsumtime.Columns.Add("ค่าโดยสาร", typeof(int));
                dtsumtime.Columns.Add("ค่าแก๊ส", typeof(int));


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Boolean.Parse(dt.Rows[i]["gps_input1"].ToString()))
                    {
                        if (i == 0)
                        {
                            timestart = dt.Rows[0]["gps_time"].ToString();

                        }
                        else
                        {
                            ts = DateTime.Parse(dt.Rows[i]["gps_time"].ToString()).Subtract(DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()));
                            if (ts.TotalMinutes >= 3)
                            {

                                for (int a = c + 1; a <= (i - 1); a++)
                                {
                                    sumdis1 += SQL.CalcDistance(Convert.ToDouble(dt.Rows[a - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lon"]) / 1000000, SQL.GeoCodeCalcMeasurement.Kilometers);
                                }
                                if (sumdis1 >= 0.5)
                                {
                                    row = dtsumtime.NewRow();
                                    row["เริ่มรับ"] = timestart;
                                    row["ส่ง"] = dt.Rows[i - 1]["gps_time"].ToString();
                                    row["ระยะทาง"] = Math.Round(sumdis1, 2);
                                    row["ค่าโดยสาร"] = PriceDay(Math.Round(sumdis1, 2));
                                    row["ค่าแก๊ส"] = Math.Round(sumdis1, 2) * gas;
                                    dtsumtime.Rows.Add(row);
                                    timestart = dt.Rows[i]["gps_time"].ToString();
                                    c = i;
                                    sumdis1 = 0;
                                }

                            }

                        }
                    }
                }

                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }

        public DataTable AvgOfDayPeriod(string startDate, string endDate, DataTable dtcarimei, double gas, int rent)//รายงานรายได้ของแท็กซี่เป็นวัน สรุปภาพรวมเป็นช่วง
        {

            double sumdis1 = 0;//ผลรวมระยะทาง
            double sumdis2 = 0;//ผลรวมระยะทางรับผู้โดยสาร
            int sumrevenue = 0;//ผลรวมรายได้
            int sumpassenger = 0;//ผลรวมจน.ผู้โดยสาร
            int sumrent = 0;//ผลรวมค่าเช่า
            string connectionBackup = "";
            DataTable backupid = new DataTable();//เก็บค่าข้อมูล database backup
            DataTable dtsumtime = new DataTable();//เก็บค่ารายงานรายได้ของแท็กซี่เป็นวัน
            string[] arr;//เก็บค่า จำนวนแถวทั้งหมด ราคารวม ระยะทางรวมของแต่ละวัน
            DataRow row;
            try
            {

                for (int k = 0; k < dtcarimei.Rows.Count; k++)
                {
                    using (MySqlConnection Conn98 = new MySqlConnection(SQL.connectionString))
                    {
                        using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database backup ของรถ
                        {
                            com98.CommandType = CommandType.StoredProcedure;
                            com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = dtcarimei.Rows[k]["imei"].ToString();//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                                                                                                                            //com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = dtcarimei.Rows[3]["imei"].ToString(); //54061557;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                            MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                            at98.Fill(backupid);
                            Conn98.Close();

                        }
                    }
                    //connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
                    //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
                    //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
                    //TimeSpan ts;

                    connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
                    connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                    connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
                    TimeSpan ts;

                    ts = Convert.ToDateTime(endDate).Subtract(DateTime.Parse(startDate));

                    dtsumtime.Columns.Add("วันที่", typeof(string));
                    dtsumtime.Columns.Add("ระยะทาง", typeof(double));
                    dtsumtime.Columns.Add("ระยะทางรับผู้โดยสาร", typeof(double));
                    dtsumtime.Columns.Add("จำนวนรับผู้โดยสาร", typeof(int));
                    dtsumtime.Columns.Add("รายได้", typeof(int));
                    dtsumtime.Columns.Add("ค่าเชื้อเพลิง", typeof(int));
                    dtsumtime.Columns.Add("เหลือ", typeof(int));

                    for (int i = 0; (i <= ts.Days) && (i <= 30); i++)
                    {

                        arr = SubAvgOfDay2(Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), dtcarimei.Rows[k]["imei"].ToString(), connectionBackup).Split(':');
                        sumdis1 += SumDistanceOfDate(Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), dtcarimei.Rows[k]["imei"].ToString(), connectionBackup);
                        //row = dtsumtime.NewRow();
                        //row["วันที่"] = Convert.ToDateTime(startDate).AddDays(i).ToString("dd-MM-yyyy");
                        //row["ระยะทาง"] = sumdis1;
                        sumrevenue += int.Parse(arr[1]);
                        sumpassenger += int.Parse(arr[0]);
                        //row["ค่าเชื้อเพลิง"] += sumdis1 * gas;
                        //row["เหลือ"] = int.Parse(arr[1]) - (sumdis1 * gas + rent);
                        sumrent += rent; //ผลรวมค่าเช่า
                        sumdis2 += double.Parse(arr[2]); //ระยะทางรับผู้โดยสาร
                                                         //dtsumtime.Rows.Add(row);

                    }
                    row = dtsumtime.NewRow();
                    row["วันที่"] = ""; //Convert.ToDateTime(startDate).ToString("dd-MM-yyyy");
                    row["ระยะทาง"] = sumdis1;
                    row["รายได้"] = sumrevenue;
                    row["จำนวนรับผู้โดยสาร"] = sumpassenger;
                    row["ค่าเชื้อเพลิง"] = sumdis1 * gas;
                    row["เหลือ"] = sumrevenue - (sumdis1 * gas + sumrent);
                    row["ระยะทางรับผู้โดยสาร"] = sumdis2;
                    dtsumtime.Rows.Add(row);
                }
                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }

        public DataTable AvgOfDay2(string startDate, string endDate, string imei, double gas, int rent)//รายงานรายได้ของแท็กซี่เป็นวัน
        {

            double sumdis1 = 0;//ผลรวมระยะทาง
            string connectionBackup = "";
            DataTable backupid = new DataTable();//เก็บค่าข้อมูล database backup
            DataTable dtsumtime = new DataTable();//เก็บค่ารายงานรายได้ของแท็กซี่เป็นวัน
            string[] arr;//เก็บค่า จำนวนแถวทั้งหมด ราคารวม ระยะทางรวมของแต่ละวัน
            DataRow row;
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(SQL.connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database backup ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(backupid);
                        Conn98.Close();

                    }
                }
                //connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
                //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
                //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
                //TimeSpan ts;

                connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
                connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";
                TimeSpan ts;

                ts = Convert.ToDateTime(endDate).Subtract(DateTime.Parse(startDate));

                dtsumtime.Columns.Add("วันที่", typeof(string));
                dtsumtime.Columns.Add("ระยะทาง", typeof(double));
                dtsumtime.Columns.Add("ระยะทางรับผู้โดยสาร", typeof(double));
                dtsumtime.Columns.Add("จำนวนรับผู้โดยสาร", typeof(int));
                dtsumtime.Columns.Add("รายได้", typeof(int));
                dtsumtime.Columns.Add("ค่าเชื้อเพลิง", typeof(int));
                dtsumtime.Columns.Add("เหลือ", typeof(int));

                for (int i = 0; (i <= ts.Days) && (i <= 30); i++)
                {

                    arr = SubAvgOfDay2(Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), imei, connectionBackup).Split(':');
                    sumdis1 = SumDistanceOfDate(Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), imei, connectionBackup);
                    row = dtsumtime.NewRow();
                    row["วันที่"] = Convert.ToDateTime(startDate).AddDays(i).ToString("dd-MM-yyyy");
                    row["ระยะทาง"] = sumdis1;
                    row["รายได้"] = int.Parse(arr[1]);
                    row["จำนวนรับผู้โดยสาร"] = int.Parse(arr[0]);
                    row["ค่าเชื้อเพลิง"] = sumdis1 * gas;
                    row["เหลือ"] = int.Parse(arr[1]) - (sumdis1 * gas + rent);
                    row["ระยะทางรับผู้โดยสาร"] = double.Parse(arr[2]);
                    dtsumtime.Rows.Add(row);

                }
                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }
        public static DataTable AvgOfDay3(string startDate, string endDate, string startTime, string endTime, string imei, double gas, int rent)//รายงานรายได้ของแท็กซี่เป็นกะ
        {
            TimeSpan ts;
            string connectionBackup = "";
            string[] arr;
            DataTable backupid = new DataTable();
            DataTable dtsumtime = new DataTable();
            DataTable dt = new DataTable();
            DataRow row;
            try
            {
                using (MySqlConnection Conn98 = new MySqlConnection(SQL.connectionString))
                {
                    using (MySqlCommand com98 = new MySqlCommand("SelectServerName", Conn98))//SelectServerName คือ StoredProcedure ที่ใช้ค้นหา  database back up ของรถ
                    {
                        com98.CommandType = CommandType.StoredProcedure;
                        com98.Parameters.Add("imeibox", MySqlDbType.Int64).Value = imei;//ส่ง imei ไปยัง StoredProcedure ชื่อ SelectServerName
                        MySqlDataAdapter at98 = new MySqlDataAdapter(com98);
                        at98.Fill(backupid);
                        Conn98.Close();
                    }
                }
                //connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 3306;";
                //connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = " + backupid.Rows[0]["user_id"].ToString() + " ;Pwd = " + backupid.Rows[0]["user_password"].ToString() + ";";
                //connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";

                connectionBackup = " Server=" + backupid.Rows[0]["server_ip"].ToString() + ";Port = 6318;";
                connectionBackup += " Database =  " + backupid.Rows[0]["db_name"].ToString() + "; Uid = root" + " ;Pwd = FGNEOB" + ";";
                connectionBackup += " pooling=true;Min Pool size =" + backupid.Rows[0]["min_pool"].ToString() + ";Max Pool Size = " + backupid.Rows[0]["max_pool"].ToString() + ";Connection Timeout = 300;default command timeout=0;Allow Zero Datetime= true;";


                ts = Convert.ToDateTime(endDate).Subtract(DateTime.Parse(startDate));
                dtsumtime.Columns.Add("วันที่", typeof(string));
                dtsumtime.Columns.Add("ระยะทาง", typeof(double));
                dtsumtime.Columns.Add("ระยะทางรับผู้โดยสาร", typeof(double));
                dtsumtime.Columns.Add("จำนวนรับผู้โดยสาร", typeof(int));
                dtsumtime.Columns.Add("รายได้", typeof(int));
                dtsumtime.Columns.Add("ค่าเชื้อเพลิง", typeof(int));
                dtsumtime.Columns.Add("เหลือ", typeof(int));

                for (int i = 0; (i <= ts.Days) && (i <= 30); i++)
                {

                    arr = SubAvgOfDay3(Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd"), startTime, endTime, imei, connectionBackup).Split(':');
                    row = dtsumtime.NewRow();
                    row["วันที่"] = Convert.ToDateTime(startDate).AddDays(i).ToString("yyyy-MM-dd");
                    row["ระยะทาง"] = double.Parse(arr[1]);
                    row["จำนวนรับผู้โดยสาร"] = int.Parse(arr[0]);
                    row["รายได้"] = int.Parse(arr[2]);
                    row["ค่าเชื้อเพลิง"] = double.Parse(arr[1]) * gas;
                    row["เหลือ"] = int.Parse(arr[2]) - ((double.Parse(arr[1]) * gas) + rent);
                    row["ระยะทางรับผู้โดยสาร"] = double.Parse(arr[3]);
                    dtsumtime.Rows.Add(row);
                }

                return dtsumtime;
            }
            catch
            {
                return dtsumtime;
            }
        }
        private static string SubAvgOfDay3(string startDate, string endDate, string startTime, string endTime, string imei, string connection)//เป็นกะ
        {
            DateTime st, et;
            double sumdis1 = 0, resultDistance = 0, actualdistance = 0, sumPrice1 = 0, price = 0, speed = 0;
            TimeSpan ts;
            string timestart = "", newEndDate = "";
            int c = 0, count = 0, time = 0;
            DataTable dt = new DataTable();
            st = DateTime.Parse(startTime);
            et = DateTime.Parse(endTime);
            if (st > et)
            {
                newEndDate = Convert.ToDateTime(endDate).AddDays(1).ToString("yyyy-MM-dd");
            }
            else
            {
                newEndDate = endDate;
            }
            string sqlstr2 = "select DATE_FORMAT(gps_time,'%H:%i') AS gps_time,DATE_FORMAT(gps_date,'%Y-%m-%d') AS gps_date,gps_input1,gps_lat,gps_lon,gps_speed from positionbackup where (gps_date_time between '" + DateTime.Parse(startDate).ToString("yyyy-MM-dd ") + " " + DateTime.Parse(startTime).ToString("HH:mm:ss") + "' and '" + DateTime.Parse(newEndDate).ToString("yyyy-MM-dd") + " " + DateTime.Parse(endTime).ToString("HH:mm:ss") + "') and (imei = " + imei + ")and (gps_input1 = 1) and (gps_ign = 1);";
            using (MySqlConnection Con = new MySqlConnection(connection))
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr2, Con))
                {
                    da.Fill(dt);
                    Con.Close();
                    //Con.Dispose();
                }
            }
            resultDistance = SumDistanceOfPeriod(DateTime.Parse(startDate).ToString("yyyy-MM-dd ") + " " + DateTime.Parse(startTime).ToString("HH:mm:ss"), DateTime.Parse(newEndDate).ToString("yyyy-MM-dd") + " " + DateTime.Parse(endTime).ToString("HH:mm:ss"), imei, connection);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Boolean.Parse(dt.Rows[i]["gps_input1"].ToString()))
                {
                    if (i == 0)
                    {
                        timestart = dt.Rows[0]["gps_time"].ToString();
                    }
                    else
                    {
                        ts = DateTime.Parse(dt.Rows[i]["gps_time"].ToString()).Subtract(DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()));
                        if (ts.TotalMinutes >= 3)
                        {

                            for (int a = c + 1; a <= (i - 1); a++)
                            {
                                if (dt.Rows[a]["gps_speed"] != null)
                                {
                                    speed = double.Parse(dt.Rows[a]["gps_speed"].ToString());

                                    if (speed <= 6.0)
                                    {
                                        sumPrice1 += 1.5;
                                    }
                                }
                                sumdis1 += SQL.CalcDistance(Convert.ToDouble(dt.Rows[a - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lon"]) / 1000000, SQL.GeoCodeCalcMeasurement.Kilometers);
                                time += 1;
                            }
                            if (time > 3)
                            {
                                if (Math.Round(sumdis1, 2) > 1)
                                {
                                    count += 1;
                                    actualdistance += Math.Round(sumdis1, 2);
                                    price += PriceDay(Math.Round(sumdis1, 2)) + Convert.ToInt32(sumPrice1);
                                    timestart = dt.Rows[i]["gps_time"].ToString();
                                    c = i;
                                    sumdis1 = 0;
                                    sumPrice1 = 0;
                                }
                            }

                        }

                    }
                }
            }
            return count + ":" + resultDistance + ":" + price + ":" + actualdistance;
        }
        public double SumDistanceOfDate(string dateValues, string imei, string connection)//คำนวณระยะทางเป็นวัน
        {
            MySqlParameter sumdistances;
            using (MySqlConnection Conn101 = new MySqlConnection(connection))
            {
                using (MySqlCommand com101 = new MySqlCommand("DistanceOfDate", Conn101))//DistanceOfDate คือ StoredProcedure ที่ใช้คำนวณระยะทางรวมเป็นวัน
                {
                    com101.CommandType = CommandType.StoredProcedure;
                    com101.Parameters.Add("dateValues", MySqlDbType.VarChar).Value = dateValues;//ส่งค่า วันที่
                    com101.Parameters.Add("boxImei", MySqlDbType.Int64).Value = imei;//ส่งค่า imei (type ต้องเป็น Int64 )
                    sumdistances = com101.Parameters.Add("sumdistance", MySqlDbType.Double);// return ระยะทางรวม
                    sumdistances.Direction = ParameterDirection.Output;
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com101);
                    Conn101.Open();
                    com101.ExecuteNonQuery();
                    Conn101.Close();

                }
            }
            return Convert.ToDouble(sumdistances.Value);

        }
        public static double SumDistanceOfPeriod(string startDateTime, string endDateTime, string imei, string connection)//คำนวณระยะทางเป็นกะ
        {

            MySqlParameter sumdistances;
            using (MySqlConnection Conn101 = new MySqlConnection(connection))
            {
                using (MySqlCommand com101 = new MySqlCommand("DistanceOfPeriod", Conn101))// DistanceOfPeriod  คือ StoredProcedure ที่ใช้คำนวณระยะทางรวมเป็นกะ
                {
                    com101.CommandType = CommandType.StoredProcedure;
                    com101.Parameters.Add("ds", MySqlDbType.VarChar).Value = startDateTime;//ส่งวันเวลาเริ่มต้น(yyyy-MM-dd HH:ss:mm)
                    com101.Parameters.Add("de", MySqlDbType.VarChar).Value = endDateTime;//ส่งวันเวลาสิ้นสุด(yyyy-MM-dd HH:ss:mm)
                    com101.Parameters.Add("boxImei", MySqlDbType.Int64).Value = imei;//ส่ง imei
                    sumdistances = com101.Parameters.Add("sumdistance", MySqlDbType.Double);//return ระยะทางรวม
                    sumdistances.Direction = ParameterDirection.Output;
                    MySqlDataAdapter at98 = new MySqlDataAdapter(com101);
                    Conn101.Open();
                    com101.ExecuteNonQuery();
                    Conn101.Close();


                }
            }
            return Convert.ToDouble(sumdistances.Value);

        }
        public string SubAvgOfDay2(string dateValues, string imei, string connection)//เป็นวัน
        {
            int time = 0, c = 0, sumRow = 0;
            double sumdis2 = 0, distanceCut = 0, speed = 0, sumPrice2 = 0, sumPrice1 = 0;
            TimeSpan ts;
            DataTable dt = new DataTable();
            string sqlstr = "select DATE_FORMAT(gps_time,'%H:%i:%s') AS gps_time ,gps_input1,gps_lat,gps_lon,gps_speed from positionbackup where (gps_date = '" + DateTime.Parse(dateValues).ToString("yyyy-MM-dd") + "') and (imei = " + imei + ") and (gps_input1 = 1) and (gps_ign = 1);";
            using (MySqlConnection Con = new MySqlConnection(connection))
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, Con))
                {
                    da.Fill(dt);
                    Con.Close();
                    //Con.Dispose();
                }
            }
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (Boolean.Parse(dt.Rows[i]["gps_input1"].ToString()))
                {
                    ts = DateTime.Parse(dt.Rows[i]["gps_time"].ToString()).Subtract(DateTime.Parse(dt.Rows[i - 1]["gps_time"].ToString()));
                    if (ts.TotalMinutes >= 3)
                    {
                        for (int a = c + 1; a <= (i - 1); a++)
                        {
                            if (dt.Rows[a]["gps_speed"] != null)
                            {
                                speed = double.Parse(dt.Rows[a]["gps_speed"].ToString());

                                if (speed <= 6.0)
                                {
                                    sumPrice1 += 2; //sumPrice1 += 1.5;
                                }
                            }
                            sumdis2 += SQL.CalcDistance(Convert.ToDouble(dt.Rows[a - 1]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a - 1]["gps_lon"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lat"]) / 1000000, Convert.ToDouble(dt.Rows[a]["gps_lon"]) / 1000000, SQL.GeoCodeCalcMeasurement.Kilometers);
                            time += 1;
                        }
                        if (time > 3)
                        {
                            if (Math.Round(sumdis2, 2) > 1)
                            {
                                sumRow += 1;
                                sumPrice2 += PriceDay(Math.Round(sumdis2, 2)) + Convert.ToInt32(sumPrice1);
                                distanceCut += Math.Round(sumdis2, 2);
                                c = i;
                                sumdis2 = 0;
                                time = 0;
                                sumPrice1 = 0;

                            }
                        }
                    }
                }
            }
            //row["รายได้"] = int.Parse(arr[1]);
            //row["จำนวนรับผู้โดยสาร"] = int.Parse(arr[0]);
            //row["ค่าเชื้อเพลิง"] = sumdis1 * gas;
            //row["เหลือ"] = int.Parse(arr[1]) - (sumdis1 * gas + rent);
            //row["ระยะทางรับผู้โดยสาร"] = double.Parse(arr[2]);
            return sumRow + ":" + sumPrice2 + ":" + distanceCut;
        }
        public static int PriceDay(double distance)//คำนวนค่าแท็กซื่
        {
            double b = 50.0, price = 0, p = 35.0;
            if (distance < 2.19)
            {
                if (distance >= 1.10 && distance < 1.46) //++0.36
                {
                    price = 37;
                }
                else if (distance >= 1.46 && distance < 1.82) //++0.36
                {
                    price = 39;
                }
                else if (distance >= 1.82 && distance < 2.19) //++0.37
                {
                    price = 41;
                }
                else
                {
                    price = p;
                }
            }
            else
            {
                if ((distance >= 2.19) && (distance <= 10.0))
                {
                    price = p + ((distance - 1) * 5.5);
                }
                else if ((distance > 10.0) && (distance <= 20.0))
                {
                    price = p + b + ((distance - 10) * 6.5);
                }
                else if ((distance > 20.0) && (distance <= 40.0))
                {
                    price = p + b + (10.0 * 6.50) + ((distance - 19) * 7.5);
                }
                else if ((distance > 40.0) && (distance <= 60.0))
                {
                    price = p + b + (10.0 * 6.50) + (20 * 7.5) + ((distance - 39) * 8.0);
                }
                else if ((distance > 60.0) && (distance <= 80.0))
                {
                    price = p + b + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + ((distance - 59) * 9.0);
                }
                else if (distance > 80.0)
                {
                    price = p + b + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + (20.0 * 9.0) + ((distance - 79) * 10.5);

                }
            }
            return Convert.ToInt32(price);
        }
        //public int PriceDay2(double distance)//คำนวนค่าแท็กซื่
        //{
        //    if (distance < 2.19)
        //    {
        //        if (distance >= 1.10 && distance < 1.46) //++0.36
        //        {
        //            //price = 37;
        //            return 37;
        //        }
        //        else if (distance >= 1.46 && distance < 1.82) //++0.36
        //        {
        //            //price = 39;
        //            return 39;
        //        }
        //        else if (distance >= 1.82 && distance < 2.19) //++0.37
        //        {
        //            //price = 41;
        //            return 41;
        //        }
        //        else
        //        {
        //            return Convert.ToInt32(_P);
        //        }
        //    }
        //    else
        //    {
        //        if ((distance >= 2.19) && (distance <= 10.0))
        //        {
        //            return Convert.ToInt32(_P + ((distance - 1) * 5.5));
        //        }
        //        else if ((distance > 10.0) && (distance <= 20.0))
        //        {
        //            //price = _P + _B + ((distance - 10) * 6.5);
        //            return Convert.ToInt32(_P + _B + ((distance - 10) * 6.5));
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //        //return 0;
        //    }
        //}
        public int PriceDay1(double distance)//คำนวนค่าแท็กซื่
        {
            //double  price = 0;
            if (distance < 2.19)
            {
                if (distance >= 1.10 && distance < 1.46) //++0.36
                {
                    //price = 37;
                    return 37;
                }
                else if (distance >= 1.46 && distance < 1.82) //++0.36
                {
                    //price = 39;
                    return 39;
                }
                else if (distance >= 1.82 && distance < 2.19) //++0.37
                {
                    //price = 41;
                    return 41;
                }
                else
                {
                    return Convert.ToInt32(_P);
                }
            }
            else
            {
                if ((distance >= 2.19) && (distance <= 10.0))
                {
                    //price = _P + ((distance - 1) * 5.5);
                    return Convert.ToInt32(_P + ((distance - 1) * 5.5));
                }
                else if ((distance > 10.0) && (distance <= 20.0))
                {
                    //price = _P + _B + ((distance - 10) * 6.5);
                    return Convert.ToInt32(_P + _B + ((distance - 10) * 6.5));
                }
                else if ((distance > 20.0) && (distance <= 40.0))
                {
                    //price = _P + _B + (10.0 * 6.50) + ((distance - 19) * 7.5);
                    return Convert.ToInt32(_P + _B + (10.0 * 6.50) + ((distance - 19) * 7.5));
                }
                else if ((distance > 40.0) && (distance <= 60.0))
                {
                    //price = _P + _B + (10.0 * 6.50) + (20 * 7.5) + ((distance - 39) * 8.0);
                    return Convert.ToInt32(_P + _B + (10.0 * 6.50) + (20 * 7.5) + ((distance - 39) * 8.0));
                }
                else if ((distance > 60.0) && (distance <= 80.0))
                {
                    //price = _P + _B + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + ((distance - 59) * 9.0);
                    return Convert.ToInt32(_P + _B + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + ((distance - 59) * 9.0));
                }
                else if (distance > 80.0)
                {
                    //price = _P + _B + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + (20.0 * 9.0) + ((distance - 79) * 10.5);
                    return Convert.ToInt32(_P + _B + (10.0 * 6.50) + (20 * 7.5) + (20.0 * 8.0) + (20.0 * 9.0) + ((distance - 79) * 10.5));
                }
                else
                {
                    return 0;
                }
            }
            //return Convert.ToInt32(price);
        }
    }
}
