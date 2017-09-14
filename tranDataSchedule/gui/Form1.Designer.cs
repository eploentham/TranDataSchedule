namespace tranDataSchedule
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtGPSErrorNoInsert = new System.Windows.Forms.NumericUpDown();
            this.btnTest = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGPSError = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTimeEnd = new System.Windows.Forms.MaskedTextBox();
            this.btnCheckData = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAutoStart = new System.Windows.Forms.MaskedTextBox();
            this.gbManual = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDateManual = new System.Windows.Forms.DateTimePicker();
            this.chkInsertNew = new System.Windows.Forms.RadioButton();
            this.chkDelOldData = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.chkManual = new System.Windows.Forms.RadioButton();
            this.chkAuto = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTimeStart = new System.Windows.Forms.MaskedTextBox();
            this.txtTimeCurrent = new System.Windows.Forms.MaskedTextBox();
            this.pB1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lB1 = new System.Windows.Forms.ListBox();
            this.txtConGPSOnLIne = new System.Windows.Forms.TextBox();
            this.txtConnGPS01 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtConnDaily = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGPSErrorNoInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGPSError)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.gbManual.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtGPSErrorNoInsert);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtGPSError);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtTimeEnd);
            this.groupBox1.Controls.Add(this.btnCheckData);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtTimeStart);
            this.groupBox1.Controls.Add(this.txtTimeCurrent);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1090, 343);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "เงื่อนไข";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(529, 208);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(182, 13);
            this.label10.TabIndex = 41;
            this.label10.Text = "ค่าคลาดเคลื่อน กดmeter แต่ไม่ออกรถ";
            // 
            // txtGPSErrorNoInsert
            // 
            this.txtGPSErrorNoInsert.Location = new System.Drawing.Point(653, 224);
            this.txtGPSErrorNoInsert.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.txtGPSErrorNoInsert.Name = "txtGPSErrorNoInsert";
            this.txtGPSErrorNoInsert.Size = new System.Drawing.Size(65, 20);
            this.txtGPSErrorNoInsert.TabIndex = 42;
            this.txtGPSErrorNoInsert.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(530, 297);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(160, 23);
            this.btnTest.TabIndex = 9;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(529, 169);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(189, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "ค่าคลาดเคลื่อน จอดรถรับผู้โดยสารทันที";
            // 
            // txtGPSError
            // 
            this.txtGPSError.Location = new System.Drawing.Point(653, 185);
            this.txtGPSError.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.txtGPSError.Name = "txtGPSError";
            this.txtGPSError.Size = new System.Drawing.Size(65, 20);
            this.txtGPSError.TabIndex = 40;
            this.txtGPSError.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(527, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "เวลาสิ้นสุดทำงาน";
            // 
            // txtTimeEnd
            // 
            this.txtTimeEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeEnd.Location = new System.Drawing.Point(627, 69);
            this.txtTimeEnd.Mask = "00:00";
            this.txtTimeEnd.Name = "txtTimeEnd";
            this.txtTimeEnd.Size = new System.Drawing.Size(65, 26);
            this.txtTimeEnd.TabIndex = 5;
            this.txtTimeEnd.ValidatingType = typeof(System.DateTime);
            // 
            // btnCheckData
            // 
            this.btnCheckData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnCheckData.Location = new System.Drawing.Point(528, 261);
            this.btnCheckData.Name = "btnCheckData";
            this.btnCheckData.Size = new System.Drawing.Size(162, 29);
            this.btnCheckData.TabIndex = 4;
            this.btnCheckData.Text = "ตรวจสอบข้อมูล";
            this.btnCheckData.UseVisualStyleBackColor = true;
            this.btnCheckData.Click += new System.EventHandler(this.btnCheckData_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtAutoStart);
            this.groupBox2.Controls.Add(this.gbManual);
            this.groupBox2.Controls.Add(this.chkManual);
            this.groupBox2.Controls.Add(this.chkAuto);
            this.groupBox2.Location = new System.Drawing.Point(7, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 299);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label9.Location = new System.Drawing.Point(177, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "เวลาดึงข้อมูล";
            // 
            // txtAutoStart
            // 
            this.txtAutoStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtAutoStart.Location = new System.Drawing.Point(277, 16);
            this.txtAutoStart.Mask = "00:00";
            this.txtAutoStart.Name = "txtAutoStart";
            this.txtAutoStart.Size = new System.Drawing.Size(65, 29);
            this.txtAutoStart.TabIndex = 4;
            this.txtAutoStart.ValidatingType = typeof(System.DateTime);
            // 
            // gbManual
            // 
            this.gbManual.Controls.Add(this.label3);
            this.gbManual.Controls.Add(this.txtDateManual);
            this.gbManual.Controls.Add(this.chkInsertNew);
            this.gbManual.Controls.Add(this.chkDelOldData);
            this.gbManual.Controls.Add(this.btnStart);
            this.gbManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.gbManual.Location = new System.Drawing.Point(183, 63);
            this.gbManual.Name = "gbManual";
            this.gbManual.Size = new System.Drawing.Size(319, 227);
            this.gbManual.TabIndex = 2;
            this.gbManual.TabStop = false;
            this.gbManual.Text = "เงื่อนการลงข้อมูล";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "วันที่ต้องการดึงข้อมูล";
            // 
            // txtDateManual
            // 
            this.txtDateManual.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateManual.Location = new System.Drawing.Point(161, 99);
            this.txtDateManual.Name = "txtDateManual";
            this.txtDateManual.Size = new System.Drawing.Size(152, 22);
            this.txtDateManual.TabIndex = 3;
            // 
            // chkInsertNew
            // 
            this.chkInsertNew.AutoSize = true;
            this.chkInsertNew.Location = new System.Drawing.Point(31, 72);
            this.chkInsertNew.Name = "chkInsertNew";
            this.chkInsertNew.Size = new System.Drawing.Size(127, 20);
            this.chkInsertNew.TabIndex = 2;
            this.chkInsertNew.TabStop = true;
            this.chkInsertNew.Text = "ให้ลงเฉพาะข้อมูลใหม่";
            this.chkInsertNew.UseVisualStyleBackColor = true;
            // 
            // chkDelOldData
            // 
            this.chkDelOldData.AutoSize = true;
            this.chkDelOldData.Location = new System.Drawing.Point(31, 32);
            this.chkDelOldData.Name = "chkDelOldData";
            this.chkDelOldData.Size = new System.Drawing.Size(128, 20);
            this.chkDelOldData.TabIndex = 1;
            this.chkDelOldData.TabStop = true;
            this.chkDelOldData.Text = "มีข้อมูลเก่า ให้ลบข้อมูล";
            this.chkDelOldData.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnStart.Location = new System.Drawing.Point(108, 175);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(205, 46);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "เริ่มทำงาน แบบManually";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // chkManual
            // 
            this.chkManual.AutoSize = true;
            this.chkManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.chkManual.Location = new System.Drawing.Point(19, 129);
            this.chkManual.Name = "chkManual";
            this.chkManual.Size = new System.Drawing.Size(157, 24);
            this.chkManual.TabIndex = 1;
            this.chkManual.TabStop = true;
            this.chkManual.Text = "ดึงข้อมูลแบบmanual";
            this.chkManual.UseVisualStyleBackColor = true;
            this.chkManual.Click += new System.EventHandler(this.chkManual_Click);
            // 
            // chkAuto
            // 
            this.chkAuto.AutoSize = true;
            this.chkAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.chkAuto.Location = new System.Drawing.Point(19, 20);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.Size = new System.Drawing.Size(132, 24);
            this.chkAuto.TabIndex = 0;
            this.chkAuto.TabStop = true;
            this.chkAuto.Text = "ดึงข้อมูลประจำวัน";
            this.chkAuto.UseVisualStyleBackColor = true;
            this.chkAuto.Click += new System.EventHandler(this.chkAuto_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(527, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "เวลาเริ่มทำงาน";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.Location = new System.Drawing.Point(527, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "เวลาปัจจุบัน";
            // 
            // txtTimeStart
            // 
            this.txtTimeStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeStart.Location = new System.Drawing.Point(627, 37);
            this.txtTimeStart.Mask = "00:00";
            this.txtTimeStart.Name = "txtTimeStart";
            this.txtTimeStart.Size = new System.Drawing.Size(65, 26);
            this.txtTimeStart.TabIndex = 1;
            this.txtTimeStart.ValidatingType = typeof(System.DateTime);
            // 
            // txtTimeCurrent
            // 
            this.txtTimeCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeCurrent.Location = new System.Drawing.Point(627, 102);
            this.txtTimeCurrent.Mask = "00:00";
            this.txtTimeCurrent.Name = "txtTimeCurrent";
            this.txtTimeCurrent.Size = new System.Drawing.Size(65, 29);
            this.txtTimeCurrent.TabIndex = 2;
            this.txtTimeCurrent.ValidatingType = typeof(System.DateTime);
            // 
            // pB1
            // 
            this.pB1.Location = new System.Drawing.Point(12, 460);
            this.pB1.Name = "pB1";
            this.pB1.Size = new System.Drawing.Size(1091, 23);
            this.pB1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lB1
            // 
            this.lB1.FormattingEnabled = true;
            this.lB1.Location = new System.Drawing.Point(13, 489);
            this.lB1.Name = "lB1";
            this.lB1.Size = new System.Drawing.Size(1091, 199);
            this.lB1.TabIndex = 4;
            // 
            // txtConGPSOnLIne
            // 
            this.txtConGPSOnLIne.Location = new System.Drawing.Point(203, 366);
            this.txtConGPSOnLIne.Name = "txtConGPSOnLIne";
            this.txtConGPSOnLIne.Size = new System.Drawing.Size(900, 20);
            this.txtConGPSOnLIne.TabIndex = 5;
            // 
            // txtConnGPS01
            // 
            this.txtConnGPS01.Location = new System.Drawing.Point(203, 393);
            this.txtConnGPS01.Name = "txtConnGPS01";
            this.txtConnGPS01.Size = new System.Drawing.Size(900, 20);
            this.txtConnGPS01.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 369);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Conection String gpsonline";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 396);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Conection String gps backup 01";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 437);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(139, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Conection String daily report";
            // 
            // txtConnDaily
            // 
            this.txtConnDaily.Location = new System.Drawing.Point(203, 434);
            this.txtConnDaily.Name = "txtConnDaily";
            this.txtConnDaily.Size = new System.Drawing.Size(900, 20);
            this.txtConnDaily.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 704);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtConnDaily);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtConnGPS01);
            this.Controls.Add(this.txtConGPSOnLIne);
            this.Controls.Add(this.lB1);
            this.Controls.Add(this.pB1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGPSErrorNoInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGPSError)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbManual.ResumeLayout(false);
            this.gbManual.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar pB1;
        private System.Windows.Forms.MaskedTextBox txtTimeStart;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.MaskedTextBox txtTimeCurrent;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton chkManual;
        private System.Windows.Forms.RadioButton chkAuto;
        private System.Windows.Forms.GroupBox gbManual;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker txtDateManual;
        private System.Windows.Forms.RadioButton chkInsertNew;
        private System.Windows.Forms.RadioButton chkDelOldData;
        private System.Windows.Forms.Button btnCheckData;
        private System.Windows.Forms.ListBox lB1;
        private System.Windows.Forms.TextBox txtConGPSOnLIne;
        private System.Windows.Forms.TextBox txtConnGPS01;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MaskedTextBox txtTimeEnd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtConnDaily;
        private System.Windows.Forms.NumericUpDown txtGPSError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.MaskedTextBox txtAutoStart;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown txtGPSErrorNoInsert;
    }
}

