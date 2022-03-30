using System;
using System.Timers;

namespace TEST.Snippets
{
    class PartiallyDebouncedAction
    {
        /// <summary>
        /// Will run the first instance of the action, and then wait for debounce time before sending another action.
        /// If another action is received in this interval it will overwrite the current action.
        /// </summary>
        private object _targetActionLock = new object();

        private System.Timers.Timer debounceTimer;

        private Action targetAction = null;
        public Action TargetAction
        {
            get
            {
                return targetAction;
            }
            set
            {
                lock (_targetActionLock)
                {
                    targetAction = value;
                    TrySet();
                }
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        public PartiallyDebouncedAction(double debounceTimeMs)
        {
            debounceTimer = new System.Timers.Timer(debounceTimeMs);
            isBusy = false;

            debounceTimer.Elapsed += DoSet;
            debounceTimer.AutoReset = true;
        }

        public void TrySet()
        {
            if (!isBusy)
            {
                isBusy = true;
                debounceTimer.Start();

                targetAction();
                targetAction = null;
                // will wait for timer to finish before firing the next one
            }
        }

        public void DoSet(Object source, ElapsedEventArgs e)
        {
            lock (_targetActionLock)
            {
                if (targetAction != null)
                {
                    targetAction();
                    targetAction = null;
                    //timer will auto reset, run one more timer instance, if the next action is still null the timer will exit
                }
                else
                {
                    debounceTimer.Stop();
                    isBusy = false;
                }
            }
        }
    }
}
