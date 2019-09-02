namespace Image_Labeling
{
    partial class Shortcut_keys
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
            this.pboxShortcut = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pboxShortcut)).BeginInit();
            this.SuspendLayout();
            // 
            // pboxShortcut
            // 
            this.pboxShortcut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pboxShortcut.Image = global::Image_Labeling.Properties.Resources.Shortcut_keys;
            this.pboxShortcut.Location = new System.Drawing.Point(26, 16);
            this.pboxShortcut.Name = "pboxShortcut";
            this.pboxShortcut.Size = new System.Drawing.Size(503, 749);
            this.pboxShortcut.TabIndex = 0;
            this.pboxShortcut.TabStop = false;
            // 
            // Shortcut_keys
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 777);
            this.Controls.Add(this.pboxShortcut);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Shortcut_keys";
            this.Text = "Shortcut_keys";
            ((System.ComponentModel.ISupportInitialize)(this.pboxShortcut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pboxShortcut;
    }
}