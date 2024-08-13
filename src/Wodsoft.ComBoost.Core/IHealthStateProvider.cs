using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IHealthStateProvider
    {
        HealthState State { get; }

        event EventHandler<HealthStateChangedEventArgs> HealthStateChanged;
    }

    public enum HealthState
    {
        Healthy = 0,
        Warning = 1,
        Critical = 2
    }

    public class HealthStateChangedEventArgs : EventArgs
    {
        public HealthStateChangedEventArgs(HealthState state)
        {
            State = state;
        }

        public HealthState State { get; }
    }
}
