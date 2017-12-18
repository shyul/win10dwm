namespace NcRenderer
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
            this.ncToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.tsSep01 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ncToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ncToolStrip
            // 
            this.ncToolStrip.BackColor = System.Drawing.Color.Black;
            this.ncToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.ncToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ncToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.tsSep01,
            this.btnUndo,
            this.btnRedo});
            this.ncToolStrip.Location = new System.Drawing.Point(10, 3);
            this.ncToolStrip.Name = "ncToolStrip";
            this.ncToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.ncToolStrip.Size = new System.Drawing.Size(123, 25);
            this.ncToolStrip.TabIndex = 1;
            this.ncToolStrip.Text = "Tools";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = global::NcRenderer.Properties.Resources.new16;
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Margin = new System.Windows.Forms.Padding(0);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(23, 25);
            this.btnNew.Text = "New Document";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::NcRenderer.Properties.Resources.open16;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 25);
            this.btnOpen.Text = "Open document";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::NcRenderer.Properties.Resources.save16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 25);
            this.btnSave.Text = "Save document";
            // 
            // tsSep01
            // 
            this.tsSep01.Name = "tsSep01";
            this.tsSep01.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Image = global::NcRenderer.Properties.Resources.undo16;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Margin = new System.Windows.Forms.Padding(0);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 25);
            this.btnUndo.Text = "Undo";
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Image = global::NcRenderer.Properties.Resources.redo16;
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Margin = new System.Windows.Forms.Padding(0);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 25);
            this.btnRedo.Text = "Redo";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(89, 137);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(275, 35);
            this.button1.TabIndex = 2;
            this.button1.Text = "Example 2: Extending Frame into the Client";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(32, 193);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(379, 35);
            this.button2.TabIndex = 3;
            this.button2.Text = "Example 3: Solid Glass Window";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(449, 262);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ncToolStrip);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hello Wold";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ncToolStrip.ResumeLayout(false);
            this.ncToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ncToolStrip;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator tsSep01;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

