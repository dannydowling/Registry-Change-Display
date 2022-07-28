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
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.registryChangeRecorderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.registryChangeRecorderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 9);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Image Registry";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Image_Registry_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(10, 70);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 29);
            this.button2.TabIndex = 1;
            this.button2.Text = "Diff File1 and File2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Diff_File1_File2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 40);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 26);
            this.button3.TabIndex = 2;
            this.button3.Text = "Filter Results";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Title = "Save File";
            // 
            // registryChangeRecorderBindingSource
            // 
            this.registryChangeRecorderBindingSource.DataSource = typeof(Registry_Change_Display.Registry_Change_Recorder);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(209, 75);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 26);
            this.button4.TabIndex = 3;
            this.button4.Text = "Save Diff File";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Save_File_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(209, 40);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(95, 29);
            this.button5.TabIndex = 4;
            this.button5.Text = "Load File2";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Load_File2_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(209, 10);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(95, 26);
            this.button6.TabIndex = 5;
            this.button6.Text = "Load File1";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Load_File1_Click);
            // 
            // Registry_Change_Recorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 112);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Registry_Change_Recorder";
            this.Text = "Registry_Change_Recorder";
            ((System.ComponentModel.ISupportInitialize)(this.registryChangeRecorderBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private SaveFileDialog saveFileDialog1;
        private BindingSource registryChangeRecorderBindingSource;
        private Button button4;
        private Button button5;
        private Button button6;
    }
}