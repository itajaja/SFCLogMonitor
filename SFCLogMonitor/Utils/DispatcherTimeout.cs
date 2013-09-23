using System;
using System.Windows.Threading;

namespace SFCLogMonitor.Utils
{
    /// <summary>
    /// A dispatcherTimer subclass that fires only once
    /// </summary>
    public class DispatcherTimeout : DispatcherTimer
    {
        #region Constructors and Destructors

        protected DispatcherTimeout(DispatcherPriority priority)
            : base(priority)
        {
        }

        #endregion

        #region Public Properties

        public Action<DispatcherTimeout> Callback { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Instantiates a new DispatcherTimeout and starts it.
        /// </summary>
        /// <param name="priority">
        /// The dispatcher priority used for the timer. 
        /// </param>
        /// <param name="duration">
        /// The duration. 
        /// </param>
        /// <param name="callback">
        /// The callback which should be called on tick. 
        /// </param>
        /// <returns>
        /// An instance of DispatcherTimeout.
        /// </returns>
        public static DispatcherTimeout Timeout(DispatcherPriority priority, TimeSpan duration, Action<DispatcherTimeout> callback)
        {
            if (duration < TimeSpan.Zero) duration = TimeSpan.Zero;
            var dispatcherTimeout = new DispatcherTimeout(priority) {Interval = duration, Callback = callback};
            dispatcherTimeout.Tick += dispatcherTimeout.HandleTick;
            dispatcherTimeout.Start();
            return dispatcherTimeout;
        }

        #endregion

        #region Methods

        private void HandleTick(object sender, EventArgs e)
        {
            Stop();
            Tick -= HandleTick;

            if (Callback != null)
            {
                Callback(this);
            }
        }

        #endregion
    }
}
