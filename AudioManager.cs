using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Media;


namespace Snake_C_
{
    public class MusicManager
    {

        //    Mets les fields en privé et encapsule les dans une propriété publique -> i.e
        //       private int myValue;  // private field

        //    public int MyValue    // public property to access the private field
        //    {
        //        get { return myValue; }
        //        set { myValue = value; }
        //    }
        public bool IsPlayed = false;
        private bool IsLooping = true;
        public string FileName;
        public string TrackName;
        private long lngVolume = 500;

       public MusicManager(string Name)
        {
            this.TrackName = Name;
            this.FileName = Path.Combine(Form1.executableDirectory, "sounds", Name + ".wav");
        }

        /*  public void playSimpleMusic(string sound)
        {
            soundLocation = Path.Combine(Form1.executableDirectory, "sounds", sound + ".wav");
            SoundPlayer simpleMusic = new SoundPlayer(soundLocation);
            simpleMusic.Play();
        }*/
        public void PlayWorker()
        {
            StringBuilder sb = new StringBuilder();
            //Tu n'utilises pas la variable "result", peut être la supprimer ? Ou job en cours ? 
            int result = mciSendString("open \"" + FileName + "\" type waveaudio  alias " + this.TrackName, sb, 0, IntPtr.Zero);
            //Ce serait plus safe de capturer le code d'erreur quand on utilise une méthode prebuilt comme celle ci -> attribuer la méthode à result comme au dessus et utiliser result pour check ie:
            // if (result != 0)
            // {
            //     Console.WriteLine($"Failed to open audio: error code {result}");
            //     return;
            // }
            mciSendString("play " + this.TrackName, sb, 0, IntPtr.Zero);
            IsPlayed = true;

            sb = new StringBuilder();
            //Same pour le catch d'erreur
            mciSendString("status " + this.TrackName + " length", sb, 255, IntPtr.Zero);
            int length = Convert.ToInt32(sb.ToString());
            int pos = 0;
            long oldvol = lngVolume;
            // Je peux me tromper et probablement work in progress mais pour l'instant j'ai l'impression que "isplayed" est toujours vrai vu que ça a l'air de se lancer pour le build du jeu. Si le but est que ce soit tout le temps vrai, pas besoin de while vu que c'est appelé après play this qui catch l'erreur
            while (IsPlayed) 
            {
                sb = new StringBuilder();
                //Same pour le catch d'erreur
                mciSendString("status " + this.TrackName + " position", sb, 255, IntPtr.Zero);
                pos = Convert.ToInt32(sb.ToString());
                if (pos >= length)
                {
                    if (!IsLooping)
                    {
                        IsPlayed = false;
                        break;
                    }
                    else
                    {
                        //Same pour le catch d'erreur
                        mciSendString("play " + this.TrackName + " from 0", sb, 0, IntPtr.Zero);
                    }
                }

                if (oldvol != lngVolume)
                {
                    sb = new StringBuilder("................................................................................................................................");
                    string cmd = "setaudio " + this.TrackName + " volume to " + lngVolume.ToString();
                    long err = mciSendString(cmd, sb, sb.Length, IntPtr.Zero);
                    System.Diagnostics.Debug.Print(cmd);
                    if (err != 0)
                    {
                        System.Diagnostics.Debug.Print("ERror " + err);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Print("No errors!");
                    }
                    oldvol = lngVolume;
                }
                Application.DoEvents();
            }
            //Same pour le catch d'erreur
            mciSendString("stop " + this.TrackName, sb, 0, IntPtr.Zero);
            mciSendString("close " + this.TrackName, sb, 0, IntPtr.Zero);
        }

        public int GetVolume()
        {
            return (int)this.lngVolume / 100;
        }
        public void SetVolume(int newvolume)
        {
            this.lngVolume = newvolume * 100;
        }

        public void PlayThis(bool Looping)
        {
            try
            {
                if (IsPlayed) // Assuming IsPlayed is the new name for IsBeingPlayed
                    return;

                if (!File.Exists(FileName))
                {
                    // The file does not exist.
                    // Log this as an error or show a message to the user.
                    // DO NOT set IsPlayed = true, as nothing is playing.
                    //frmMain.WriteLog($"Error: Music file not found at '{FileName}'"); // Re-enable logging
                    MessageBox.Show($"Error: The audio file '{FileName}' was not found.", "Audio Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    IsPlayed = false; // Ensure it's false, nothing is playing.
                    return; // EXIT THE METHOD if the file doesn't exist.
                }

                // If the file exists, then proceed to set looping and start the worker thread.
                this.IsLooping = Looping;
                ThreadStart ts = new ThreadStart(PlayWorker);
                Thread WorkerThread = new Thread(ts);
                WorkerThread.Start();

                IsPlayed = true; // Only set to true AFTER the worker thread has been successfully started
                                 // and is *attempting* to play.
            }
            catch (Exception ex)
            {
                // Catch any exceptions that occur during thread creation or initial setup.
                //frmMain.WriteLog("Error occurred in PlayThis method: " + ex.Message);
                IsPlayed = false; // Ensure flag is reset if an error prevents playback
            }
        }
        public void StopPlaying()
        {
            IsPlayed = false;
        }
        // On aime bien normalement ajouter le charset au P/Invoke argument pour éviter de potentiels bugs s'il y arrive pas par défaut : ie: [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
        //On préfère normalement utiliser une librairie quand c'est pour appeler une méthode -> Ca permet d'opti au lancement (ex ici : 
        //[LibraryImport("winmm.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        // [return: MarshalAs(UnmanagedType.Bool)]
        // static partial bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);)
        [DllImport("winmm.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern bool PlaySound(
            string pszSound,
            IntPtr hMod,
            SoundFlags sf);

        // L'attribut "Flags" s'utilise quand on utilise des valeurs qui sont des puissances de deux, or ici ce n'est pas le cas de certaines d'entre elles SND_RESOURCE = 0x00040004 par ex. -> Soit supprimer Flags soit corrigé pour que ça le devienne ? 
        [Flags]
        // Si tu supprime flags faudrait rename soundflags
        // Pas besoin de préciser int, c'est le type par défaut d'une enum
        public enum SoundFlags : int
        {
            SND_SYNC = 0x0000,  // play synchronously (default)
            SND_ASYNC = 0x0001,  // play asynchronously
            SND_NODEFAULT = 0x0002,  // silence (!default) if sound not found
            SND_MEMORY = 0x0004,  // pszSound points to a memory file
            SND_LOOP = 0x0008,  // loop the sound until next sndPlaySound
            SND_NOSTOP = 0x0010,  // don't stop any currently playing sound
            SND_PURGE = 0x40, // <summary>Stop Playing Wave</summary>
            SND_NOWAIT = 0x00002000, // don't wait if the driver is busy
            SND_ALIAS = 0x00010000, // name is a registry alias
            SND_ALIAS_ID = 0x00110000, // alias is a predefined ID
            SND_FILENAME = 0x00020000, // name is file name
            SND_RESOURCE = 0x00040004  // name is resource name or atom
        }
    }
}
