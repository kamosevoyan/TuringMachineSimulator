using System;
using System.Drawing;
using System.IO;

using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class CompilerForm : Form
    {
        private int globalWidth;
        private int globalHeight;
        private SimulatorForm simulatorForm;

        public Simulator simulator;
        Compiler compiler;

        string compiledSource;

        public CompilerForm()
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

            this.simulatorForm = new SimulatorForm(this);

            this.simulator = new Simulator();

            this.simulateToolStripMenuItem.Enabled = false;
        }

        public void VisualizeResult()
        {
            string layout = simulator.GetLayout();
            this.setTape(layout);
        }
        public bool StepSimulator(bool visualize=true)
        {
            bool result = simulator.Step();
            if (visualize)
            {
                VisualizeResult();
            }
            return result;
        }

        public void setTape(string values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                this.simulatorForm.buttons[i].Text = values[i].ToString();
            }
        }
        public void ResetVisualizationTape()
        {
            int n = this.simulatorForm.buttons.Length;
            for (int i = 0; i < n; i++)
            {
                this.simulatorForm.buttons[i].Text = "";
            }

        }
        public void setSimulatorInput(string input)
        {
            this.simulator.SetInput(input);
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
            this.logTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
            this.logTextBox.Location = new System.Drawing.Point(8, currentHeight * 17 / 20);

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to create a new file?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                this.codeTextBox.Text = "";
            }

        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.simulateToolStripMenuItem.Enabled = false;
            string source = this.codeTextBox.Text;

            if (source.Length == 0)
            {
                return;
            }

            try
            {
                compiler.SetStream(source);
                this.compiledSource = compiler.Compile();
            }
            catch (SyntaxErrorException ex)
            {
                this.logTextBox.Text = ex.Message;
                return;
            }
            this.logTextBox.Text = "Compiled successfully";

            this.simulateToolStripMenuItem.Enabled = true;
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.compiledSource == null) //TODO check 
            {
                return;
            }

            this.ResetVisualizationTape();
            this.simulator.Reset();

            this.simulator.SetConfiguration(this.compiledSource);
            this.simulatorForm.Initialize();

            this.Hide();
            this.simulatorForm.Show();
        }

        private void codeTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open File";
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                StreamReader sr = new StreamReader(selectedFilePath);
                this.codeTextBox.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Save File";
            saveFileDialog.Filter = "All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = saveFileDialog.FileName;
                StreamWriter sw = new StreamWriter(selectedFilePath);
                sw.Write(this.codeTextBox.Text);
                sw.Close();
            }
        }

        private void CompilerForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void CompilerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
          
        }
    }
}

