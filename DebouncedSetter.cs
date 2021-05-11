using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace TESTS.Snippets
{
    public class DebouncedAction 
    {
        private Timer debounceTimer;

        private Action targetAction;
        public Action TargetAction
        {
          get
          {
              return targetAction;
          }
          set
          {
              targetAction = value;
              TrySet();
          }
        }

        public DebouncedAction(double debounceTimeMs)
        {
          debounceTimer = new Timer(debounceTimeMs);
          debounceTimer.Elapsed += DoSet;
          debounceTimer.AutoReset = false;
        }

        public void TrySet()
        {
          if (!debounceTimer.Enabled)
          {
              debounceTimer.Start();
          }
          else
          {
              // reset the timer
              debounceTimer.Stop();
              debounceTimer.Start();
          }
        }

        public void DoSet(Object source, ElapsedEventArgs e)
        {
          targetAction();
        }
    }
}
