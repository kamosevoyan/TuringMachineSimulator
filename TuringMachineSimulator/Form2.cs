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
        private bool isRunning = false;
        private bool isStopped;

        Thread simulationThread;
        Task MyTask;

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
            MyTask = MyAsyncMethod();
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

        private void button2_Click(object sender, EventArgs e)
        {
            bool isContinuing = this.parent.step();
            this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";

            if (!isContinuing)
            {
                this.FinishSimulation();
            }
        }

        public void Initialize()
        {
            //this.simulationThread = new Thread(SimulationLoop);
            //this.simulationThread.Start();            
            this.isStopped = false;
            this.DisableStepButtons();
            this.inputSetButton.Enabled = true;
            this.positionText.Text = $"Head position: {0}";            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void DisableStepButtons()
        {
            this.singleStep.Enabled = false;
            this.continiousStep.Enabled = false;
        }

        public void EnableStepButtons()
        {
            this.singleStep.Enabled = true;
            this.continiousStep.Enabled = true;
        }
        private void continuousStep_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                this.continiousStep.Text = "⏩";
                isRunning = false;
                this.singleStep.Enabled = true;
            }
            else
            {
                isRunning = true;
                this.continiousStep.Text = "⏸";
                this.singleStep.Enabled = false;
            }
        }

        async Task MyAsyncMethod()
        {
            await Task.Run(() => SimulationLoop());
        }


        async private void SimulationLoop()
        {
            bool isContinuing;
            while (true)
            {
                if (!isRunning) 
                {
                    continue;
                }
                isContinuing = this.parent.step();
                this.positionText.Text = $"Head position: {this.parent.simulator.tape.position}";
                await Task.Delay(200);
                if (!isContinuing || isStopped)
                {
                    this.FinishSimulation();
                    break;
                }
            }
        }

        private void FinishSimulation()
        {
            this.continiousStep.Text = "⏩";
            this.DisableStepButtons();
            MessageBox.Show("Finish");
            this.inputSetButton.Enabled = true;
        }

        private async void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.parent.Show();
            e.Cancel = true;
            this.isStopped = true;
            //this.simulationThread.Join();
            await this.MyTask;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.EnableStepButtons();
            this.isRunning = false;
        }

        private void positionText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
