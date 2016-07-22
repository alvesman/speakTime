using System;
using System.Windows.Forms;

using System.Speech;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace speakTime
{
    public partial class Form1 : Form
    {

        private SpeechSynthesizer speechSynthesizerObj;

        public Form1()
        {
            InitializeComponent();
        }

        private void setMessageFilter()
        {
            int error;

            // preparar para escutar a mensagem NativeMethods.WM_SHOWME

            NativeMethods.ChangeWindowMessageFilterEx(
                this.Handle,
                NativeMethods.WM_SHOWME,
                (uint)1, // Reset = 0, Allow = 1, DisAllow = 2
                IntPtr.Zero);

            error = Marshal.GetLastWin32Error();
            if (error != 0)
            {
                MessageBox.Show(new Win32Exception(error).Message);
            }

            // ver Program.cs, FindWindowByCaption.
            this.Text = NativeMethods.Window_Caption;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            setMessageFilter();

            speechSynthesizerObj = new SpeechSynthesizer();
            speechSynthesizerObj.Volume = 100; // 0 -> 100

            // %windir%\sysWOW64\speech\SpeechUX\SAPI.cpl
            //
            //foreach (InstalledVoice voice in speechSynthesizerObj.GetInstalledVoices())
            //{
            //    VoiceInfo info = voice.VoiceInfo;
            //    Console.WriteLine(" Voice Name: " + info.Name);
            //}

            if (Properties.Settings.Default.SpeakAtIntervals == 61)
            {
                // hack para saber que é a 1ª vez que é invocado.
                // neste caso mostrar-se para permitir a configuração

                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                Properties.Settings.Default.SpeakAtIntervals = 30;
                Properties.Settings.Default.Save();

            }

            textBox1.Text = Properties.Settings.Default.SpeakAtIntervals.ToString();
            
            timer1.Interval = 500;
            timer1.Enabled = true;

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowMe();
            }
            base.WndProc(ref m);
        }
        private void ShowMe()
        {
            // speechSynthesizerObj.Speak("Activating!");

            this.ShowInTaskbar = true;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            // make our form jump to the top of everything
            TopMost = true;
            // set it back to whatever it was
            TopMost = top;
        }

        private int lastMinuteSpoken = -1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if ((DateTime.Now.Minute != lastMinuteSpoken) && (DateTime.Now.Minute % Properties.Settings.Default.SpeakAtIntervals == 0))
            {
                speechSynthesizerObj.SpeakAsync(horas());
                lastMinuteSpoken = DateTime.Now.Minute;
            }

            timer1.Enabled = true;

        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            speechSynthesizerObj.Speak("Goodbye!");
        }

        private string horas()
        {
            string msg;

            msg = string.Format("{0} hours", DateTime.Now.Hour);
            if (DateTime.Now.Minute != 0)
            {
                msg = string.Format("{0} and {1} minutes",msg, DateTime.Now.Minute);
            }

            return msg;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int n;
            bool isInteger = int.TryParse(textBox1.Text, out n);

            if (isInteger)
            {
                Properties.Settings.Default.SpeakAtIntervals = n;
                Properties.Settings.Default.Save();

                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            speechSynthesizerObj.SpeakAsync(horas());
        }
    }
}
