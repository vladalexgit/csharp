using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TESTS.Snippets
{
    class CancellableAction
    {
        private CancellationTokenSource actionCancelationTokenSource;
        private CancellationToken actionCancelationToken;

        private object _cancellationTokenLock = new object();

        private bool isRunning = false;
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        private bool canCancelBeforeFirstStep;
        public bool CanCancelBeforeFirstStep
        {
            get
            {
                return canCancelBeforeFirstStep;
            }
        }

        private Action targetPreAction;
        public Action TargetPreAction
        {
            get
            {
                return targetPreAction;
            }
            set
            {
                if (!isRunning)
                {
                    targetPreAction = value;
                }
            }
        }

        private Action targetAction;
        public Action TargetAction
        {
            get
            {
                return targetAction;
            }
            set
            {
                if (!isRunning)
                {
                    targetAction = value;
                }
            }
        }

        private Action targetPostAction;
        public Action TargetPostAction
        {
            get
            {
                return targetPostAction;
            }
            set
            {
                if (!isRunning)
                {
                    targetPostAction = value;
                }
            }
        }

        public CancellableAction(bool canCancelBeforeFirstStep = false)
        {
            this.canCancelBeforeFirstStep = canCancelBeforeFirstStep;

            lock (_cancellationTokenLock)
            {
                actionCancelationTokenSource = new CancellationTokenSource();
                actionCancelationToken = actionCancelationTokenSource.Token;
            }
        }

        public void Start()
        {
            lock (_cancellationTokenLock)
            {
                actionCancelationTokenSource = new CancellationTokenSource();
                actionCancelationToken = actionCancelationTokenSource.Token;

                isRunning = true;
            }

            Task.Run(() =>
            {
                if (canCancelBeforeFirstStep && actionCancelationToken.IsCancellationRequested)
                {
                    return;
                }

                targetPreAction();

                while (true)
                {
                    targetAction();

                    if (actionCancelationToken.IsCancellationRequested)
                    {
                        // cleanup and exit
                        break;
                    }

                    targetPostAction();
                }

                isRunning = false;

            }, actionCancelationToken);
        }

        public void Cancel()
        {
            lock (_cancellationTokenLock)
            {
                actionCancelationTokenSource.Cancel();
                isRunning = false;
            }
        }
    }
}
