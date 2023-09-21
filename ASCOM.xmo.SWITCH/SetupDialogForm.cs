using ASCOM.Utilities;
using ASCOM.xmo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ASCOM.xmo
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        TraceLogger tl; // Holder for a reference to the driver's trace logger

        IPAddress deviceIP = IPAddress.Parse("192.168.1.166");
        int devicePort = 1234;
        bool ip_check_pass = false;
        bool port_check_pass = false;

        public SetupDialogForm(TraceLogger tlDriver)
        {
            InitializeComponent();

            // Save the provided trace logger for use within the setup dialogue
            tl = tlDriver;

            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here
            // Update the state variables with results from the dialogue
            Switch.comPort = (string)comboBoxComPort.SelectedItem;
            Switch.client_ip = deviceIP.ToString() ;
            Switch.port = devicePort;
            tl.Enabled = chkTrace.Checked;
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("https://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {
            chkTrace.Checked = tl.Enabled;
            // set the list of com ports to those that are currently available
            comboBoxComPort.Items.Clear();
            comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static
            // select the current port if possible
            if (comboBoxComPort.Items.Contains(Switch.comPort))
            {
                comboBoxComPort.SelectedItem = Switch.comPort;
            }
        }

        private void SetupDialogForm_Load(object sender, EventArgs e)
        {
            textBox_ip_TextChanged(sender, e);
            textBox_port_TextChanged(sender, e);
        }

        private void textBox_ip_TextChanged(object sender, EventArgs e)
        {
            string ip_input = this.textBox_ip.Text;
            if (IPAddress.TryParse(ip_input, out IPAddress ipAddress))
            {
                // 输入的字符串是有效的IPv4地址
                Console.WriteLine("有效的IPv4地址: " + ip_input);
                deviceIP = ipAddress;
                ip_check_pass = true;
                display_check_result();

            }
            else
            {
                ip_check_pass = false;
                // 输入的字符串不是有效的IPv4地址
                Console.WriteLine("无效的IPv4地址  " + ip_input);
                display_check_result();
            }
        }

        private void textBox_port_TextChanged(object sender, EventArgs e)
        {
            string port_input = this.textBox_port.Text;
            if (int.TryParse(port_input, out int port))
            {
                if(port>0 && port < 65535)
                {
                    port_check_pass = true;
                    // 输入的字符串是有效的IPv4地址
                    Console.WriteLine("有效的IPv4端口: " + port_input);
                    this.textBox_ip.ForeColor = Color.Black;
                    display_check_result();
                }
                else
                {
                    port_check_pass = false;
                    Console.WriteLine("无效的IPv4端口  " + port_input);
                    display_check_result();
                }

            }
            else
            {
                port_check_pass = false;
                // 输入的字符串不是有效的IPv4地址
                Console.WriteLine("无效的IPv4端口  " + port_input);
                display_check_result();
            }
        }
        private void display_check_result()
        {
            this.label_msg.Text = deviceIP.ToString() +":" + devicePort.ToString();
            this.label_msg.ForeColor = (ip_check_pass && port_check_pass) ? Color.Green : Color.Red;
            this.textBox_ip.ForeColor = ip_check_pass ? Color.Green : Color.Red;
            this.textBox_port.ForeColor = port_check_pass ? Color.Green : Color.Red;
            this.cmdOK.Enabled = ip_check_pass && port_check_pass ;
        }

        private void button_test_Click(object sender, EventArgs e)
        {

        }
    }
}