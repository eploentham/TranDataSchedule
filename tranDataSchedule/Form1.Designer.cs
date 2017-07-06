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
            this.pB1 = new System.Windows.Forms.ProgressBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtTimeStart = new System.Windows.Forms.MaskedTextBox();
            this.txtTimeCurrent = new System.Windows.Forms.MaskedTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAuto = new System.Windows.Forms.RadioButton();
            this.chkManual = new System.Windows.Forms.RadioButton();
            this.gbManual = new System.Windows.Forms.GroupBox();
            this.chkDelOldData = new System.Windows.Forms.RadioButton();
            this.chkInsertNew = new System.Windows.Forms.RadioButton();
            this.txtDateManual = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCheckData = new System.Windows.Forms.Button();
            this.lB1 = new System.Windows.Forms.ListBox();
            this.txtConGPSOnLIne = new System.Windows.Forms.TextBox();
            this.txtConnGPS01 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTimeEnd = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbManual.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtTimeEnd);
            this.groupBox1.Controls.Add(this.btnCheckData);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTimeStart);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(741, 417);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "เงื่อนไข";
            // 
            // pB1
            // 
            this.pB1.Location = new System.Drawing.Point(12, 528);
            this.pB1.Name = "pB1";
            this.pB1.Size = new System.Drawing.Size(701, 23);
            this.pB1.TabIndex = 1;
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
            // txtTimeStart
            // 
            this.txtTimeStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeStart.Location = new System.Drawing.Point(635, 37);
            this.txtTimeStart.Mask = "00:00";
            this.txtTimeStart.Name = "txtTimeStart";
            this.txtTimeStart.Size = new System.Drawing.Size(65, 26);
            this.txtTimeStart.TabIndex = 1;
            this.txtTimeStart.ValidatingType = typeof(System.DateTime);
            // 
            // txtTimeCurrent
            // 
            this.txtTimeCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeCurrent.Location = new System.Drawing.Point(654, 493);
            this.txtTimeCurrent.Mask = "00:00";
            this.txtTimeCurrent.Name = "txtTimeCurrent";
            this.txtTimeCurrent.Size = new System.Drawing.Size(59, 29);
            this.txtTimeCurrent.TabIndex = 2;
            this.txtTimeCurrent.ValidatingType = typeof(System.DateTime);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.Location = new System.Drawing.Point(570, 502);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "เวลาปัจจุบัน";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(535, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "เวลาเริ่มทำงาน";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gbManual);
            this.groupBox2.Controls.Add(this.chkManual);
            this.groupBox2.Controls.Add(this.chkAuto);
            this.groupBox2.Location = new System.Drawing.Point(7, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 358);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
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
            // chkManual
            // 
            this.chkManual.AutoSize = true;
            this.chkManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.chkManual.Location = new System.Drawing.Point(19, 191);
            this.chkManual.Name = "chkManual";
            this.chkManual.Size = new System.Drawing.Size(157, 24);
            this.chkManual.TabIndex = 1;
            this.chkManual.TabStop = true;
            this.chkManual.Text = "ดึงข้อมูลแบบmanual";
            this.chkManual.UseVisualStyleBackColor = true;
            this.chkManual.Click += new System.EventHandler(this.chkManual_Click);
            // 
            // gbManual
            // 
            this.gbManual.Controls.Add(this.label3);
            this.gbManual.Controls.Add(this.txtDateManual);
            this.gbManual.Controls.Add(this.chkInsertNew);
            this.gbManual.Controls.Add(this.chkDelOldData);
            this.gbManual.Controls.Add(this.btnStart);
            this.gbManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.gbManual.Location = new System.Drawing.Point(183, 125);
            this.gbManual.Name = "gbManual";
            this.gbManual.Size = new System.Drawing.Size(319, 227);
            this.gbManual.TabIndex = 2;
            this.gbManual.TabStop = false;
            this.gbManual.Text = "เงื่อนการลงข้อมูล";
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
            // txtDateManual
            // 
            this.txtDateManual.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateManual.Location = new System.Drawing.Point(161, 99);
            this.txtDateManual.Name = "txtDateManual";
            this.txtDateManual.Size = new System.Drawing.Size(152, 22);
            this.txtDateManual.TabIndex = 3;
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
            // btnCheckData
            // 
            this.btnCheckData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnCheckData.Location = new System.Drawing.Point(538, 261);
            this.btnCheckData.Name = "btnCheckData";
            this.btnCheckData.Size = new System.Drawing.Size(162, 29);
            this.btnCheckData.TabIndex = 4;
            this.btnCheckData.Text = "ตรวจสอบข้อมูล";
            this.btnCheckData.UseVisualStyleBackColor = true;
            // 
            // lB1
            // 
            this.lB1.FormattingEnabled = true;
            this.lB1.Location = new System.Drawing.Point(719, 13);
            this.lB1.Name = "lB1";
            this.lB1.Size = new System.Drawing.Size(543, 537);
            this.lB1.TabIndex = 4;
            // 
            // txtConGPSOnLIne
            // 
            this.txtConGPSOnLIne.Location = new System.Drawing.Point(203, 437);
            this.txtConGPSOnLIne.Name = "txtConGPSOnLIne";
            this.txtConGPSOnLIne.Size = new System.Drawing.Size(510, 20);
            this.txtConGPSOnLIne.TabIndex = 5;
            // 
            // txtConnGPS01
            // 
            this.txtConnGPS01.Location = new System.Drawing.Point(203, 464);
            this.txtConnGPS01.Name = "txtConnGPS01";
            this.txtConnGPS01.Size = new System.Drawing.Size(510, 20);
            this.txtConnGPS01.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 440);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Conection String gpsonline";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 467);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Conection String gps backup 01";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(535, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "เวลาสิ้นสุดทำงาน";
            // 
            // txtTimeEnd
            // 
            this.txtTimeEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtTimeEnd.Location = new System.Drawing.Point(635, 69);
            this.txtTimeEnd.Mask = "00:00";
            this.txtTimeEnd.Name = "txtTimeEnd";
            this.txtTimeEnd.Size = new System.Drawing.Size(65, 26);
            this.txtTimeEnd.TabIndex = 5;
            this.txtTimeEnd.ValidatingType = typeof(System.DateTime);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1269, 564);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtConnGPS01);
            this.Controls.Add(this.txtConGPSOnLIne);
            this.Controls.Add(this.lB1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTimeCurrent);
            this.Controls.Add(this.pB1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
    }
}

