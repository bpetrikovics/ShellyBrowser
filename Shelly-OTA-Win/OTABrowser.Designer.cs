
namespace Shelly_OTA_Win
{
    partial class OTABrowser
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OTABrowser));
            this.BrowserLabel = new System.Windows.Forms.Label();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DeviceCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DeviceListView = new System.Windows.Forms.ListView();
            this.DeviceName = new System.Windows.Forms.ColumnHeader();
            this.DeviceMAC = new System.Windows.Forms.ColumnHeader();
            this.DeviceIP = new System.Windows.Forms.ColumnHeader();
            this.DeviceModel = new System.Windows.Forms.ColumnHeader();
            this.DeviceFirmware = new System.Windows.Forms.ColumnHeader();
            this.BrowserImageList = new System.Windows.Forms.ImageList(this.components);
            this.DetailPanel = new System.Windows.Forms.Panel();
            this.DetailBox = new System.Windows.Forms.GroupBox();
            this.UrlBox = new System.Windows.Forms.GroupBox();
            this.StatusLinkLabel = new System.Windows.Forms.LinkLabel();
            this.DeviceInfoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.WebUILinkLabel = new System.Windows.Forms.LinkLabel();
            this.StatusStrip.SuspendLayout();
            this.DetailPanel.SuspendLayout();
            this.UrlBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // BrowserLabel
            // 
            this.BrowserLabel.AutoSize = true;
            this.BrowserLabel.Location = new System.Drawing.Point(13, 16);
            this.BrowserLabel.Name = "BrowserLabel";
            this.BrowserLabel.Size = new System.Drawing.Size(165, 15);
            this.BrowserLabel.TabIndex = 2;
            this.BrowserLabel.Text = "Devices found on the network";
            // 
            // StatusStrip
            // 
            this.StatusStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.DeviceCountLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 428);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(800, 22);
            this.StatusStrip.TabIndex = 3;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(70, 17);
            this.StatusLabel.Text = "Initializing...";
            // 
            // DeviceCountLabel
            // 
            this.DeviceCountLabel.Name = "DeviceCountLabel";
            this.DeviceCountLabel.Size = new System.Drawing.Size(715, 17);
            this.DeviceCountLabel.Spring = true;
            this.DeviceCountLabel.Text = "No devices";
            this.DeviceCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DeviceListView
            // 
            this.DeviceListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DeviceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DeviceName,
            this.DeviceMAC,
            this.DeviceIP,
            this.DeviceModel,
            this.DeviceFirmware});
            this.DeviceListView.FullRowSelect = true;
            this.DeviceListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.DeviceListView.HideSelection = false;
            this.DeviceListView.Location = new System.Drawing.Point(13, 34);
            this.DeviceListView.MinimumSize = new System.Drawing.Size(4, 180);
            this.DeviceListView.MultiSelect = false;
            this.DeviceListView.Name = "DeviceListView";
            this.DeviceListView.ShowItemToolTips = true;
            this.DeviceListView.Size = new System.Drawing.Size(775, 180);
            this.DeviceListView.SmallImageList = this.BrowserImageList;
            this.DeviceListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.DeviceListView.TabIndex = 4;
            this.DeviceListView.UseCompatibleStateImageBehavior = false;
            this.DeviceListView.View = System.Windows.Forms.View.Details;
            this.DeviceListView.SelectedIndexChanged += new System.EventHandler(this.DeviceListView_SelectedIndexChanged);
            // 
            // DeviceName
            // 
            this.DeviceName.Text = "Device Name";
            this.DeviceName.Width = 210;
            // 
            // DeviceMAC
            // 
            this.DeviceMAC.Text = "MAC";
            this.DeviceMAC.Width = 100;
            // 
            // DeviceIP
            // 
            this.DeviceIP.Text = "IP Address";
            this.DeviceIP.Width = 120;
            // 
            // DeviceModel
            // 
            this.DeviceModel.Text = "Model";
            this.DeviceModel.Width = 100;
            // 
            // DeviceFirmware
            // 
            this.DeviceFirmware.Text = "Firmware";
            this.DeviceFirmware.Width = 240;
            // 
            // BrowserImageList
            // 
            this.BrowserImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.BrowserImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("BrowserImageList.ImageStream")));
            this.BrowserImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.BrowserImageList.Images.SetKeyName(0, "lock-open-variant-outline.png");
            this.BrowserImageList.Images.SetKeyName(1, "lock-alert.png");
            this.BrowserImageList.Images.SetKeyName(2, "lock-open.png");
            // 
            // DetailPanel
            // 
            this.DetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DetailPanel.Controls.Add(this.DetailBox);
            this.DetailPanel.Controls.Add(this.UrlBox);
            this.DetailPanel.Enabled = false;
            this.DetailPanel.Location = new System.Drawing.Point(13, 218);
            this.DetailPanel.Name = "DetailPanel";
            this.DetailPanel.Size = new System.Drawing.Size(775, 207);
            this.DetailPanel.TabIndex = 5;
            // 
            // DetailBox
            // 
            this.DetailBox.Location = new System.Drawing.Point(4, 3);
            this.DetailBox.Name = "DetailBox";
            this.DetailBox.Size = new System.Drawing.Size(550, 201);
            this.DetailBox.TabIndex = 1;
            this.DetailBox.TabStop = false;
            this.DetailBox.Text = "Shelly Device";
            // 
            // UrlBox
            // 
            this.UrlBox.Controls.Add(this.StatusLinkLabel);
            this.UrlBox.Controls.Add(this.DeviceInfoLinkLabel);
            this.UrlBox.Controls.Add(this.WebUILinkLabel);
            this.UrlBox.Location = new System.Drawing.Point(572, 3);
            this.UrlBox.Name = "UrlBox";
            this.UrlBox.Size = new System.Drawing.Size(200, 63);
            this.UrlBox.TabIndex = 0;
            this.UrlBox.TabStop = false;
            this.UrlBox.Text = "Device URLs";
            // 
            // StatusLinkLabel
            // 
            this.StatusLinkLabel.AutoSize = true;
            this.StatusLinkLabel.Location = new System.Drawing.Point(142, 23);
            this.StatusLinkLabel.Name = "StatusLinkLabel";
            this.StatusLinkLabel.Size = new System.Drawing.Size(39, 15);
            this.StatusLinkLabel.TabIndex = 2;
            this.StatusLinkLabel.TabStop = true;
            this.StatusLinkLabel.Text = "Status";
            this.StatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StatusLinkLabel_LinkClicked);
            // 
            // DeviceInfoLinkLabel
            // 
            this.DeviceInfoLinkLabel.AutoSize = true;
            this.DeviceInfoLinkLabel.Location = new System.Drawing.Point(70, 23);
            this.DeviceInfoLinkLabel.Name = "DeviceInfoLinkLabel";
            this.DeviceInfoLinkLabel.Size = new System.Drawing.Size(66, 15);
            this.DeviceInfoLinkLabel.TabIndex = 1;
            this.DeviceInfoLinkLabel.TabStop = true;
            this.DeviceInfoLinkLabel.Text = "Device Info";
            this.DeviceInfoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DeviceInfoLinkLabel_LinkClicked);
            // 
            // WebUILinkLabel
            // 
            this.WebUILinkLabel.AutoSize = true;
            this.WebUILinkLabel.Location = new System.Drawing.Point(19, 23);
            this.WebUILinkLabel.Name = "WebUILinkLabel";
            this.WebUILinkLabel.Size = new System.Drawing.Size(45, 15);
            this.WebUILinkLabel.TabIndex = 0;
            this.WebUILinkLabel.TabStop = true;
            this.WebUILinkLabel.Text = "Web UI";
            this.WebUILinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebUILinkLabel_LinkClicked);
            // 
            // OTABrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DetailPanel);
            this.Controls.Add(this.DeviceListView);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.BrowserLabel);
            this.Name = "OTABrowser";
            this.Text = "Shelly OTA Browser";
            this.Load += new System.EventHandler(this.OTABrowser_Load);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.DetailPanel.ResumeLayout(false);
            this.UrlBox.ResumeLayout(false);
            this.UrlBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label BrowserLabel;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel DeviceCountLabel;
        private System.Windows.Forms.ListView DeviceListView;
        private System.Windows.Forms.ColumnHeader DeviceName;
        private System.Windows.Forms.ColumnHeader DeviceMAC;
        private System.Windows.Forms.ColumnHeader DeviceIP;
        private System.Windows.Forms.ColumnHeader DeviceModel;
        private System.Windows.Forms.ColumnHeader DeviceFirmware;
        private System.Windows.Forms.Panel DetailPanel;
        private System.Windows.Forms.GroupBox UrlBox;
        private System.Windows.Forms.LinkLabel WebUILinkLabel;
        private System.Windows.Forms.LinkLabel DeviceInfoLinkLabel;
        private System.Windows.Forms.LinkLabel StatusLinkLabel;
        private System.Windows.Forms.GroupBox DetailBox;
        private System.Windows.Forms.ImageList BrowserImageList;
    }
}

