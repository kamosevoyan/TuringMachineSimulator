using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class Form1 : Form
    {
        private int globalWidth;
        private int globalHeight;
        private Form2 simulatorForm;

        public Simulator simulator;
        Compiler compiler;

        string compiledSource;

        public Form1()
        {
            InitializeComponent();

            this.codeTextBox.Font = new Font("Arial", 12);

            this.globalWidth = Screen.PrimaryScreen.Bounds.Width;
            this.globalHeight = Screen.PrimaryScreen.Bounds.Height;

            this.Width = globalWidth * 4 / 5;
            this.Height = globalHeight * 3 / 4;

            int relativeHeight = globalHeight * 10 / 10;
            int relativeWidth = globalWidth * 10 / 10;

            this.codeTextBox.Size = new System.Drawing.Size(relativeWidth, relativeHeight);

            this.compiler = new Compiler();

            this.simulatorForm = new Form2(this);

            this.simulator = new Simulator();
        }

        public bool step()
        {
            bool result = simulator.step();
            string layout = simulator.getLayout();
            this.setTape(layout);

            return result;
        }

        public void setTape(string values)
        {
            for (int i = 0; i < values.Length; i++) 
            {
                this.simulatorForm.buttons[i].Text = values[i].ToString();
            }
        }
        public void resetTape()
        {
            int n = this.simulatorForm.buttons.Length;
            for (int i = 0; i < n; i++)
            {
                this.simulatorForm.buttons[i].Text = this.simulator.emptySymbol.ToString();
            }

        }
        public void setSimulatorInput(string input)
        {
            //TODO: add checking here
            this.simulator.setInput(input);
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int currentHeight = this.Height;
            int currentWidth = this.Width;

            this.codeTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            int currentHeight = this.Height;
            int currentWidth = this.Width;

            this.codeTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to create a new file?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes) 
            {
                this.codeTextBox.Text = "";
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string source = this.codeTextBox.Text;

            if (source.Length == 0)
            {
                return;
            }

            compiler.setStream(source);
            this.compiledSource = compiler.compile();
            //this.codeTextBox.Text = this.compiledSource;
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.compiledSource == null) //TODO check 
            {
                return;
            }

            this.resetTape();
            this.simulatorForm.Show();
            this.simulator.setConfiguration(this.compiledSource);
        }

        private void codeTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
