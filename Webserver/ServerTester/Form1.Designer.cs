namespace webserver.tester
{
    partial class WebServer
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
            this.ltbDebug = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.brnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ltbDebug
            // 
            this.ltbDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ltbDebug.FormattingEnabled = true;
            this.ltbDebug.Location = new System.Drawing.Point(12, 54);
            this.ltbDebug.Name = "ltbDebug";
            this.ltbDebug.Size = new System.Drawing.Size(412, 342);
            this.ltbDebug.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(46, 25);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(102, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "8080";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(154, 23);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // brnClear
            // 
            this.brnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.brnClear.Location = new System.Drawing.Point(349, 25);
            this.brnClear.Name = "brnClear";
            this.brnClear.Size = new System.Drawing.Size(75, 23);
            this.brnClear.TabIndex = 4;
            this.brnClear.Text = "Clear";
            this.brnClear.UseVisualStyleBackColor = true;
            this.brnClear.Click += new System.EventHandler(this.brnClear_Click);
            // 
            // WebServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 410);
            this.Controls.Add(this.brnClear);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ltbDebug);
            this.Name = "WebServer";
            this.Text = "WebServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ltbDebug;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button brnClear;
    }
}

