//------------------------------------------------------------------------------
// <copyright file="FOTerminal.cs" >
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Media;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace com.jacky.foconsole
{

    internal sealed class FOTerminal
    {


        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;


        /// <summary>
        /// Initializes a new instance of the <see cref="FOTerminal"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        /// 
        SoundPlayer powerOnAudio = new SoundPlayer(Properties.Resources.poweron);
        SoundPlayer powerOffAudio = new SoundPlayer(Properties.Resources.poweroff);
        Stream[] keystrokeAudioStreams;
        const int numKeyAudio = 30;

        Timer timer = new Timer();

        public FOTerminal(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            this.layer = view.GetAdornmentLayer("TextAdornment1");
            //generate random key strokes
            keystrokeAudioStreams = new Stream[numKeyAudio];
            Random r = new Random();
            for (int i = 0; i < numKeyAudio; i++)
            {

                keystrokeAudioStreams[i] = Properties.Resources.ResourceManager.GetStream("k" + r.Next() % 11);


            }
            //
            timer.Interval = 50f;
            timer.Elapsed += this.OnTimeElapsed;
            timer.Start();
            
            //
            //play console boot audio
            //
            powerOnAudio.Play();
            this.view = view;
            this.view.TextBuffer.Changed += this.OnTextBufferChanged;
            this.view.Closed += this.OnViewClosed;
        }

        private void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            canPlaySound = true;
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            powerOffAudio.Play();
        }
        //play keystroke audio
        SoundPlayer sp = new SoundPlayer();
        int keyAudioQueue = 0;
        bool canPlaySound = true;
        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.Changes != null)
            {
                if (canPlaySound)
                {
                    sp.Stream = keystrokeAudioStreams[keyAudioQueue];
                    sp.Stream.Seek(0, SeekOrigin.Begin);
                    sp.Play();
                    keyAudioQueue = (keyAudioQueue + 1) % numKeyAudio;
                    canPlaySound = false;
                }
            }
        }
    }
}
