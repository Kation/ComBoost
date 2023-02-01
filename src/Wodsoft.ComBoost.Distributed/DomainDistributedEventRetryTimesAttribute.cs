using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// Define distributed event retry times.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DomainDistributedEventRetryTimesAttribute : Attribute
    {
        /// <summary>
        /// Initiate attribute.
        /// </summary>
        /// <param name="times">Retry times and waiting milliseconds.<br/>
        /// Length of array defined retry times.<br/>
        /// Each value of array defined waiting milliseconds after failed for previous executing.
        /// Each value of array must large or equal than previous array value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DomainDistributedEventRetryTimesAttribute(params int[] times)
        {
            if (times.Length == 0)
                throw new ArgumentException("Array can`t be empty.");
            int wait = 0;
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] < wait)
                    throw new ArgumentOutOfRangeException("Each value must large or equal than previous value.");
                wait = times[i];
            }
            Times = times;
        }

        public int[] Times { get; }
    }
}
