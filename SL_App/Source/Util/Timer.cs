using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SL_App.Util
{
    public class SimpleTimer
    {
        private Thread _thread;
        private Action _callback;

        /**
         * time = is timing in ms
         * loop = wether or not the timer is looping
         */
        public SimpleTimer(int time, bool loop, Action callback)
        {
            Time = time;
            IsLooping = loop;
            IsRunning = false;

            _callback = callback;
        }

        /**
         * Makes a new thread instance, starts it and sets IsRunning to true
         */
        public void StartTimer()
        {
            _thread = new Thread(Run);
            _thread.Name = "Timer";
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            IsRunning = true;
        }

        public void Run()
        {
            try
            {
                Thread.Sleep(Time);
                if (IsRunning)
                {
                    _callback.Invoke();

                    if (IsLooping)
                    {
                        Run();
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
            }

            IsRunning = false;
        }

        /**
         * Interupts the thread and sets IsRunning to false, causing it to stop and exit
         */
        public void StopTimer()
        {
            IsRunning = false;
            _thread.Interrupt();
        }
        
        /**
         * The interval of the timer
         */
        public int Time { get; private set; }

        /**
         * Wether or not the timer is looping, if the timer is looping it will continue to loop until the timer is stopped
         */
        public bool IsLooping { get; private set; }

        /**
         * Determines if the thread is running or not, in order to stop the thread from running call StopTimer
         */
        public bool IsRunning { get; private set; }
    }
}
