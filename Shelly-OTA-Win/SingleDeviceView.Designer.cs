
namespace Shelly_OTA_Win
{
    partial class SingleDeviceView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UpdateStatusLabel = new System.Windows.Forms.Label();
            this.InternetAccessLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdateStatusLabel
            // 
            this.UpdateStatusLabel.AutoSize = true;
            this.UpdateStatusLabel.Location = new System.Drawing.Point(5, 10);
            this.UpdateStatusLabel.Name = "UpdateStatusLabel";
            this.UpdateStatusLabel.Size = new System.Drawing.Size(60, 15);
            this.UpdateStatusLabel.TabIndex = 0;
            this.UpdateStatusLabel.Text = "{{ CODE }}";
            // 
            // InternetAccessLabel
            // 
            this.InternetAccessLabel.AutoSize = true;
            this.InternetAccessLabel.Location = new System.Drawing.Point(5, 38);
            this.InternetAccessLabel.Name = "InternetAccessLabel";
            this.InternetAccessLabel.Size = new System.Drawing.Size(60, 15);
            this.InternetAccessLabel.TabIndex = 1;
            this.InternetAccessLabel.Text = "{{ CODE }}";
            // 
            // SingleDeviceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.InternetAccessLabel);
            this.Controls.Add(this.UpdateStatusLabel);
            this.Location = new System.Drawing.Point(5, 15);
            this.Name = "SingleDeviceView";
            this.Padding = new System.Windows.Forms.Padding(5, 10, 5, 10);
            this.Size = new System.Drawing.Size(540, 180);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UpdateStatusLabel;
        private System.Windows.Forms.Label InternetAccessLabel;
    }
}
