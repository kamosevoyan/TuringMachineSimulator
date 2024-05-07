using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class SimulatorForm : Form
    {
        readonly CompilerForm _parent;
        Simulator.MachineState _simulatorState;

        public SimulatorForm(CompilerForm parent)
        {
            InitializeComponent();
            _parent = parent;

            Buttons = new Button[]
            {
                button4,
                button5,
                button6,
                button7,
                button8,
                button9,
                button10,
                button11,
                button12,
                button13,
                button14,
                button15,
                button16,
                button17,
                button18,
                button19
        };
            continuousSimulationTimerInterval.Value = simulationTimer.Interval;
        }

        public Button[] Buttons {get; set;}
        public bool IsContinuouslyRunning { get; set; } = false;
        public void Initialize()
        {
            DisableStepButtons();
            inputSetButton.Enabled = true;
            IsContinuouslyRunning = false;
            positionText.Text = $"Head position: {0}";
            numStepsText.Text = $"Total steps: {0}";
            continiousStepButton.Text = "⏩";
        }

        public void DisableStepButtons()
        {
            singleStepButton.Enabled = false;
            continiousStepButton.Enabled = false;
            instantaneousEvaluateButton.Enabled = false;
        }

        public void EnableStepButtons()
        {
            singleStepButton.Enabled = true;
            continiousStepButton.Enabled = true;
            instantaneousEvaluateButton.Enabled = true;
        }
        public void FinishSimulation()
        {
            continiousStepButton.Text = "⏩";
            DisableStepButtons();
            MessageBox.Show("Finish");
            inputSetButton.Enabled = true;
            IsContinuouslyRunning = false;
        }

        bool IsInputValid(string input)
        {
            foreach (char ch in input)
            {
                if (!_parent.GlobalSymbols.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        void Button20_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Length == 0) || (!IsInputValid(textBox1.Text)))
            {
                return;
            }

            _parent.SetSimulatorInput(textBox1.Text);
            inputSetButton.Enabled = false;
            EnableStepButtons();
            string layout = _parent.simulator.GetLayout();
            _parent.SetTape(layout);
        }

        void StepWrapper(bool visualize)
        {

            _simulatorState = _parent.StepSimulator(visualize);
            if (visualize)
            {
                positionText.Text = $"Head position: {_parent.simulator.tape.Position}";
                numStepsText.Text = $"Total steps: {_parent.simulator.NumSteps}";
            }

            if (_simulatorState == Simulator.MachineState.Failed)
            {
                continiousStepButton.Text = "⏩";
                DisableStepButtons();
                inputSetButton.Enabled = true;
                IsContinuouslyRunning = false;
                MessageBox.Show("The simulator terminated with error.");
            }

        }
        void SingleStepButton_Click(object sender, EventArgs e)
        {
            StepWrapper(visualize: true);

            if (_simulatorState == Simulator.MachineState.Terminated)
            {
                FinishSimulation();
            }
        }

        void ContinuousStepButton_Click(object sender, EventArgs e)
        {
            if (IsContinuouslyRunning)
            {
                continiousStepButton.Text = "⏩";
                IsContinuouslyRunning = false;
                singleStepButton.Enabled = true;
                instantaneousEvaluateButton.Enabled = true;
            }
            else
            {
                IsContinuouslyRunning = true;
                continiousStepButton.Text = "⏸";
                singleStepButton.Enabled = false;
                instantaneousEvaluateButton.Enabled = false;
            }
        }


        void SimulatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            _parent.Show();
            e.Cancel = true;
        }

        void SimulationTimer_Tick(object sender, EventArgs e)
        {
            if (IsContinuouslyRunning)
            {
                StepWrapper(visualize: true);

                if (_simulatorState == Simulator.MachineState.Terminated)
                {
                    IsContinuouslyRunning = false;
                    FinishSimulation();
                }
            }
        }

        void ContinuousSimulationTimerInterval_ValueChanged(object sender, EventArgs e)
        {
            simulationTimer.Interval = (int)continuousSimulationTimerInterval.Value;
        }

        async void InstantaneousEvaluateButton_Click(object sender, EventArgs e)
        {
            singleStepButton.Enabled = false;
            continiousStepButton.Enabled = false;

            await Task.Run(() =>
            {
                while (true)
                {
                    StepWrapper(visualize: false);
                    if (_simulatorState == Simulator.MachineState.Terminated)
                    {
                        break;
                    }
                }
                _parent.VisualizeResult();
                positionText.Text = $"Head position: {_parent.simulator.tape.Position}";
                numStepsText.Text = $"Total steps: {_parent.simulator.NumSteps}";
                FinishSimulation();
            });

        }

        void singleStepButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Single step", singleStepButton);
        }

        void continiousStepButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Self running", continiousStepButton);
        }

        void instantaneousEvaluateButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Instantaneous run", instantaneousEvaluateButton);
        }

        void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        void SimulatorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
