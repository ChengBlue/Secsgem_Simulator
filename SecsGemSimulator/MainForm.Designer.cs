namespace SecsGemSimulator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpNetwork = new System.Windows.Forms.GroupBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblDeviceId = new System.Windows.Forms.Label();
            this.txtDeviceId = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpSend = new System.Windows.Forms.GroupBox();
            this.lblStream = new System.Windows.Forms.Label();
            this.txtStream = new System.Windows.Forms.TextBox();
            this.lblFunction = new System.Windows.Forms.Label();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.chkReply = new System.Windows.Forms.CheckBox();
            this.lblBody = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.cmbTemplates = new System.Windows.Forms.ComboBox();
            this.lblTemplates = new System.Windows.Forms.Label();

            this.grpLog = new System.Windows.Forms.GroupBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.chkHexMode = new System.Windows.Forms.CheckBox();
            this.btnClearLog = new System.Windows.Forms.Button();

            this.grpNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpSend.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();

            // 
            // grpNetwork
            // 
            this.grpNetwork.Controls.Add(this.btnSaveConfig);
            this.grpNetwork.Controls.Add(this.btnConnect);
            this.grpNetwork.Controls.Add(this.chkActive);
            this.grpNetwork.Controls.Add(this.txtDeviceId);
            this.grpNetwork.Controls.Add(this.lblDeviceId);
            this.grpNetwork.Controls.Add(this.txtPort);
            this.grpNetwork.Controls.Add(this.lblPort);
            this.grpNetwork.Controls.Add(this.txtIp);
            this.grpNetwork.Controls.Add(this.lblIp);
            this.grpNetwork.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpNetwork.Location = new System.Drawing.Point(0, 0);
            this.grpNetwork.Name = "grpNetwork";
            this.grpNetwork.Size = new System.Drawing.Size(800, 70);
            this.grpNetwork.TabIndex = 0;
            this.grpNetwork.TabStop = false;
            this.grpNetwork.Text = "Network Configuration";

            // lblIp
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(12, 25);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(20, 15);
            this.lblIp.Text = "IP:";

            // txtIp
            this.txtIp.Location = new System.Drawing.Point(38, 22);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(100, 23);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "127.0.0.1";

            // lblPort
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(150, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 15);
            this.lblPort.Text = "Port:";

            // txtPort
            this.txtPort.Location = new System.Drawing.Point(188, 22);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(60, 23);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "5000";

            // lblDeviceId
            this.lblDeviceId.AutoSize = true;
            this.lblDeviceId.Location = new System.Drawing.Point(260, 25);
            this.lblDeviceId.Name = "lblDeviceId";
            this.lblDeviceId.Size = new System.Drawing.Size(57, 15);
            this.lblDeviceId.Text = "DeviceID:";

            // txtDeviceId
            this.txtDeviceId.Location = new System.Drawing.Point(323, 22);
            this.txtDeviceId.Name = "txtDeviceId";
            this.txtDeviceId.Size = new System.Drawing.Size(50, 23);
            this.txtDeviceId.TabIndex = 3;
            this.txtDeviceId.Text = "0";

            // chkActive
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(390, 24);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(92, 19);
            this.chkActive.TabIndex = 4;
            this.chkActive.Text = "Active Mode";
            this.chkActive.UseVisualStyleBackColor = true;

            // btnConnect
            this.btnConnect.Location = new System.Drawing.Point(500, 21);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 25);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Start/Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // btnSaveConfig
            this.btnSaveConfig.Location = new System.Drawing.Point(610, 21);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(80, 25);
            this.btnSaveConfig.TabIndex = 6;
            this.btnSaveConfig.Text = "Save Config";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);

            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 70);
            this.splitContainer1.Name = "splitContainer1";
            
            // Panel1 (Left) - Send
            this.splitContainer1.Panel1.Controls.Add(this.grpSend);
            
            // Panel2 (Right) - Log
            this.splitContainer1.Panel2.Controls.Add(this.grpLog);
            
            this.splitContainer1.Size = new System.Drawing.Size(800, 380);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 1;

            // 
            // grpSend
            // 
            this.grpSend.Controls.Add(this.lblTemplates);
            this.grpSend.Controls.Add(this.cmbTemplates);
            this.grpSend.Controls.Add(this.btnSend);
            this.grpSend.Controls.Add(this.txtBody);
            this.grpSend.Controls.Add(this.lblBody);
            this.grpSend.Controls.Add(this.chkReply);
            this.grpSend.Controls.Add(this.txtFunction);
            this.grpSend.Controls.Add(this.lblFunction);
            this.grpSend.Controls.Add(this.txtStream);
            this.grpSend.Controls.Add(this.lblStream);
            this.grpSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSend.Location = new System.Drawing.Point(0, 0);
            this.grpSend.Name = "grpSend";
            this.grpSend.Size = new System.Drawing.Size(300, 380);
            this.grpSend.TabIndex = 0;
            this.grpSend.TabStop = false;
            this.grpSend.Text = "Send Message";

            // lblStream
            this.lblStream.AutoSize = true;
            this.lblStream.Location = new System.Drawing.Point(10, 25);
            this.lblStream.Name = "lblStream";
            this.lblStream.Size = new System.Drawing.Size(48, 15);
            this.lblStream.Text = "Stream:";

            // txtStream
            this.txtStream.Location = new System.Drawing.Point(60, 22);
            this.txtStream.Name = "txtStream";
            this.txtStream.Size = new System.Drawing.Size(40, 23);
            this.txtStream.TabIndex = 0;
            this.txtStream.Text = "1";

            // lblFunction
            this.lblFunction.AutoSize = true;
            this.lblFunction.Location = new System.Drawing.Point(110, 25);
            this.lblFunction.Name = "lblFunction";
            this.lblFunction.Size = new System.Drawing.Size(58, 15);
            this.lblFunction.Text = "Function:";

            // txtFunction
            this.txtFunction.Location = new System.Drawing.Point(170, 22);
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(40, 23);
            this.txtFunction.TabIndex = 1;
            this.txtFunction.Text = "1";

            // chkReply
            this.chkReply.AutoSize = true;
            this.chkReply.Location = new System.Drawing.Point(220, 24);
            this.chkReply.Name = "chkReply";
            this.chkReply.Size = new System.Drawing.Size(68, 19);
            this.chkReply.TabIndex = 2;
            this.chkReply.Text = "Wait Bit";
            this.chkReply.UseVisualStyleBackColor = true;
            this.chkReply.Checked = true;

            // lblTemplates
            this.lblTemplates.AutoSize = true;
            this.lblTemplates.Location = new System.Drawing.Point(10, 60);
            this.lblTemplates.Name = "lblTemplates";
            this.lblTemplates.Size = new System.Drawing.Size(58, 15);
            this.lblTemplates.Text = "Template:";

            // cmbTemplates
            this.cmbTemplates.Location = new System.Drawing.Point(80, 57);
            this.cmbTemplates.Name = "cmbTemplates";
            this.cmbTemplates.Size = new System.Drawing.Size(200, 23);
            this.cmbTemplates.TabIndex = 3;
            this.cmbTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTemplates.SelectedIndexChanged += new System.EventHandler(this.cmbTemplates_SelectedIndexChanged);

            // lblBody
            this.lblBody.AutoSize = true;
            this.lblBody.Location = new System.Drawing.Point(10, 90);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(85, 15);
            this.lblBody.Text = "Body (SML-like):";

            // txtBody
            this.txtBody.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtBody.Location = new System.Drawing.Point(10, 110);
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(280, 220);
            this.txtBody.TabIndex = 4;
            this.txtBody.Text = "";
            this.txtBody.Font = new System.Drawing.Font("Consolas", 9F);

            // btnSend
            this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.btnSend.Location = new System.Drawing.Point(10, 340);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(280, 30);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "Send Message";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.btnClearLog);
            this.grpLog.Controls.Add(this.chkHexMode);
            this.grpLog.Controls.Add(this.rtbLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(0, 0);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(496, 380);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Communication Log";

            // chkHexMode
            this.chkHexMode.AutoSize = true;
            this.chkHexMode.Location = new System.Drawing.Point(10, 20);
            this.chkHexMode.Name = "chkHexMode";
            this.chkHexMode.Size = new System.Drawing.Size(115, 19);
            this.chkHexMode.TabIndex = 0;
            this.chkHexMode.Text = "Show Hex Details";
            this.chkHexMode.UseVisualStyleBackColor = true;

            // btnClearLog
            this.btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(410, 15);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            // rtbLog
            this.rtbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.rtbLog.Location = new System.Drawing.Point(10, 50);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(480, 320);
            this.rtbLog.TabIndex = 2;
            this.rtbLog.ReadOnly = true;
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.ForeColor = System.Drawing.Color.Lime;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.grpNetwork);
            this.Name = "MainForm";
            this.Text = "SECS/GEM Simulator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.grpNetwork.ResumeLayout(false);
            this.grpNetwork.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpSend.ResumeLayout(false);
            this.grpSend.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpNetwork;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblDeviceId;
        private System.Windows.Forms.TextBox txtDeviceId;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSaveConfig;
        
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpSend;
        private System.Windows.Forms.Label lblStream;
        private System.Windows.Forms.TextBox txtStream;
        private System.Windows.Forms.Label lblFunction;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.CheckBox chkReply;
        private System.Windows.Forms.Label lblBody;
        private System.Windows.Forms.RichTextBox txtBody;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox cmbTemplates;
        private System.Windows.Forms.Label lblTemplates;
        
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.CheckBox chkHexMode;
        private System.Windows.Forms.Button btnClearLog;
    }
}
