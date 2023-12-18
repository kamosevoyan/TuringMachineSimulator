using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class SimulatorForm : Form
    {
        CompilerForm parent;
        public Button[] buttons;
        public bool isContinuouslyRunning = false;
        public bool isContinuouslyRunningStopped = false;

        public Thread continuousSimulationThread;

        public SimulatorForm(CompilerForm parent)
        {
            InitializeComponent();
            this.parent = parent;

            this.buttons = new Button[]
            {
                this.button4,
                this.button5,
                this.button6,
                this.button7,
                this.button8,
                this.button9,
                this.button10,
                this.button11,
                this.button12,
                this.button13,
                this.button14,
                this.button15,
                this.button16,
                this.button17,
                this.button18,
                this.button19
        };
            this.continuousSimulationThread = new Thread(this.ContinousSimulation);
            this.continuousSimulationThread.Start();
        }
        private async void ContinousSimulation()
        {
            while (true)
            {
                if (isContinuouslyRunningStopped)
                {
                    break;
                }
                bool isContinuing;
                if (!isContinuouslyRunning)
                {
                    continue;
                }
                else
                {
                    isContinuing = this.parent.StepSimulator();
                    this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                    await Task.Delay(200);
                    if (!isContinuing)
                    {
                        this.FinishSimulation();
                        break;
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {

            if (this.textBox1.Text.Length == 0)
            {
                return;
            }

            this.parent.setSimulatorInput(this.textBox1.Text);
            this.inputSetButton.Enabled = false;
            this.EnableStepButtons();
            string layout = this.parent.simulator.GetLayout();
            this.parent.setTape(layout);
        }

        private void singleStepButton_Click(object sender, EventArgs e)
        {
            bool isContinuing = this.parent.StepSimulator();
            this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";

            if (!isContinuing)
            {
                this.FinishSimulation();
            }
        }

        public void Initialize()
        {
            this.DisableStepButtons();
            this.inputSetButton.Enabled = true;
            this.positionText.Text = $"Head position: {0}";            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void DisableStepButtons()
        {
            this.singleStepButton.Enabled = false;
            this.continiousStepButton.Enabled = false;
        }

        public void EnableStepButtons()
        {
            this.singleStepButton.Enabled = true;
            this.continiousStepButton.Enabled = true;
        }
        private void continuousStepButton_Click(object sender, EventArgs e)
        {
            if (isContinuouslyRunning)
            {
                this.continiousStepButton.Text = "⏩";
                isContinuouslyRunning = false;
                this.singleStepButton.Enabled = true;
            }
            else
            {
                isContinuouslyRunning = true;
                this.continiousStepButton.Text = "⏸";
                this.singleStepButton.Enabled = false;
            }
        }

        private void FinishSimulation()
        {
            this.continiousStepButton.Text = "⏩";
            this.DisableStepButtons();
            MessageBox.Show("Finish");
            this.inputSetButton.Enabled = true;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.parent.Show();
            e.Cancel = true;
        }

        private void positionText_TextChanged(object sender, EventArgs e)
        {

        }

        private void SimulatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
