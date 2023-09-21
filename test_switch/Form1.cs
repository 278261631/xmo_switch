using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASCOM.xmo
{
    public partial class Form1 : Form
    {

        private ASCOM.DriverAccess.Switch driver;

        public Form1()
        {
            InitializeComponent();
            SetUIState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnected)
                driver.Connected = false;

            Properties.Settings.Default.Save();
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DriverId = ASCOM.DriverAccess.Switch.Choose(Properties.Settings.Default.DriverId);
            SetUIState();
        }

        List<CheckBox> switch_list = new List<CheckBox>();
        List<Button> button_list = new List<Button>();
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                driver.Connected = false;
                switch_list.Clear();
                button_list.Clear();
                flowLayoutPanel_switches.Controls.Clear();
                timer_states.Stop();
            }
            else
            {
                driver = new ASCOM.DriverAccess.Switch(Properties.Settings.Default.DriverId);
                driver.Connected = true;
                switch_list.Clear();
                button_list.Clear();
                flowLayoutPanel_switches.Controls.Clear();
                for (short i = 0; i < driver.MaxSwitch; i++)
                {
                    CheckBox checkbox = new CheckBox();
                    Button button_temp = new Button();
                    button_temp.Text = "Button " + i;
                    button_temp.Click += ChangeSwitchCLicked;
                    checkbox.Text =  i.ToString();
                    //checkbox.CheckedChanged += CheckBoxChanged;

                    checkbox.Checked = driver.GetSwitch(i);
                    
                    flowLayoutPanel_switches.Controls.Add(checkbox);
                    flowLayoutPanel_switches.Controls.Add(button_temp);
                    switch_list.Add(checkbox);
                    button_list.Add(button_temp);
                    Console.WriteLine(String.Format("+  {0} {1} ",i,checkbox.Checked));
                }
                timer_states.Start();
            }
            SetUIState();
        }

        private void SetUIState()
        {
            buttonConnect.Enabled = !string.IsNullOrEmpty(Properties.Settings.Default.DriverId);
            buttonChoose.Enabled = !IsConnected;
            buttonConnect.Text = IsConnected ? "Disconnect" : "Connect";
        }

        private bool IsConnected
        {
            get
            {
                return ((this.driver != null) && (driver.Connected == true));
            }
        }
        private void ChangeSwitchCLicked(object sender, EventArgs e)
        {
           
            Button button_switch = (Button)sender;
            
            for (short i = 0; i < switch_list.Count; i++)
            {
                if (button_list[i] == button_switch)
                {
                    driver.SetSwitch(i, !switch_list[i].Checked);
                    switch_list[i].Text = !switch_list[i].Checked ? "+" : "-";
                }
            }
           
        }

        private void timer_states_Tick(object sender, EventArgs e)
        {
            for (short i = 0; i < driver.MaxSwitch; i++)
            {
                switch_list[i].Checked =  driver.GetSwitch(i);
                Console.WriteLine(String.Format("U  {0} {1} ", i, switch_list[i].Checked));
            }
        }
    }
}
