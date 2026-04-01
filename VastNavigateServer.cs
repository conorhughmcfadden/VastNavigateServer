using System;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;

namespace VastNavigateServer
{
    public partial class VastNavigateServer : Form
    {
        private bool running = true;
        private bool readyForInit = false;
        private bool vastInitialized = false;

        // verbose text
        private string vText = string.Empty;

        // threading
        private Thread signalThread;
        private ThreadStart signalStartThread;

        // named pipe
        private NamedPipeServerStream serverStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        // Vast-specific members
        private Reflx2011.IAutoSampler _autoSamp = null;
        private Reflx2011.VastBioImagerWrapper _vastWrap = null;

        public VastNavigateServer()
        {
            // initialize the WinForm
            InitializeComponent();

            VerboseLine("- - - - - - Welcome to VastNavigateServer! - - - - - -");
            VerboseLine();

            // create the autosamper object
            _autoSamp = new Reflx2011.AutoSampler();

            // start listening for LPS init
            this.lpsTimer.Enabled = true;
        }

        private void VerboseLine(string text = "")
        {
            vText += text + "\r\n";
            this.textBox.Text = vText;
        }

        private void ConnectToNavigate()
        {
            VerboseLine("Waiting for Navigate connection...");

            // disable button while connected
            this.buttonConnect.Enabled = false;

            // create and start signal thread
            signalStartThread = new ThreadStart(Run);
            signalThread = new Thread(signalStartThread);

            // create pipe and connect
            serverStream = new NamedPipeServerStream("VastServerPipe");
            serverStream.WaitForConnection();
            reader = new BinaryReader(serverStream);
            writer = new BinaryWriter(serverStream);

            VerboseLine("Navigate connection established!");

            signalThread.Start();
        }

        private void Run()
        {
            while (running)
            {
                try
                {
                    Send(HandleMessage(Receive()));
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }

            // cleanup
            serverStream.Close();
            serverStream.Dispose();

            VerboseLine("Navigate connection lost! Please reconnect and restart Navigate.");

            // enable connect button
            this.buttonConnect.Enabled = true;
        }

        private string HandleMessage(string message)
        {
            string outputString = "";

            VerboseLine("Received msg: \'" + message + "\'...");

            // parse message string based on ','
            string[] msgComponents = message.Split(',');
            message = msgComponents[0];                      // first component = command
            string[] args = msgComponents.Skip(1).ToArray(); // args are the rest

            if (vastInitialized)
            {
                // allow commands once initialized
                switch (message)
                {
                    // set methods
                    case "mabs":
                        _autoSamp.ExecuteVastMethod("MoveAbs3Motors", args);
                        break;

                    case "mrel":
                        _autoSamp.ExecuteVastMethod("MoveRelative3Motors", args);
                        break;

                    case "rot":
                        _autoSamp.ExecuteVastMethod("RotateCapillary", args);
                        break;

                    case "get_xy_pos":
                        int[] xyPos = (int[])_autoSamp.ExecuteVastMethod("GetXYPosition", args);
                        
                        outputString = "" + xyPos[0] + "," + xyPos[1];
                        break;

                    case "set_autost":
                        _vastWrap.SetAutoStore(true, args[0]);
                        break;

                    // get methods
                    case "get_autost":
                        outputString = _vastWrap.GetLastAutoStoreLocation();
                        break;

                    case "busy":
                        args = new string[1];
                        args[0] = "3"; // int motorId = 3 (query all motors)
                        outputString = "" + (int)_autoSamp.ExecuteVastMethod("CheckMotorsBusyStatus", args);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                // only allow "boot" if not initialized
                if (message == "boot")
                {
                    VerboseLine("Initalizing LPS+VAST...");
                    readyForInit = true;
                }
                else
                    VerboseLine("LPS+VAST not yet initialized...");
            }

            return outputString;
        }

        private void Send(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            writer.Write((uint)buffer.Length);
            writer.Write(buffer);
        }

        private string Receive()
        {
            return new string(reader.ReadChars((int)reader.ReadUInt32()));
        }

        // ---------- Timer Event Functions ----------
        // Handles initialization of LPS+VAST software
        private void LPSTimer_Tick(object sender, EventArgs e)
        {
            // Waits for "init" call from pipe to initialize the LPS
            if (readyForInit)
            {
                readyForInit = false;
                
                // init LPS
                if (_autoSamp.Initialize(false) != 0)
                {
                    running = false; // kill the whole thing
                    MessageBox.Show("Autosampler failed to initialize!");
                }

                this.lpsTimer.Enabled = false; // kill lps timer; it's done it's job
                this.vastTimer.Enabled = true; // begin to init the VAST
            }
        }

        private void VASTTimer_Tick(object sender, EventArgs e)
        {
            this.vastTimer.Enabled = false; // again, done it's job

            // initialize VAST and show screen
            _vastWrap = _autoSamp.IniVastBioImager();
            if (_vastWrap != null)
            {
                _vastWrap.ShowVastScreen();
                vastInitialized = true;
            }
            else
            {
                running = false; // kill the whole thing
                MessageBox.Show("VAST BioImager failed to initialize!");
                return;
            }

            // show LPS screen
            _autoSamp.ShowLPSamplerScreen();
        }

        // ---------- Button Event Functions ----------
        private void LaunchButton_Click(object sender, EventArgs e)
        {
            HandleMessage("boot");
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectToNavigate();
        }

        private void VastNavigateServer_Load(object sender, EventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
