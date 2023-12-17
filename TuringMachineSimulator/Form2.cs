using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public partial class Form2 : Form
    {
        Form1 parent;
        public Button[] buttons;
        public bool isPaused = false;
        public Form2(Form1 parent)
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
            this.enableStepButtons();
            string layout = this.parent.simulator.getLayout();
            this.parent.setTape(layout);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            this.parent.simulator.reset();
            this.inputSetButton.Enabled = true;
            this.enableStepButtons();
            this.textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool isContinuing = this.parent.step();
            this.positionText.Text = this.parent.simulator.tape.position.ToString();
            
            if (!isContinuing)
            {
                MessageBox.Show("Finish");
                this.disableStepButtons();
            }
        }

        public void initialize()
        {
            this.disableStepButtons();
            this.inputSetButton.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void disableStepButtons()
        {
            this.singleStep.Enabled = false;
            this.continiousStep.Enabled = false;
        }

        public void enableStepButtons()
        {
            this.singleStep.Enabled = true;
            this.continiousStep.Enabled = true;
        }
        async private void button3_Click(object sender, EventArgs e)
        {

            this.disableStepButtons();
            this.isPaused = false;
            bool isContinuing;
            do
            {
                isContinuing = this.parent.step();
                this.positionText.Text = this.parent.simulator.tape.position.ToString();
                await Task.Delay(10);
            }
            while (isContinuing && (!isPaused));
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.isPaused = true;
            this.enableStepButtons();
        }

        private void positionText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
