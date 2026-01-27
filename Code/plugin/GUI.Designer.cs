namespace glass_plugin
{
    partial class GUI
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
            this.components = new System.ComponentModel.Container();
            this.groupBoxParameters = new System.Windows.Forms.GroupBox();
            this.StandRadiusLimitsLabel = new System.Windows.Forms.Label();
            this.SideAngleLimitsLabel = new System.Windows.Forms.Label();
            this.StalkRadiusLimitsLabel = new System.Windows.Forms.Label();
            this.BowlRadiusLimitsLabel = new System.Windows.Forms.Label();
            this.SideHeightLimitsLabel = new System.Windows.Forms.Label();
            this.StalkHeightLimitsLabel = new System.Windows.Forms.Label();
            this.labelHeightUpperEdge = new System.Windows.Forms.Label();
            this.labelThicknessUpperEdge = new System.Windows.Forms.Label();
            this.labelThicknessLowerEdge = new System.Windows.Forms.Label();
            this.labelHeightBottom = new System.Windows.Forms.Label();
            this.StandRadiusTextBox = new System.Windows.Forms.TextBox();
            this.SideAngleTextBox = new System.Windows.Forms.TextBox();
            this.StalkRadiusTextBox = new System.Windows.Forms.TextBox();
            this.BowlRadiusTextBox = new System.Windows.Forms.TextBox();
            this.SideHeightTextBox = new System.Windows.Forms.TextBox();
            this.StalkHeightTextBox = new System.Windows.Forms.TextBox();
            this.labelRadius = new System.Windows.Forms.Label();
            this.labelHeightTotal = new System.Windows.Forms.Label();
            this.CreateButton = new System.Windows.Forms.Button();
            this.ErrorToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ErrorToolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.ErrorToolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.ErrorToolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.ErrorToolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.ErrorToolTip6 = new System.Windows.Forms.ToolTip(this.components);
            this.ResetButton = new System.Windows.Forms.Button();
            this.HandleCheckBox = new System.Windows.Forms.CheckBox();
            this.CalculationsGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AreaLabel = new System.Windows.Forms.Label();
            this.TotalHeightLabel = new System.Windows.Forms.Label();
            this.VolumeLabelMM = new System.Windows.Forms.Label();
            this.VolumeLabelML = new System.Windows.Forms.Label();
            this.groupBoxParameters.SuspendLayout();
            this.CalculationsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxParameters
            // 
            this.groupBoxParameters.Controls.Add(this.StandRadiusLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.SideAngleLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.StalkRadiusLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.BowlRadiusLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.SideHeightLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.StalkHeightLimitsLabel);
            this.groupBoxParameters.Controls.Add(this.labelHeightUpperEdge);
            this.groupBoxParameters.Controls.Add(this.labelThicknessUpperEdge);
            this.groupBoxParameters.Controls.Add(this.labelThicknessLowerEdge);
            this.groupBoxParameters.Controls.Add(this.labelHeightBottom);
            this.groupBoxParameters.Controls.Add(this.StandRadiusTextBox);
            this.groupBoxParameters.Controls.Add(this.SideAngleTextBox);
            this.groupBoxParameters.Controls.Add(this.StalkRadiusTextBox);
            this.groupBoxParameters.Controls.Add(this.BowlRadiusTextBox);
            this.groupBoxParameters.Controls.Add(this.SideHeightTextBox);
            this.groupBoxParameters.Controls.Add(this.StalkHeightTextBox);
            this.groupBoxParameters.Controls.Add(this.labelRadius);
            this.groupBoxParameters.Controls.Add(this.labelHeightTotal);
            this.groupBoxParameters.Location = new System.Drawing.Point(12, 6);
            this.groupBoxParameters.Name = "groupBoxParameters";
            this.groupBoxParameters.Size = new System.Drawing.Size(198, 270);
            this.groupBoxParameters.TabIndex = 1;
            this.groupBoxParameters.TabStop = false;
            this.groupBoxParameters.Text = "Входные параметры";
            // 
            // StandRadiusLimitsLabel
            // 
            this.StandRadiusLimitsLabel.AutoSize = true;
            this.StandRadiusLimitsLabel.Location = new System.Drawing.Point(112, 251);
            this.StandRadiusLimitsLabel.Name = "StandRadiusLimitsLabel";
            this.StandRadiusLimitsLabel.Size = new System.Drawing.Size(58, 13);
            this.StandRadiusLimitsLabel.TabIndex = 18;
            this.StandRadiusLimitsLabel.Text = "20.0 - 75.0";
            // 
            // SideAngleLimitsLabel
            // 
            this.SideAngleLimitsLabel.AutoSize = true;
            this.SideAngleLimitsLabel.Location = new System.Drawing.Point(112, 212);
            this.SideAngleLimitsLabel.Name = "SideAngleLimitsLabel";
            this.SideAngleLimitsLabel.Size = new System.Drawing.Size(52, 13);
            this.SideAngleLimitsLabel.TabIndex = 17;
            this.SideAngleLimitsLabel.Text = "0.0 - 15.0";
            // 
            // StalkRadiusLimitsLabel
            // 
            this.StalkRadiusLimitsLabel.AutoSize = true;
            this.StalkRadiusLimitsLabel.Location = new System.Drawing.Point(112, 173);
            this.StalkRadiusLimitsLabel.Name = "StalkRadiusLimitsLabel";
            this.StalkRadiusLimitsLabel.Size = new System.Drawing.Size(46, 13);
            this.StalkRadiusLimitsLabel.TabIndex = 16;
            this.StalkRadiusLimitsLabel.Text = "1.0 - 3.0";
            // 
            // BowlRadiusLimitsLabel
            // 
            this.BowlRadiusLimitsLabel.AutoSize = true;
            this.BowlRadiusLimitsLabel.Location = new System.Drawing.Point(112, 134);
            this.BowlRadiusLimitsLabel.Name = "BowlRadiusLimitsLabel";
            this.BowlRadiusLimitsLabel.Size = new System.Drawing.Size(58, 13);
            this.BowlRadiusLimitsLabel.TabIndex = 15;
            this.BowlRadiusLimitsLabel.Text = "25.0 - 50.0";
            // 
            // SideHeightLimitsLabel
            // 
            this.SideHeightLimitsLabel.AutoSize = true;
            this.SideHeightLimitsLabel.Location = new System.Drawing.Point(112, 95);
            this.SideHeightLimitsLabel.Name = "SideHeightLimitsLabel";
            this.SideHeightLimitsLabel.Size = new System.Drawing.Size(52, 13);
            this.SideHeightLimitsLabel.TabIndex = 14;
            this.SideHeightLimitsLabel.Text = "0.0 - 50.0";
            // 
            // StalkHeightLimitsLabel
            // 
            this.StalkHeightLimitsLabel.AutoSize = true;
            this.StalkHeightLimitsLabel.Location = new System.Drawing.Point(112, 56);
            this.StalkHeightLimitsLabel.Name = "StalkHeightLimitsLabel";
            this.StalkHeightLimitsLabel.Size = new System.Drawing.Size(64, 13);
            this.StalkHeightLimitsLabel.TabIndex = 13;
            this.StalkHeightLimitsLabel.Text = "50.0 - 100.0";
            // 
            // labelHeightUpperEdge
            // 
            this.labelHeightUpperEdge.AutoSize = true;
            this.labelHeightUpperEdge.Location = new System.Drawing.Point(6, 231);
            this.labelHeightUpperEdge.Name = "labelHeightUpperEdge";
            this.labelHeightUpperEdge.Size = new System.Drawing.Size(103, 13);
            this.labelHeightUpperEdge.TabIndex = 12;
            this.labelHeightUpperEdge.Text = "Радиус основания:";
            // 
            // labelThicknessUpperEdge
            // 
            this.labelThicknessUpperEdge.AutoSize = true;
            this.labelThicknessUpperEdge.Location = new System.Drawing.Point(6, 192);
            this.labelThicknessUpperEdge.Name = "labelThicknessUpperEdge";
            this.labelThicknessUpperEdge.Size = new System.Drawing.Size(86, 13);
            this.labelThicknessUpperEdge.TabIndex = 11;
            this.labelThicknessUpperEdge.Text = "Наклон стенки:";
            // 
            // labelThicknessLowerEdge
            // 
            this.labelThicknessLowerEdge.AutoSize = true;
            this.labelThicknessLowerEdge.Location = new System.Drawing.Point(6, 153);
            this.labelThicknessLowerEdge.Name = "labelThicknessLowerEdge";
            this.labelThicknessLowerEdge.Size = new System.Drawing.Size(81, 13);
            this.labelThicknessLowerEdge.TabIndex = 10;
            this.labelThicknessLowerEdge.Text = "Радиус ножки:";
            // 
            // labelHeightBottom
            // 
            this.labelHeightBottom.AutoSize = true;
            this.labelHeightBottom.Location = new System.Drawing.Point(6, 114);
            this.labelHeightBottom.Name = "labelHeightBottom";
            this.labelHeightBottom.Size = new System.Drawing.Size(74, 13);
            this.labelHeightBottom.TabIndex = 9;
            this.labelHeightBottom.Text = "Радиус чаши:";
            // 
            // StandRadiusTextBox
            // 
            this.StandRadiusTextBox.Location = new System.Drawing.Point(115, 228);
            this.StandRadiusTextBox.Name = "StandRadiusTextBox";
            this.StandRadiusTextBox.Size = new System.Drawing.Size(73, 20);
            this.StandRadiusTextBox.TabIndex = 7;
            this.StandRadiusTextBox.Text = "30";
            this.StandRadiusTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // SideAngleTextBox
            // 
            this.SideAngleTextBox.Location = new System.Drawing.Point(115, 189);
            this.SideAngleTextBox.Name = "SideAngleTextBox";
            this.SideAngleTextBox.Size = new System.Drawing.Size(73, 20);
            this.SideAngleTextBox.TabIndex = 6;
            this.SideAngleTextBox.Text = "10";
            this.SideAngleTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // StalkRadiusTextBox
            // 
            this.StalkRadiusTextBox.Location = new System.Drawing.Point(115, 150);
            this.StalkRadiusTextBox.Name = "StalkRadiusTextBox";
            this.StalkRadiusTextBox.Size = new System.Drawing.Size(73, 20);
            this.StalkRadiusTextBox.TabIndex = 5;
            this.StalkRadiusTextBox.Text = "2";
            this.StalkRadiusTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // BowlRadiusTextBox
            // 
            this.BowlRadiusTextBox.Location = new System.Drawing.Point(115, 111);
            this.BowlRadiusTextBox.Name = "BowlRadiusTextBox";
            this.BowlRadiusTextBox.Size = new System.Drawing.Size(73, 20);
            this.BowlRadiusTextBox.TabIndex = 4;
            this.BowlRadiusTextBox.Text = "35";
            this.BowlRadiusTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // SideHeightTextBox
            // 
            this.SideHeightTextBox.Location = new System.Drawing.Point(115, 72);
            this.SideHeightTextBox.Name = "SideHeightTextBox";
            this.SideHeightTextBox.Size = new System.Drawing.Size(73, 20);
            this.SideHeightTextBox.TabIndex = 3;
            this.SideHeightTextBox.Text = "40";
            this.SideHeightTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // StalkHeightTextBox
            // 
            this.StalkHeightTextBox.Location = new System.Drawing.Point(115, 33);
            this.StalkHeightTextBox.Name = "StalkHeightTextBox";
            this.StalkHeightTextBox.Size = new System.Drawing.Size(73, 20);
            this.StalkHeightTextBox.TabIndex = 2;
            this.StalkHeightTextBox.Text = "75";
            this.StalkHeightTextBox.TextChanged += new System.EventHandler(this.UnifiedTextChangedHandler);
            // 
            // labelRadius
            // 
            this.labelRadius.AutoSize = true;
            this.labelRadius.Location = new System.Drawing.Point(6, 75);
            this.labelRadius.Name = "labelRadius";
            this.labelRadius.Size = new System.Drawing.Size(86, 13);
            this.labelRadius.TabIndex = 1;
            this.labelRadius.Text = "Высота стенки:";
            // 
            // labelHeightTotal
            // 
            this.labelHeightTotal.AutoSize = true;
            this.labelHeightTotal.Location = new System.Drawing.Point(6, 36);
            this.labelHeightTotal.Name = "labelHeightTotal";
            this.labelHeightTotal.Size = new System.Drawing.Size(83, 13);
            this.labelHeightTotal.TabIndex = 0;
            this.labelHeightTotal.Text = "Высота ножки:";
            // 
            // CreateButton
            // 
            this.CreateButton.Location = new System.Drawing.Point(110, 279);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(98, 35);
            this.CreateButton.TabIndex = 14;
            this.CreateButton.Text = "Построить бокал";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 279);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(92, 35);
            this.ResetButton.TabIndex = 15;
            this.ResetButton.Text = "Сброс";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // HandleCheckBox
            // 
            this.HandleCheckBox.AutoSize = true;
            this.HandleCheckBox.Location = new System.Drawing.Point(217, 13);
            this.HandleCheckBox.Name = "HandleCheckBox";
            this.HandleCheckBox.Size = new System.Drawing.Size(94, 17);
            this.HandleCheckBox.TabIndex = 16;
            this.HandleCheckBox.Text = "Ручка бокала";
            this.HandleCheckBox.UseVisualStyleBackColor = true;
            // 
            // CalculationsGroupBox
            // 
            this.CalculationsGroupBox.Controls.Add(this.AreaLabel);
            this.CalculationsGroupBox.Controls.Add(this.label1);
            this.CalculationsGroupBox.Controls.Add(this.TotalHeightLabel);
            this.CalculationsGroupBox.Controls.Add(this.label2);
            this.CalculationsGroupBox.Controls.Add(this.VolumeLabelMM);
            this.CalculationsGroupBox.Controls.Add(this.label3);
            this.CalculationsGroupBox.Controls.Add(this.VolumeLabelML);
            this.CalculationsGroupBox.Controls.Add(this.label4);
            this.CalculationsGroupBox.Location = new System.Drawing.Point(216, 36);
            this.CalculationsGroupBox.Name = "CalculationsGroupBox";
            this.CalculationsGroupBox.Size = new System.Drawing.Size(200, 117);
            this.CalculationsGroupBox.TabIndex = 17;
            this.CalculationsGroupBox.TabStop = false;
            this.CalculationsGroupBox.Text = "Характеристики бокала";
            this.CalculationsGroupBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Объем бокала (мл):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Объем бокала (мм):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Высота бокала:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Площадь основания:";
            // 
            // AreaLabel
            // 
            this.AreaLabel.AutoSize = true;
            this.AreaLabel.Location = new System.Drawing.Point(126, 96);
            this.AreaLabel.Name = "AreaLabel";
            this.AreaLabel.Size = new System.Drawing.Size(13, 13);
            this.AreaLabel.TabIndex = 24;
            this.AreaLabel.Text = "0";
            // 
            // TotalHeightLabel
            // 
            this.TotalHeightLabel.AutoSize = true;
            this.TotalHeightLabel.Location = new System.Drawing.Point(126, 72);
            this.TotalHeightLabel.Name = "TotalHeightLabel";
            this.TotalHeightLabel.Size = new System.Drawing.Size(13, 13);
            this.TotalHeightLabel.TabIndex = 23;
            this.TotalHeightLabel.Text = "0";
            // 
            // VolumeLabelMM
            // 
            this.VolumeLabelMM.AutoSize = true;
            this.VolumeLabelMM.Location = new System.Drawing.Point(126, 49);
            this.VolumeLabelMM.Name = "VolumeLabelMM";
            this.VolumeLabelMM.Size = new System.Drawing.Size(13, 13);
            this.VolumeLabelMM.TabIndex = 22;
            this.VolumeLabelMM.Text = "0";
            // 
            // VolumeLabelML
            // 
            this.VolumeLabelML.AutoSize = true;
            this.VolumeLabelML.Location = new System.Drawing.Point(126, 26);
            this.VolumeLabelML.Name = "VolumeLabelML";
            this.VolumeLabelML.Size = new System.Drawing.Size(13, 13);
            this.VolumeLabelML.TabIndex = 21;
            this.VolumeLabelML.Text = "0";
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 326);
            this.Controls.Add(this.CalculationsGroupBox);
            this.Controls.Add(this.HandleCheckBox);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.groupBoxParameters);
            this.MaximumSize = new System.Drawing.Size(443, 365);
            this.MinimumSize = new System.Drawing.Size(443, 365);
            this.Name = "GUI";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Построитель гранёного стакана";
            this.Load += new System.EventHandler(this.GUI_Load);
            this.groupBoxParameters.ResumeLayout(false);
            this.groupBoxParameters.PerformLayout();
            this.CalculationsGroupBox.ResumeLayout(false);
            this.CalculationsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxParameters;
        private System.Windows.Forms.TextBox StalkRadiusTextBox;
        private System.Windows.Forms.TextBox BowlRadiusTextBox;
        private System.Windows.Forms.TextBox SideHeightTextBox;
        private System.Windows.Forms.TextBox StalkHeightTextBox;
        private System.Windows.Forms.Label labelRadius;
        private System.Windows.Forms.Label labelHeightTotal;
        private System.Windows.Forms.Label labelThicknessLowerEdge;
        private System.Windows.Forms.Label labelHeightBottom;
        private System.Windows.Forms.TextBox StandRadiusTextBox;
        private System.Windows.Forms.TextBox SideAngleTextBox;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Label labelHeightUpperEdge;
        private System.Windows.Forms.Label labelThicknessUpperEdge;
        private System.Windows.Forms.Label StandRadiusLimitsLabel;
        private System.Windows.Forms.Label SideAngleLimitsLabel;
        private System.Windows.Forms.Label StalkRadiusLimitsLabel;
        private System.Windows.Forms.Label BowlRadiusLimitsLabel;
        private System.Windows.Forms.Label SideHeightLimitsLabel;
        private System.Windows.Forms.Label StalkHeightLimitsLabel;
        private System.Windows.Forms.ToolTip ErrorToolTip1;
        private System.Windows.Forms.ToolTip ErrorToolTip2;
        private System.Windows.Forms.ToolTip ErrorToolTip3;
        private System.Windows.Forms.ToolTip ErrorToolTip4;
        private System.Windows.Forms.ToolTip ErrorToolTip5;
        private System.Windows.Forms.ToolTip ErrorToolTip6;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.CheckBox HandleCheckBox;
        private System.Windows.Forms.GroupBox CalculationsGroupBox;
        private System.Windows.Forms.Label AreaLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TotalHeightLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label VolumeLabelMM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label VolumeLabelML;
        private System.Windows.Forms.Label label4;
    }
}

