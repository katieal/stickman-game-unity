using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Timer
{

    public enum Status { Inactive, Running, Paused, Complete }

    /// <summary>
    /// Base class for Custom Timers
    /// </summary>
    public abstract class CustomTimer
    {
        /// <summary>
        /// Current timer status.
        /// </summary>
        public Status Status { get; protected set; } = Status.Inactive;

        /// <summary>
        /// Time left on timer. Decreases when timer.tick() is called.
        /// </summary>
        public float Duration = 0;

        /// <summary>
        /// Enabled timers can be started and stopped, disabled ones do nothing.
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// Is the timer running?
        /// </summary>
        public bool IsRunning { get { return Status == Status.Running; } } 

        /// <summary>
        /// Invoked in Tick() when Duration reaches zero.
        /// </summary>
        public UnityAction OnTimerFinished;

        public void Enable() { IsEnabled = true; }
        public virtual void Disable()
        {
            IsEnabled = false;
            Status = Status.Inactive;
        }

        /// <summary>
        /// Temporarily pause timer execution, retaining current Duration.
        /// Timer must have been started and enabled first.
        /// </summary>
        public void Pause()
        {
            if (!IsEnabled) { return; }
            if (Status == Status.Running) { Status = Status.Paused; }
        }

        /// <summary>
        /// Resume timer after it is paused.
        /// </summary>
        /// <returns>False if timer is not paused.</returns>
        public bool Resume()
        {
            if (!IsEnabled) { return false; }
            if (Status == Status.Paused) { Status = Status.Running; return true; }
            return false;
        }

        /// <summary>
        /// If timer is both enabled and running, subtract deltatime from time.
        /// When duration reaches zero, invoke OnTimerFinished and stop timer.
        /// </summary>
        public void Tick()
        {
            if (IsEnabled && Status == Status.Running)
            {
                if (Duration > 0) { Duration -= Time.deltaTime; }
                if (Duration <= 0)
                {
                    OnTimerFinished?.Invoke();
                    Status = Status.Complete;
                }
            }
        }
    }

    /// <summary>
    /// Timer that runs for a fixed number of TotalSeconds.
    /// Can be easily reset or restarted.
    /// </summary>
    public class FixedTimer : CustomTimer
    {
        /// <summary>
        /// Total duration of timer. Ticks only happen if timer is both enabled and running.
        /// </summary>
        public float TotalSeconds = 0;

        public FixedTimer(float seconds)
        {
            TotalSeconds = seconds;
            Duration = seconds;
        }

        /// <summary>
        /// Start a timer with duration TotalSeconds.
        /// </summary>
        public void Start() { StartTimer(); }

        /// <summary>
        /// Reset Duration to TotalSeconds and start timer.
        /// </summary>
        public void Restart() { StartTimer(); }

        /// <summary>
        /// Reset timer Duration to TotalSeconds.
        /// </summary>
        public void Reset()
        {
            if (!IsEnabled) { return; }

            Duration = TotalSeconds;
        }

        /// <summary>
        /// Set timer to inactive and reset duration.
        /// </summary>
        public void Stop()
        {
            if (!IsEnabled) { return; }

            Status = Status.Inactive;
            Duration = TotalSeconds;
        }

        // start and restart do the same thing, using different functions for readability
        private void StartTimer()
        {
            if (!IsEnabled) { return; }

            Duration = TotalSeconds;
            Status = Status.Running;
        }
    }

    /// <summary>
    /// Timer with changable Duration. Cannot be started without specifying Duration.
    /// </summary>
    public class FlexibleTimer : CustomTimer
    {
        // default constructor
        public FlexibleTimer() { }

        /// <summary>
        /// Start the timer for specified seconds.
        /// Timer must be enabled to start.
        /// </summary>
        /// <param name="seconds"></param>
        public void StartTimer(float seconds)
        {
            if (!IsEnabled) { return; }
            Duration = seconds;
            Status = Status.Running;
        }

        /// <summary>
        /// Set timer status to Inactive and Duration to zero.
        /// </summary>
        public void Stop()
        {
            if (!IsEnabled) { return; }

            Status = Status.Inactive;
            Duration = 0;
        }

        /// <summary>
        /// Add specified number of seconds to timer duration.
        /// </summary>
        /// <param name="seconds"></param>
        public void AddTime(float seconds)
        {
            Duration += seconds;
        }

        /// <summary>
        /// Set timer duration.
        /// </summary>
        /// <param name="seconds"></param>
        public void SetDuration(float seconds) { Duration = seconds; }
    }
}