using System;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Keystroke.API;
using System.Drawing;
using System.Runtime.InteropServices;
using CSCore.SoundOut;
using System.IO;
using CSCore.MediaFoundation;
using CSCore;

namespace tts3
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;


        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();


            ContextMenu contextMenu1 = new ContextMenu();
            

            NotifyIcon notifyIcon1 = new NotifyIcon(new System.ComponentModel.Container())
            {
                Icon = Properties.Resources.icon,
                ContextMenu = contextMenu1,
                Text = "ttsvoice is running...",
                Visible = true
            };

           
            MenuItem menuItem1 = new MenuItem { 
                Index = 0,
                Text = "E&xit",
            };

            menuItem1.Click += new EventHandler((object Sender, EventArgs e) => {
                Environment.Exit(1);
            });


            contextMenu1.MenuItems.AddRange(
                    new MenuItem[] { menuItem1 });


            var api = new KeystrokeAPI();
            SpeechSynthesizer synth = new SpeechSynthesizer();

            synth.SelectVoiceByHints(VoiceGender.Female);


            Console.WriteLine("Available devices:");
            foreach (var device in WaveOutDevice.EnumerateDevices())
            {
                Console.WriteLine("{0}: {1}", device.DeviceId, device.Name);
            }
            Console.WriteLine("\nEnter device for speech output:");
            var deviceId = (int)char.GetNumericValue(Console.ReadKey().KeyChar);
            ShowWindow(handle, SW_HIDE);


            var stream = new MemoryStream();



            var waveOut = new WaveOut { Device = new WaveOutDevice(deviceId) };

            api.CreateKeyboardHook((character) =>
            {
                if ((int)character.KeyCode == 114)
                {
                    stream = new MemoryStream(stream.Capacity);
                    synth.SetOutputToWaveStream(stream);
                    synth.Speak(ShowDialog("TTS window"));
                    var waveSource = new MediaFoundationDecoder(stream);
                    waveOut.Initialize(waveSource);
                    waveOut.Play();
                    waveOut.WaitForStopped();
                }
            });

            Application.Run();
            Console.ReadKey();
        }

        public static string ShowDialog(string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 40,
                FormBorderStyle = FormBorderStyle.None,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.Gray
            };
            TextBox textBox = new TextBox() { Left = 1, Top = 1, Width = 297, BackColor = Color.Black, ForeColor = Color.Beige, Font = new Font(prompt.Font.FontFamily, 24) };
            prompt.Height = textBox.Height + 2;
            Button confirmation = new Button() { Text = "Ok", Left = 0, Width = 0, Top = 0, DialogResult = DialogResult.OK, Height = 0 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

    }
}
