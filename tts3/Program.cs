using System;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Keystroke.API;
using System.Runtime.InteropServices;
using CSCore.SoundOut;
using System.IO;
using CSCore.MediaFoundation;
using CSCore;
using System.Threading;

namespace tts3
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        

        public static void Main()
        {
            GuiHandler gui = new GuiHandler();
            var handle = GetConsoleWindow();


            Console.WriteLine("Available devices:");
            foreach (WaveOutDevice device in WaveOutDevice.EnumerateDevices())
                Console.WriteLine("{0}: {1}", device.DeviceId, device.Name);
            
            Console.WriteLine("\nEnter device for speech output:");
            var deviceId = (int)char.GetNumericValue(Console.ReadKey().KeyChar);



            ShowWindow(handle, SW_HIDE);

            gui.CreateTaskBarIcon();
            gui.AddMenuOption("E&xit", (sender, e) => Environment.Exit(0));


            
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(VoiceGender.Female);

            foreach (InstalledVoice f in synth.GetInstalledVoices())
            {
                gui.AddMenuOption("Se&t voice to " + f.VoiceInfo.Name,
                    (sender, e) => synth.SelectVoice(f.VoiceInfo.Name)
                );
                
            }

            


            new KeystrokeAPI().CreateKeyboardHook((character) =>
            {
                if ((int)character.KeyCode == 114)
                {

                    WaveOut waveOut = new WaveOut { Device = new WaveOutDevice(deviceId) };

                    MemoryStream stream = new MemoryStream();


                    synth.SetOutputToWaveStream(stream);

                    string text = GuiHandler.GetUserInput();

                    if (text != "")
                    {
                        synth.Speak(text);

                        var waveSource = new MediaFoundationDecoder(stream);
                        new Thread(() =>
                        {
                            waveOut.WaitForStopped();
                            waveOut.Initialize(waveSource);
                            waveOut.Play();
                            waveOut.WaitForStopped();
                        }).Start();

                    }
                }
            });

            Application.Run();
            while (true) { }
        }

    }
}
