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
        private bool isContinuing;

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
            this.continuousSimulationTimerInterval.Value = this.simulationTimer.Interval;
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

        private void StepWrapper(bool visualize)
        {
            try
            {
                this.isContinuing = this.parent.StepSimulator(visualize);
                if (visualize)
                {
                    this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                }
            }
            catch (SimulatorErrorException ex) 
            {
                this.isContinuing = false;
            }

        }
        private void singleStepButton_Click(object sender, EventArgs e)
        {
            this.StepWrapper(visualize:true);

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
            this.instantaneousEvaluateButton.Enabled = false;
        }

        public void EnableStepButtons()
        {
            this.singleStepButton.Enabled = true;
            this.continiousStepButton.Enabled = true;
            this.instantaneousEvaluateButton.Enabled = true;
        }
        private void continuousStepButton_Click(object sender, EventArgs e)
        {
            if (isContinuouslyRunning)
            {
                this.continiousStepButton.Text = "⏩";
                isContinuouslyRunning = false;
                this.singleStepButton.Enabled = true;
                this.instantaneousEvaluateButton.Enabled = true;
            }
            else
            {
                isContinuouslyRunning = true;
                this.continiousStepButton.Text = "⏸";
                this.singleStepButton.Enabled = false;
                this.instantaneousEvaluateButton.Enabled = false;
            }
        }

        public void FinishSimulation()
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

        private void simulationTimer_Tick(object sender, EventArgs e)
        {
            if (this.isContinuouslyRunning)
            {
                this.StepWrapper(visualize:true);

                if (!isContinuing)
                {
                    this.isContinuouslyRunning = false;
                    this.FinishSimulation();
                }
            }
        }

        private void continuousSimulationTimerInterval_ValueChanged(object sender, EventArgs e)
        {
            this.simulationTimer.Interval = (int) this.continuousSimulationTimerInterval.Value;
        }

        private async void instantaneousEvaluateButton_Click(object sender, EventArgs e)
        {
            this.singleStepButton.Enabled = false;
            this.continiousStepButton.Enabled = false;

            await Task.Run(() =>
            {
                while (true)
                {
                    this.StepWrapper(visualize:false);
                    if (!this.isContinuing)
                    {
                        break;
                    }
                }
                this.parent.VisualizeResult();
                this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                this.FinishSimulation();
            });

        }
    }
}
