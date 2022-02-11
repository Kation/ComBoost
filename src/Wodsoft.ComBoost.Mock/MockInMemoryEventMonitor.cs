using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockInMemoryEventMonitor : IDisposable
    {
        public MockInMemoryEventMonitor(Type eventType)
        {
            EventType = eventType;
            _waitTaskSource = new TaskCompletionSource<bool>();
        }

        public Type EventType { get; }

        public int EventFiredCount { get; private set; }

        public void Fired()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockInMemoryEventMonitor));
            EventFiredCount++;
            _waitTaskSource.TrySetResult(true);
        }

        private TaskCompletionSource<bool> _waitTaskSource;
        public Task GetWaitTask(int timeout = 0)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockInMemoryEventMonitor));
            if (timeout < 0)
                throw new ArgumentOutOfRangeException("Timeout can not be negative.");
            if (timeout == 0)
                return _waitTaskSource.Task;
            TaskCompletionSource<bool> timeoutTask = new TaskCompletionSource<bool>();
            Task.Delay(timeout).ContinueWith(task =>
            {
                if (!_waitTaskSource.Task.IsCompleted)
                    timeoutTask.TrySetCanceled();
                else
                    timeoutTask.TrySetResult(true);
            });
            return Task.WhenAny(timeoutTask.Task, _waitTaskSource.Task);
        }

        public void ResetWaitTask()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockInMemoryEventMonitor));
            EventFiredCount = 0;
            _waitTaskSource = new TaskCompletionSource<bool>();
        }

        internal event EventHandler<EventArgs>? Disposed;
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _waitTaskSource.TrySetCanceled();
            Disposed?.Invoke(this, new EventArgs());
        }
    }
}
