namespace Image_Labeling
{
    partial class About_Box
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
            this.btnOK = new System.Windows.Forms.Button();
            this.lblImageLabeling = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblHKASTRI = new System.Windows.Forms.Label();
            this.pboxAstriLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pboxAstriLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(286, 184);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(86, 29);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblImageLabeling
            // 
            this.lblImageLabeling.AutoSize = true;
            this.lblImageLabeling.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageLabeling.Location = new System.Drawing.Point(183, 32);
            this.lblImageLabeling.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImageLabeling.Name = "lblImageLabeling";
            this.lblImageLabeling.Size = new System.Drawing.Size(111, 19);
            this.lblImageLabeling.TabIndex = 2;
            this.lblImageLabeling.Text = "Defect Labeling";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(183, 62);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(81, 19);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "Version 1.3";
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(183, 94);
            this.lblCopyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(165, 19);
            this.lblCopyright.TabIndex = 4;
            this.lblCopyright.Text = "Copyright © ASTRI 2019";
            // 
            // lblHKASTRI
            // 
            this.lblHKASTRI.AutoSize = true;
            this.lblHKASTRI.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHKASTRI.Location = new System.Drawing.Point(183, 129);
            this.lblHKASTRI.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHKASTRI.Name = "lblHKASTRI";
            this.lblHKASTRI.Size = new System.Drawing.Size(412, 19);
            this.lblHKASTRI.TabIndex = 5;
            this.lblHKASTRI.Text = "Hong Kong Applied Science and Technology Research Institute";
            // 
            // pboxAstriLogo
            // 
            this.pboxAstriLogo.Image = global::Image_Labeling.Properties.Resources.AstriLogo;
            this.pboxAstriLogo.Location = new System.Drawing.Point(36, 33);
            this.pboxAstriLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pboxAstriLogo.Name = "pboxAstriLogo";
            this.pboxAstriLogo.Size = new System.Drawing.Size(121, 77);
            this.pboxAstriLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pboxAstriLogo.TabIndex = 1;
            this.pboxAstriLogo.TabStop = false;
            // 
            // About_Box
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 227);
            this.Controls.Add(this.lblHKASTRI);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblImageLabeling);
            this.Controls.Add(this.pboxAstriLogo);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About_Box";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Defect Labeling";
            ((System.ComponentModel.ISupportInitialize)(this.pboxAstriLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pboxAstriLogo;
        private System.Windows.Forms.Label lblImageLabeling;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblHKASTRI;
    }
}