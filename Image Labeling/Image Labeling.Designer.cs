namespace Image_Labeling
{
    partial class ImageLabeling
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
            this.tbarBrightness = new System.Windows.Forms.TrackBar();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.tbarContrast = new System.Windows.Forms.TrackBar();
            this.lblContrast = new System.Windows.Forms.Label();
            this.lblCoordinates = new System.Windows.Forms.Label();
            this.lblDisplayScale = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblCurrentFolderText = new System.Windows.Forms.Label();
            this.lblCurrentFolder = new System.Windows.Forms.Label();
            this.lblProcessedFolders = new System.Windows.Forms.Label();
            this.cboxProcessedFolders = new System.Windows.Forms.ComboBox();
            this.lboxFilesList = new System.Windows.Forms.ListBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.lblStars1 = new System.Windows.Forms.Label();
            this.lblStars5 = new System.Windows.Forms.Label();
            this.lblStars4 = new System.Windows.Forms.Label();
            this.lblStars3 = new System.Windows.Forms.Label();
            this.lblStars2 = new System.Windows.Forms.Label();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.lblTotalProcessed = new System.Windows.Forms.Label();
            this.pboxImage = new System.Windows.Forms.PictureBox();
            this.pboxBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbarBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarContrast)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // tbarBrightness
            // 
            this.tbarBrightness.Location = new System.Drawing.Point(77, 483);
            this.tbarBrightness.Maximum = 100;
            this.tbarBrightness.Minimum = -100;
            this.tbarBrightness.Name = "tbarBrightness";
            this.tbarBrightness.Size = new System.Drawing.Size(125, 45);
            this.tbarBrightness.TabIndex = 2;
            this.tbarBrightness.TickFrequency = 20;
            this.tbarBrightness.ValueChanged += new System.EventHandler(this.tbarBrightness_ValueChanged);
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrightness.Location = new System.Drawing.Point(81, 514);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(100, 26);
            this.lblBrightness.TabIndex = 3;
            this.lblBrightness.Text = "Brightness";
            // 
            // tbarContrast
            // 
            this.tbarContrast.Location = new System.Drawing.Point(224, 482);
            this.tbarContrast.Maximum = 500;
            this.tbarContrast.Name = "tbarContrast";
            this.tbarContrast.Size = new System.Drawing.Size(125, 45);
            this.tbarContrast.TabIndex = 5;
            this.tbarContrast.TickFrequency = 50;
            this.tbarContrast.Value = 100;
            this.tbarContrast.ValueChanged += new System.EventHandler(this.tbarContrast_ValueChanged);
            // 
            // lblContrast
            // 
            this.lblContrast.AutoSize = true;
            this.lblContrast.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrast.Location = new System.Drawing.Point(230, 514);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(76, 23);
            this.lblContrast.TabIndex = 6;
            this.lblContrast.Text = "Contrast";
            // 
            // lblCoordinates
            // 
            this.lblCoordinates.AutoSize = true;
            this.lblCoordinates.Location = new System.Drawing.Point(37, 57);
            this.lblCoordinates.Name = "lblCoordinates";
            this.lblCoordinates.Size = new System.Drawing.Size(33, 12);
            this.lblCoordinates.TabIndex = 7;
            this.lblCoordinates.Text = "label1";
            // 
            // lblDisplayScale
            // 
            this.lblDisplayScale.AutoSize = true;
            this.lblDisplayScale.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisplayScale.Location = new System.Drawing.Point(156, 111);
            this.lblDisplayScale.Name = "lblDisplayScale";
            this.lblDisplayScale.Size = new System.Drawing.Size(86, 19);
            this.lblDisplayScale.TabIndex = 8;
            this.lblDisplayScale.Text = "Scale: 100%";
            this.lblDisplayScale.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 29);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseFolderToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // chooseFolderToolStripMenuItem
            // 
            this.chooseFolderToolStripMenuItem.Name = "chooseFolderToolStripMenuItem";
            this.chooseFolderToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.chooseFolderToolStripMenuItem.Text = "Choose folder";
            this.chooseFolderToolStripMenuItem.Click += new System.EventHandler(this.chooseFolderToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(54, 25);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblCurrentFolderText
            // 
            this.lblCurrentFolderText.AutoSize = true;
            this.lblCurrentFolderText.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFolderText.Location = new System.Drawing.Point(557, 109);
            this.lblCurrentFolderText.Name = "lblCurrentFolderText";
            this.lblCurrentFolderText.Size = new System.Drawing.Size(160, 19);
            this.lblCurrentFolderText.TabIndex = 11;
            this.lblCurrentFolderText.Text = "Current Folder and File:";
            // 
            // lblCurrentFolder
            // 
            this.lblCurrentFolder.AutoEllipsis = true;
            this.lblCurrentFolder.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFolder.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblCurrentFolder.Location = new System.Drawing.Point(557, 130);
            this.lblCurrentFolder.Name = "lblCurrentFolder";
            this.lblCurrentFolder.Size = new System.Drawing.Size(25, 19);
            this.lblCurrentFolder.TabIndex = 12;
            this.lblCurrentFolder.Text = "<>";
            // 
            // lblProcessedFolders
            // 
            this.lblProcessedFolders.AutoSize = true;
            this.lblProcessedFolders.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessedFolders.Location = new System.Drawing.Point(555, 50);
            this.lblProcessedFolders.Name = "lblProcessedFolders";
            this.lblProcessedFolders.Size = new System.Drawing.Size(130, 19);
            this.lblProcessedFolders.TabIndex = 13;
            this.lblProcessedFolders.Text = "Processed Folders:";
            // 
            // cboxProcessedFolders
            // 
            this.cboxProcessedFolders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxProcessedFolders.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxProcessedFolders.FormattingEnabled = true;
            this.cboxProcessedFolders.Location = new System.Drawing.Point(559, 71);
            this.cboxProcessedFolders.Name = "cboxProcessedFolders";
            this.cboxProcessedFolders.Size = new System.Drawing.Size(193, 27);
            this.cboxProcessedFolders.TabIndex = 14;
            this.cboxProcessedFolders.SelectedIndexChanged += new System.EventHandler(this.cboxProcessedFolders_SelectedIndexChanged);
            // 
            // lboxFilesList
            // 
            this.lboxFilesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lboxFilesList.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxFilesList.FormattingEnabled = true;
            this.lboxFilesList.HorizontalScrollbar = true;
            this.lboxFilesList.ItemHeight = 19;
            this.lboxFilesList.Location = new System.Drawing.Point(709, 151);
            this.lboxFilesList.Name = "lboxFilesList";
            this.lboxFilesList.Size = new System.Drawing.Size(144, 76);
            this.lboxFilesList.TabIndex = 15;
            this.lboxFilesList.SelectedIndexChanged += new System.EventHandler(this.lboxFilesList_SelectedIndexChanged);
            this.lboxFilesList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lboxFilesList_MouseMove);
            // 
            // timer2
            // 
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // lblStars1
            // 
            this.lblStars1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStars1.ForeColor = System.Drawing.Color.Red;
            this.lblStars1.Location = new System.Drawing.Point(599, 169);
            this.lblStars1.Name = "lblStars1";
            this.lblStars1.Size = new System.Drawing.Size(98, 18);
            this.lblStars1.TabIndex = 16;
            this.lblStars1.Text = "★★";
            this.lblStars1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStars1.Visible = false;
            // 
            // lblStars5
            // 
            this.lblStars5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStars5.ForeColor = System.Drawing.Color.Red;
            this.lblStars5.Location = new System.Drawing.Point(599, 249);
            this.lblStars5.Name = "lblStars5";
            this.lblStars5.Size = new System.Drawing.Size(98, 18);
            this.lblStars5.TabIndex = 17;
            this.lblStars5.Text = "★★ Many ★★";
            this.lblStars5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStars5.Visible = false;
            // 
            // lblStars4
            // 
            this.lblStars4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStars4.ForeColor = System.Drawing.Color.Red;
            this.lblStars4.Location = new System.Drawing.Point(599, 228);
            this.lblStars4.Name = "lblStars4";
            this.lblStars4.Size = new System.Drawing.Size(98, 18);
            this.lblStars4.TabIndex = 18;
            this.lblStars4.Text = "★";
            this.lblStars4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStars4.Visible = false;
            // 
            // lblStars3
            // 
            this.lblStars3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStars3.ForeColor = System.Drawing.Color.Red;
            this.lblStars3.Location = new System.Drawing.Point(599, 208);
            this.lblStars3.Name = "lblStars3";
            this.lblStars3.Size = new System.Drawing.Size(98, 18);
            this.lblStars3.TabIndex = 19;
            this.lblStars3.Text = "★★★";
            this.lblStars3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStars3.Visible = false;
            // 
            // lblStars2
            // 
            this.lblStars2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStars2.ForeColor = System.Drawing.Color.Red;
            this.lblStars2.Location = new System.Drawing.Point(599, 187);
            this.lblStars2.Name = "lblStars2";
            this.lblStars2.Size = new System.Drawing.Size(98, 18);
            this.lblStars2.TabIndex = 20;
            this.lblStars2.Text = "★★";
            this.lblStars2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStars2.Visible = false;
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // lblTotalProcessed
            // 
            this.lblTotalProcessed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalProcessed.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalProcessed.Location = new System.Drawing.Point(587, 521);
            this.lblTotalProcessed.Name = "lblTotalProcessed";
            this.lblTotalProcessed.Padding = new System.Windows.Forms.Padding(5);
            this.lblTotalProcessed.Size = new System.Drawing.Size(251, 19);
            this.lblTotalProcessed.TabIndex = 21;
            this.lblTotalProcessed.Text = "Total: <>, Processed: <>";
            this.lblTotalProcessed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pboxImage
            // 
            this.pboxImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pboxImage.BackgroundImage = global::Image_Labeling.Properties.Resources.Transparent_background;
            this.pboxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pboxImage.Location = new System.Drawing.Point(49, 92);
            this.pboxImage.Name = "pboxImage";
            this.pboxImage.Size = new System.Drawing.Size(408, 363);
            this.pboxImage.TabIndex = 0;
            this.pboxImage.TabStop = false;
            this.pboxImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pboxImage_Paint);
            this.pboxImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pboxImage_MouseDown);
            this.pboxImage.MouseEnter += new System.EventHandler(this.pboxImage_MouseEnter);
            this.pboxImage.MouseLeave += new System.EventHandler(this.pboxImage_MouseLeave);
            this.pboxImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pboxImage_MouseMove);
            this.pboxImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pboxImage_MouseUp);
            // 
            // pboxBackground
            // 
            this.pboxBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pboxBackground.Location = new System.Drawing.Point(12, 44);
            this.pboxBackground.Name = "pboxBackground";
            this.pboxBackground.Size = new System.Drawing.Size(510, 510);
            this.pboxBackground.TabIndex = 9;
            this.pboxBackground.TabStop = false;
            // 
            // ImageLabeling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(884, 566);
            this.Controls.Add(this.lblTotalProcessed);
            this.Controls.Add(this.lblStars2);
            this.Controls.Add(this.lblStars3);
            this.Controls.Add(this.lblStars4);
            this.Controls.Add(this.lblStars5);
            this.Controls.Add(this.lblStars1);
            this.Controls.Add(this.lboxFilesList);
            this.Controls.Add(this.cboxProcessedFolders);
            this.Controls.Add(this.lblProcessedFolders);
            this.Controls.Add(this.lblCurrentFolder);
            this.Controls.Add(this.lblCurrentFolderText);
            this.Controls.Add(this.lblContrast);
            this.Controls.Add(this.lblBrightness);
            this.Controls.Add(this.lblDisplayScale);
            this.Controls.Add(this.lblCoordinates);
            this.Controls.Add(this.tbarContrast);
            this.Controls.Add(this.tbarBrightness);
            this.Controls.Add(this.pboxImage);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pboxBackground);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(900, 455);
            this.Name = "ImageLabeling";
            this.Text = "Image Labeling";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageLabeling_FormClosing);
            this.Load += new System.EventHandler(this.ImageLabeling_Load);
            this.SizeChanged += new System.EventHandler(this.ImageLabeling_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.tbarBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarContrast)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pboxImage;
        private System.Windows.Forms.TrackBar tbarBrightness;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.TrackBar tbarContrast;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.Label lblCoordinates;
        private System.Windows.Forms.Label lblDisplayScale;
        private System.Windows.Forms.PictureBox pboxBackground;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblCurrentFolderText;
        private System.Windows.Forms.Label lblCurrentFolder;
        private System.Windows.Forms.Label lblProcessedFolders;
        private System.Windows.Forms.ComboBox cboxProcessedFolders;
        private System.Windows.Forms.ListBox lboxFilesList;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label lblStars1;
        private System.Windows.Forms.Label lblStars5;
        private System.Windows.Forms.Label lblStars4;
        private System.Windows.Forms.Label lblStars3;
        private System.Windows.Forms.Label lblStars2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Label lblTotalProcessed;
    }
}

