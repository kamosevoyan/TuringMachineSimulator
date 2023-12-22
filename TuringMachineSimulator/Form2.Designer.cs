namespace TuringMachineSimulator
{
    partial class SimulatorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulatorForm));
            this.singleStepButton = new System.Windows.Forms.Button();
            this.continiousStepButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.inputSetButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.positionText = new System.Windows.Forms.TextBox();
            this.simulationTimer = new System.Windows.Forms.Timer(this.components);
            this.continuousSimulationTimerInterval = new System.Windows.Forms.NumericUpDown();
            this.instantaneousEvaluateButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.continuousSimulationTimerInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // singleStepButton
            // 
            this.singleStepButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.singleStepButton.Location = new System.Drawing.Point(655, 334);
            this.singleStepButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.singleStepButton.Name = "singleStepButton";
            this.singleStepButton.Size = new System.Drawing.Size(47, 42);
            this.singleStepButton.TabIndex = 1;
            this.singleStepButton.Text = "⏯";
            this.singleStepButton.UseCompatibleTextRendering = true;
            this.singleStepButton.UseVisualStyleBackColor = true;
            this.singleStepButton.Click += new System.EventHandler(this.SingleStepButton_Click);
            this.singleStepButton.MouseHover += new System.EventHandler(this.singleStepButton_MouseHover);
            // 
            // continiousStepButton
            // 
            this.continiousStepButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.continiousStepButton.Location = new System.Drawing.Point(708, 334);
            this.continiousStepButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.continiousStepButton.Name = "continiousStepButton";
            this.continiousStepButton.Size = new System.Drawing.Size(47, 42);
            this.continiousStepButton.TabIndex = 2;
            this.continiousStepButton.Text = "⏩";
            this.continiousStepButton.UseCompatibleTextRendering = true;
            this.continiousStepButton.UseVisualStyleBackColor = true;
            this.continiousStepButton.Click += new System.EventHandler(this.ContinuousStepButton_Click);
            this.continiousStepButton.MouseHover += new System.EventHandler(this.continiousStepButton_MouseHover);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(399, 121);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(51, 74);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(11, 216);
            this.button4.Margin = new System.Windows.Forms.Padding(0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(44, 36);
            this.button4.TabIndex = 4;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(60, 216);
            this.button5.Margin = new System.Windows.Forms.Padding(0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(44, 36);
            this.button5.TabIndex = 4;
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(110, 216);
            this.button6.Margin = new System.Windows.Forms.Padding(0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(44, 36);
            this.button6.TabIndex = 4;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(160, 216);
            this.button7.Margin = new System.Windows.Forms.Padding(0);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(44, 36);
            this.button7.TabIndex = 4;
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Location = new System.Drawing.Point(210, 216);
            this.button8.Margin = new System.Windows.Forms.Padding(0);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(44, 36);
            this.button8.TabIndex = 4;
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(260, 216);
            this.button9.Margin = new System.Windows.Forms.Padding(0);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(44, 36);
            this.button9.TabIndex = 4;
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Enabled = false;
            this.button10.Location = new System.Drawing.Point(309, 216);
            this.button10.Margin = new System.Windows.Forms.Padding(0);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(44, 36);
            this.button10.TabIndex = 4;
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Enabled = false;
            this.button11.Location = new System.Drawing.Point(359, 216);
            this.button11.Margin = new System.Windows.Forms.Padding(0);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(44, 36);
            this.button11.TabIndex = 4;
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Enabled = false;
            this.button12.Location = new System.Drawing.Point(409, 216);
            this.button12.Margin = new System.Windows.Forms.Padding(0);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(44, 36);
            this.button12.TabIndex = 5;
            this.button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            this.button13.Enabled = false;
            this.button13.Location = new System.Drawing.Point(459, 216);
            this.button13.Margin = new System.Windows.Forms.Padding(0);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(44, 36);
            this.button13.TabIndex = 6;
            this.button13.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            this.button14.Enabled = false;
            this.button14.Location = new System.Drawing.Point(508, 216);
            this.button14.Margin = new System.Windows.Forms.Padding(0);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(44, 36);
            this.button14.TabIndex = 7;
            this.button14.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            this.button15.Enabled = false;
            this.button15.Location = new System.Drawing.Point(558, 216);
            this.button15.Margin = new System.Windows.Forms.Padding(0);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(44, 36);
            this.button15.TabIndex = 8;
            this.button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Enabled = false;
            this.button16.Location = new System.Drawing.Point(608, 216);
            this.button16.Margin = new System.Windows.Forms.Padding(0);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(44, 36);
            this.button16.TabIndex = 9;
            this.button16.UseVisualStyleBackColor = true;
            // 
            // button17
            // 
            this.button17.Enabled = false;
            this.button17.Location = new System.Drawing.Point(658, 216);
            this.button17.Margin = new System.Windows.Forms.Padding(0);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(44, 36);
            this.button17.TabIndex = 10;
            this.button17.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            this.button18.Enabled = false;
            this.button18.Location = new System.Drawing.Point(708, 216);
            this.button18.Margin = new System.Windows.Forms.Padding(0);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(44, 36);
            this.button18.TabIndex = 11;
            this.button18.UseVisualStyleBackColor = true;
            // 
            // button19
            // 
            this.button19.Enabled = false;
            this.button19.Location = new System.Drawing.Point(758, 216);
            this.button19.Margin = new System.Windows.Forms.Padding(0);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(44, 36);
            this.button19.TabIndex = 12;
            this.button19.UseVisualStyleBackColor = true;
            // 
            // inputSetButton
            // 
            this.inputSetButton.Location = new System.Drawing.Point(301, 331);
            this.inputSetButton.Margin = new System.Windows.Forms.Padding(0);
            this.inputSetButton.Name = "inputSetButton";
            this.inputSetButton.Size = new System.Drawing.Size(70, 29);
            this.inputSetButton.TabIndex = 13;
            this.inputSetButton.Text = "Set";
            this.inputSetButton.UseVisualStyleBackColor = true;
            this.inputSetButton.Click += new System.EventHandler(this.Button20_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 334);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.MaxLength = 30;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(288, 22);
            this.textBox1.TabIndex = 14;
            this.textBox1.Tag = "";
            // 
            // positionText
            // 
            this.positionText.Location = new System.Drawing.Point(11, 10);
            this.positionText.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.positionText.Name = "positionText";
            this.positionText.ReadOnly = true;
            this.positionText.Size = new System.Drawing.Size(126, 22);
            this.positionText.TabIndex = 16;
            // 
            // simulationTimer
            // 
            this.simulationTimer.Enabled = true;
            this.simulationTimer.Tick += new System.EventHandler(this.SimulationTimer_Tick);
            // 
            // continuousSimulationTimerInterval
            // 
            this.continuousSimulationTimerInterval.Location = new System.Drawing.Point(732, 19);
            this.continuousSimulationTimerInterval.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.continuousSimulationTimerInterval.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.continuousSimulationTimerInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.continuousSimulationTimerInterval.Name = "continuousSimulationTimerInterval";
            this.continuousSimulationTimerInterval.Size = new System.Drawing.Size(56, 22);
            this.continuousSimulationTimerInterval.TabIndex = 17;
            this.continuousSimulationTimerInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.continuousSimulationTimerInterval.ValueChanged += new System.EventHandler(this.ContinuousSimulationTimerInterval_ValueChanged);
            // 
            // instantaneousEvaluateButton
            // 
            this.instantaneousEvaluateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instantaneousEvaluateButton.Location = new System.Drawing.Point(758, 334);
            this.instantaneousEvaluateButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.instantaneousEvaluateButton.Name = "instantaneousEvaluateButton";
            this.instantaneousEvaluateButton.Size = new System.Drawing.Size(47, 42);
            this.instantaneousEvaluateButton.TabIndex = 18;
            this.instantaneousEvaluateButton.Text = "⏭️";
            this.instantaneousEvaluateButton.UseCompatibleTextRendering = true;
            this.instantaneousEvaluateButton.UseVisualStyleBackColor = true;
            this.instantaneousEvaluateButton.Click += new System.EventHandler(this.InstantaneousEvaluateButton_Click);
            this.instantaneousEvaluateButton.MouseHover += new System.EventHandler(this.instantaneousEvaluateButton_MouseHover);
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(813, 384);
            this.Controls.Add(this.instantaneousEvaluateButton);
            this.Controls.Add(this.continuousSimulationTimerInterval);
            this.Controls.Add(this.positionText);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.inputSetButton);
            this.Controls.Add(this.button19);
            this.Controls.Add(this.button18);
            this.Controls.Add(this.button17);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.continiousStepButton);
            this.Controls.Add(this.singleStepButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "SimulatorForm";
            this.Text = "Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimulatorForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.continuousSimulationTimerInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button singleStepButton;
        private System.Windows.Forms.Button continiousStepButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button inputSetButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox positionText;
        private System.Windows.Forms.Timer simulationTimer;
        private System.Windows.Forms.NumericUpDown continuousSimulationTimerInterval;
        private System.Windows.Forms.Button instantaneousEvaluateButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}