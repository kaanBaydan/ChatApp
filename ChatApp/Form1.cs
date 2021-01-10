using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChatApp
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set up socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // get user ID
            txtLocalIp.Text = GetLocalIp();
            txtRemoteIp.Text = GetLocalIp();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //binding Socket
            epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIp.Text), Convert.ToInt32(txtLocalPort.Text));
            sck.Bind(epLocal);
            //Connecting to remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(txtRemoteIp.Text), Convert.ToInt32(txtRemotePort.Text));
            sck.Connect(epRemote);
            //Listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote,
                new AsyncCallback(MessageCallBack), buffer);
        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedDate = new byte[1500];
                receivedDate = (byte[])aResult.AsyncState;
                //converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedDate);
                //adding this message into ListBox
                listMessage.Items.Add("Friend: " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote,
                    new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //convert string message to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(txtMessage.Text);
            //sending the Encoded message
            sck.Send(sendingMessage);
            //add to the list box
            listMessage.Items.Add("Me: " + txtMessage.Text);
            txtMessage.Text = "";
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtRemoteIp2_TextChanged(object sender, EventArgs e)
        {

        }

        private string GetLocalIp()
        {
            // bilgisayarın local IP sini veriyor.
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }

            }
            return "127.0.0.1";
        }
    }
}
