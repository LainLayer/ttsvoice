using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSCore.MediaFoundation;
using CSCore;
using CSCore.SoundOut;
using System.IO;
using System.Speech.Synthesis;

namespace tts3.UnitTests
{
    [TestClass]
    public class TTSVoiceTests
    {


        [TestMethod]
        public void GetAudioDeviceTest()
        {
            int c = 0;
            foreach (WaveOutDevice d in WaveOutDevice.EnumerateDevices())
                c++;
            Assert.AreNotEqual(c, 0);
        }

        [TestMethod]
        public void PlaySpeech()
        {
            MemoryStream stream = new MemoryStream();
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(VoiceGender.Female);
            WaveOut waveOut = new WaveOut { Device = new WaveOutDevice(0) };
            synth.SetOutputToWaveStream(stream);
            synth.Speak("test");
            var waveSource = new MediaFoundationDecoder(stream);
            waveOut.Initialize(waveSource);
            waveOut.Play();
            waveOut.WaitForStopped();
        }

        [TestMethod]
        public void ThreadPlaySpeech()
        {


            Thread[] threads = new Thread[10];


            for (int i = 0; i < 10; i++)
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SelectVoiceByHints(VoiceGender.Female);
                WaveOut waveOut = new WaveOut { Device = new WaveOutDevice(0) };
                MemoryStream stream = new MemoryStream();
                synth.SetOutputToWaveStream(stream);

                synth.Speak("ok so based on your answers I can for example send to the client the number of bytes that I am planing to send.");


                var waveSource = new MediaFoundationDecoder(stream);
                threads[i] = new Thread(() =>
                {
                    waveOut.WaitForStopped();
                    waveOut.Initialize(waveSource);
                    waveOut.Play();
                    waveOut.WaitForStopped();

                });
                threads[i].Start();
                Thread.Sleep(100);
            }

            threads[9].Join();

        }

    }
}
