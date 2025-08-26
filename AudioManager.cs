using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Media;
using NAudio.Wave;


namespace Snake_C_
{
    public class MusicManager
    {
        private AudioFileReader reader;
        private WaveOutEvent output;

        public bool IsPlaying => output?.PlaybackState == PlaybackState.Playing;

        public string FileName { get; private set; }
        public string TrackName { get; private set; }

        public MusicManager(string name)
        {
            TrackName = name;
            FileName = Path.Combine(Form1.executableDirectory, "sounds", name + ".wav");

            if (!File.Exists(FileName))
                throw new FileNotFoundException("Audio file not found", FileName);

            int startVolume = 400;
            reader = new AudioFileReader(FileName)
            {
                Volume = Math.Clamp(startVolume, 0, 400) / 400f
            };
            output = new WaveOutEvent();
            output.Init(reader);
        }

        public void PlayThis(bool loop = false)
        {
            reader.Position = 0; 
            output.Play();

            if (loop)
            {
                output.PlaybackStopped += (s, e) =>
                {
                    reader.Position = 0;
                    output.Play();
                };
            }
        }

        public void StopPlaying()
        {
            Volume = 0;
            output.Stop();
        }

        public int Volume
        {
            get => (int)(reader.Volume * 400);  // 0–100
            set => reader.Volume = Math.Clamp(value, 0, 400) / 400f;
        }
    }
}
