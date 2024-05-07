using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class CompilerForm : Form
    {
        readonly int _globalWidth;
        readonly int _globalHeight;
        string _compiledSource;
        readonly SimulatorForm _simulatorForm;
        readonly Compiler compiler;

        public CompilerForm()
        {
            InitializeComponent();

            codeTextBox.Font = new Font("Arial", 12);

            _globalWidth = Screen.PrimaryScreen.Bounds.Width;
            _globalHeight = Screen.PrimaryScreen.Bounds.Height;

            Width = _globalWidth * 4 / 5;
            Height = _globalHeight * 3 / 4;

            int relativeHeight = _globalHeight;
            int relativeWidth = _globalWidth;

            codeTextBox.Size = new System.Drawing.Size(relativeWidth, relativeHeight);
            compiler = new Compiler();
            _simulatorForm = new SimulatorForm(this);
            simulator = new Simulator();
            simulateToolStripMenuItem.Enabled = false;
        }

        public Simulator simulator;
        public string GlobalSymbols
        {
            get => compiler.GlobalSymbols;
        }

        public void VisualizeResult()
        {
            string layout = simulator.GetLayout();
            SetTape(layout);
        }
        public Simulator.MachineState StepSimulator(bool visualize)
        {
            Simulator.MachineState result = simulator.Step();
            if (visualize)
            {
                VisualizeResult();
            }
            return result;
        }

        public void SetTape(string values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                _simulatorForm.Buttons[i].Text = values[i].ToString();
            }
        }
        public void ResetVisualizationTape()
        {
            int n = _simulatorForm.Buttons.Length;
            for (int i = 0; i < n; i++)
            {
                _simulatorForm.Buttons[i].Text = "";
            }

        }
        public void SetSimulatorInput(string input)
        {
            simulator.SetInput(input);
        }
        void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void Form1_Load(object sender, EventArgs e)
        {          
            codeTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
        }

        void Form1_ResizeBegin(object sender, EventArgs e)
        {
            int currentHeight = Height;            

            codeTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
            logTextBox.Size = new System.Drawing.Size(Width * 9 / 10, Height * 8 / 10);
            logTextBox.Location = new System.Drawing.Point(8, currentHeight * 17 / 20);

        }

        void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Do you want to create a new file?";
            DialogResult dialogResult = MessageBox.Show(message, "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                codeTextBox.Text = "";
            }

        }

        void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simulateToolStripMenuItem.Enabled = false;
            string source = codeTextBox.Text;

            if (source.Length == 0)
            {
                return;
            }

            try
            {
                compiler.SetStream(source);
                _compiledSource = compiler.Compile();
            }
            catch (SyntaxErrorException ex)
            {
                logTextBox.Text = ex.Message;
                return;
            }
            logTextBox.Text = "Compiled successfully";

            simulateToolStripMenuItem.Enabled = true;
        }

        void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_compiledSource == null)
            {
                return;
            }

            ResetVisualizationTape();
            simulator.Reset();

            simulator.SetConfiguration(_compiledSource);
            _simulatorForm.Initialize();

            Hide();
            _simulatorForm.Show();
        }

        void codeTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open File",
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                StreamReader sr = new StreamReader(selectedFilePath);
                codeTextBox.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = saveFileDialog.FileName;
                StreamWriter sw = new StreamWriter(selectedFilePath);
                sw.Write(codeTextBox.Text);
                sw.Close();
            }
        }
        
    }
}

