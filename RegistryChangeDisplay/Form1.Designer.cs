namespace Registry_Change_Display
{
    partial class Registry_Change_Recorder
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button5 = new System.Windows.Forms.Button();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.registryChangeRecorderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.registryChangeRecorderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(203, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Create Snapshot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.create_Initial_Snapshot_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(279, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "List Changes";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.List_Changes_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 54);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(203, 34);
            this.button3.TabIndex = 2;
            this.button3.Text = "Filter Results";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(446, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 33);
            this.button4.TabIndex = 3;
            this.button4.Text = "Clear Filter";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(2, 108);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(556, 564);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 4;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Title = "Save File";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(441, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 30);
            this.button5.TabIndex = 7;
            this.button5.Text = "Save File";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(233, 61);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(207, 27);
            this.maskedTextBox1.TabIndex = 8;
            // 
            // registryChangeRecorderBindingSource
            // 
            this.registryChangeRecorderBindingSource.DataSource = typeof(Registry_Change_Display.Registry_Change_Recorder);
            // 
            // Registry_Change_Recorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 684);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Registry_Change_Recorder";
            this.Text = "Registry_Change_Recorder";
            ((System.ComponentModel.ISupportInitialize)(this.registryChangeRecorderBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private ListBox listBox1;
        private SaveFileDialog saveFileDialog1;
        private Button button5;
        private MaskedTextBox maskedTextBox1;
        private BindingSource registryChangeRecorderBindingSource;
    }
}