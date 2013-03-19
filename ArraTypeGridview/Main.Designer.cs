namespace ArraTypeGridview
{
    partial class Main
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
            this.gridViewPanel = new System.Windows.Forms.Panel();
            this.variableViewer = new System.Windows.Forms.DataGridView();
            this.gridViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.variableViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // gridViewPanel
            // 
            this.gridViewPanel.Controls.Add(this.variableViewer);
            this.gridViewPanel.Location = new System.Drawing.Point(13, 31);
            this.gridViewPanel.Name = "gridViewPanel";
            this.gridViewPanel.Size = new System.Drawing.Size(308, 183);
            this.gridViewPanel.TabIndex = 0;
            // 
            // variableViewer
            // 
            this.variableViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.variableViewer.Location = new System.Drawing.Point(3, 12);
            this.variableViewer.Name = "variableViewer";
            this.variableViewer.Size = new System.Drawing.Size(289, 150);
            this.variableViewer.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 267);
            this.Controls.Add(this.gridViewPanel);
            this.Name = "Main";
            this.Text = "Form1";
            this.gridViewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.variableViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel gridViewPanel;
        private System.Windows.Forms.DataGridView variableViewer;

    }
}

