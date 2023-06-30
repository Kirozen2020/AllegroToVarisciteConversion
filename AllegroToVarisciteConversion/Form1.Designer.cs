namespace AllegroToVarisciteConversion
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenPlace = new System.Windows.Forms.Button();
            this.btnOpenCoords = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnOpenPlace
            // 
            this.btnOpenPlace.Location = new System.Drawing.Point(192, 146);
            this.btnOpenPlace.Name = "btnOpenPlace";
            this.btnOpenPlace.Size = new System.Drawing.Size(176, 23);
            this.btnOpenPlace.TabIndex = 0;
            this.btnOpenPlace.Text = "Open Coordinates Report File";
            this.btnOpenPlace.UseVisualStyleBackColor = true;
            this.btnOpenPlace.Click += new System.EventHandler(this.btnOpenPlace_Click);
            // 
            // btnOpenCoords
            // 
            this.btnOpenCoords.Location = new System.Drawing.Point(192, 175);
            this.btnOpenCoords.Name = "btnOpenCoords";
            this.btnOpenCoords.Size = new System.Drawing.Size(176, 23);
            this.btnOpenCoords.TabIndex = 1;
            this.btnOpenCoords.Text = "Open Placement Report File";
            this.btnOpenCoords.UseVisualStyleBackColor = true;
            this.btnOpenCoords.Click += new System.EventHandler(this.btnOpenCoords_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(234, 204);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(89, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save File";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 380);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpenCoords);
            this.Controls.Add(this.btnOpenPlace);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnOpenPlace;
        private System.Windows.Forms.Button btnOpenCoords;
        private System.Windows.Forms.Button btnSave;
    }
}

