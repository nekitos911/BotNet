namespace Builder
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tFile = new System.Windows.Forms.TextBox();
            this.tIcon = new System.Windows.Forms.TextBox();
            this.bFile = new System.Windows.Forms.Button();
            this.bIcon = new System.Windows.Forms.Button();
            this.cIcon = new System.Windows.Forms.CheckBox();
            this.cNative = new System.Windows.Forms.CheckBox();
            this.bCrypt = new System.Windows.Forms.Button();
            this.cOutVersion = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tArgs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Icon:";
            // 
            // tFile
            // 
            this.tFile.Location = new System.Drawing.Point(70, 32);
            this.tFile.Name = "tFile";
            this.tFile.Size = new System.Drawing.Size(291, 20);
            this.tFile.TabIndex = 2;
            // 
            // tIcon
            // 
            this.tIcon.Location = new System.Drawing.Point(70, 58);
            this.tIcon.Name = "tIcon";
            this.tIcon.Size = new System.Drawing.Size(289, 20);
            this.tIcon.TabIndex = 3;
            // 
            // bFile
            // 
            this.bFile.Location = new System.Drawing.Point(367, 30);
            this.bFile.Name = "bFile";
            this.bFile.Size = new System.Drawing.Size(32, 23);
            this.bFile.TabIndex = 4;
            this.bFile.Text = "...";
            this.bFile.UseVisualStyleBackColor = true;
            this.bFile.Click += new System.EventHandler(this.bFile_Click);
            // 
            // bIcon
            // 
            this.bIcon.Location = new System.Drawing.Point(367, 56);
            this.bIcon.Name = "bIcon";
            this.bIcon.Size = new System.Drawing.Size(32, 23);
            this.bIcon.TabIndex = 5;
            this.bIcon.Text = "...";
            this.bIcon.UseVisualStyleBackColor = true;
            this.bIcon.Click += new System.EventHandler(this.bIcon_Click);
            // 
            // cIcon
            // 
            this.cIcon.AutoSize = true;
            this.cIcon.Location = new System.Drawing.Point(70, 110);
            this.cIcon.Name = "cIcon";
            this.cIcon.Size = new System.Drawing.Size(47, 17);
            this.cIcon.TabIndex = 6;
            this.cIcon.Text = "Icon";
            this.cIcon.UseVisualStyleBackColor = true;
            this.cIcon.CheckedChanged += new System.EventHandler(this.cIcon_CheckedChanged);
            // 
            // cNative
            // 
            this.cNative.AutoSize = true;
            this.cNative.Location = new System.Drawing.Point(121, 110);
            this.cNative.Name = "cNative";
            this.cNative.Size = new System.Drawing.Size(57, 17);
            this.cNative.TabIndex = 7;
            this.cNative.Text = "Native";
            this.cNative.UseVisualStyleBackColor = true;
            this.cNative.CheckedChanged += new System.EventHandler(this.cNative_CheckedChanged);
            // 
            // bCrypt
            // 
            this.bCrypt.Location = new System.Drawing.Point(47, 162);
            this.bCrypt.Name = "bCrypt";
            this.bCrypt.Size = new System.Drawing.Size(316, 84);
            this.bCrypt.TabIndex = 8;
            this.bCrypt.Text = "Crypt";
            this.bCrypt.UseVisualStyleBackColor = true;
            this.bCrypt.Click += new System.EventHandler(this.bCrypt_Click);
            // 
            // cOutVersion
            // 
            this.cOutVersion.FormattingEnabled = true;
            this.cOutVersion.Items.AddRange(new object[] {
            "v2.0",
            "v3.0",
            "v3.5",
            "v4.0"});
            this.cOutVersion.Location = new System.Drawing.Point(314, 110);
            this.cOutVersion.Name = "cOutVersion";
            this.cOutVersion.Size = new System.Drawing.Size(45, 21);
            this.cOutVersion.TabIndex = 9;
            this.cOutVersion.Text = "v4.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(282, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Ver:";
            // 
            // tArgs
            // 
            this.tArgs.Location = new System.Drawing.Point(70, 84);
            this.tArgs.Name = "tArgs";
            this.tArgs.Size = new System.Drawing.Size(289, 20);
            this.tArgs.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Arguments:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 256);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tArgs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cOutVersion);
            this.Controls.Add(this.bCrypt);
            this.Controls.Add(this.cNative);
            this.Controls.Add(this.cIcon);
            this.Controls.Add(this.bIcon);
            this.Controls.Add(this.bFile);
            this.Controls.Add(this.tIcon);
            this.Controls.Add(this.tFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tFile;
        private System.Windows.Forms.TextBox tIcon;
        private System.Windows.Forms.Button bFile;
        private System.Windows.Forms.Button bIcon;
        private System.Windows.Forms.CheckBox cIcon;
        private System.Windows.Forms.CheckBox cNative;
        private System.Windows.Forms.Button bCrypt;
        private System.Windows.Forms.ComboBox cOutVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tArgs;
        private System.Windows.Forms.Label label4;
    }
}

