using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class SimulatorForm : Form
    {
        private CompilerForm parent;
        private Simulator.MachineState simulatorState;

        public Button[] buttons;
        public bool isContinuouslyRunning = false;

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

        private bool IsInputValid(string input)
        {
            foreach (char ch in input)
            {
                if (!this.parent.GlobalSymbols.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            if ((this.textBox1.Text.Length == 0) || (!IsInputValid(this.textBox1.Text)))
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

            this.simulatorState = this.parent.StepSimulator(visualize);
            if (visualize)
            {
                this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                this.numStepsText.Text = $"Total steps: {this.parent.simulator.NumSteps}";
            }

            if (this.simulatorState == Simulator.MachineState.Failed)
            {
                this.continiousStepButton.Text = "⏩";
                this.DisableStepButtons();
                this.inputSetButton.Enabled = true;
                this.isContinuouslyRunning = false;
                MessageBox.Show("The simulator terminated with error.");
            }

        }
        private void SingleStepButton_Click(object sender, EventArgs e)
        {
            this.StepWrapper(visualize: true);

            if (this.simulatorState == Simulator.MachineState.Terminated)
            {
                this.FinishSimulation();
            }
        }

        public void Initialize()
        {
            this.DisableStepButtons();
            this.inputSetButton.Enabled = true;
            this.isContinuouslyRunning = false;
            this.positionText.Text = $"Head position: {0}";
            this.numStepsText.Text = $"Total steps: {0}";
            this.continiousStepButton.Text = "⏩";
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
        private void ContinuousStepButton_Click(object sender, EventArgs e)
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
            this.isContinuouslyRunning = false;
        }

        private void SimulatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.parent.Show();
            e.Cancel = true;
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            if (this.isContinuouslyRunning)
            {
                this.StepWrapper(visualize: true);

                if (this.simulatorState == Simulator.MachineState.Terminated)
                {
                    this.isContinuouslyRunning = false;
                    this.FinishSimulation();
                }
            }
        }

        private void ContinuousSimulationTimerInterval_ValueChanged(object sender, EventArgs e)
        {
            this.simulationTimer.Interval = (int)this.continuousSimulationTimerInterval.Value;
        }

        private async void InstantaneousEvaluateButton_Click(object sender, EventArgs e)
        {
            this.singleStepButton.Enabled = false;
            this.continiousStepButton.Enabled = false;

            await Task.Run(() =>
            {
                while (true)
                {
                    this.StepWrapper(visualize: false);
                    if (this.simulatorState == Simulator.MachineState.Terminated)
                    {
                        break;
                    }
                }
                this.parent.VisualizeResult();
                this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                this.numStepsText.Text = $"Total steps: {this.parent.simulator.NumSteps}";
                this.FinishSimulation();
            });

        }

        private void singleStepButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Single step", this.singleStepButton);
        }

        private void continiousStepButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Self running", this.continiousStepButton);
        }

        private void instantaneousEvaluateButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Instantaneous run", this.instantaneousEvaluateButton);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
