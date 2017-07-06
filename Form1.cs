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
            txtTimeCurrent.Text = System.DateTime.Now.ToShortTimeString();
        }
        private void selectCar(String dateStart, String dateEnd)
        {
            //String dateStart = "", dateEnd = "";
            String sql = "", carId="", day2="";
            DataTable dtCar = new DataTable();
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
                carId = dtCar.Rows[i]["car_id"].ToString();
                //lB1.Items.Add(carId);
                km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);
                //    km = tdsC.sql.SumDistanceOfDate(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConGPSOnLIne.Text);
                //day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), tdsC.conn.conn01.ConnectionString);
                day2 = tdsC.sql.SubAvgOfDay2(dateStart, dtCar.Rows[i]["imei"].ToString(), txtConnGPS01.Text);

                lB1.Items.Add(carId + " ระยะทาง " + km.ToString() + " จำนวนรับผู้โดยสาร " + day2[0] + " รายได้ " + day2[1] + " ระยะทางรับผู้โดยสาร " + day2[2]);
                lB1.Refresh();
                pB1.Value = i;
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
