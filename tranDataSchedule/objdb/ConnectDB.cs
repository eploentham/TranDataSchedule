using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tranDataSchedule.objdb
{
    public class ConnectDB
    {
        public String databaseDB = "gpsonline";
        public String databaseDB01 = "gps_backup_01";
        public String hostDB = "localhost";
        public String userDB = "root";
        public String passwordDB = "-";
        public MySqlConnection connOnLine, conn01;
        public int _rowsAffected = 0;
        public ConnectDB()
        {
            connOnLine = new MySql.Data.MySqlClient.MySqlConnection();
            conn01 = new MySql.Data.MySqlClient.MySqlConnection();
            connOnLine.ConnectionString = "Server=" + hostDB + ";Database=" + databaseDB + ";Uid=" + userDB + ";Pwd=" + passwordDB + ";port = 6318";
            conn01.ConnectionString = "Server=" + hostDB + ";Database=" + databaseDB01 + ";Uid=" + userDB + ";Pwd=" + passwordDB + ";port = 6318";

        }
        public DataTable selectData(String sql)
        {
            DataTable toReturn = new DataTable();

            MySqlCommand com = new MySqlCommand();
            com.CommandText = sql;
            //com.CommandType = CommandType.Text;
            com.Connection = connOnLine;
            MySqlDataAdapter adap = new MySqlDataAdapter(com);
            try
            {
                connOnLine.Open();
                adap.Fill(toReturn);
                //return toReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connOnLine.Close();
                com.Dispose();
                adap.Dispose();
            }
            return toReturn;
        }
        public DataTable selectData(String sql, String conString)
        {
            DataTable toReturn = new DataTable();

            MySqlCommand com = new MySqlCommand();
            com.CommandText = sql;
            //com.CommandType = CommandType.Text;
            connOnLine.ConnectionString = conString;
            com.Connection = connOnLine;
            MySqlDataAdapter adap = new MySqlDataAdapter(com);
            try
            {
                connOnLine.Open();
                adap.Fill(toReturn);
                //return toReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connOnLine.Close();
                com.Dispose();
                adap.Dispose();
            }
            return toReturn;
        }
        public String ExecuteNonQuery(String sql)
        {
            String toReturn = "";
            MySqlCommand com = new MySqlCommand();
            com.CommandText = sql;
            //com.CommandType = CommandType.Text;
            com.Connection = connOnLine;
            try
            {
                connOnLine.Open();
                _rowsAffected = com.ExecuteNonQuery();
                toReturn = _rowsAffected.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::Error occured.", ex);
                toReturn = ex.Message;
            }
            finally
            {
                //_mainConnection.Close();
                connOnLine.Close();
                com.Dispose();
            }

            return toReturn;
        }
    }
}
