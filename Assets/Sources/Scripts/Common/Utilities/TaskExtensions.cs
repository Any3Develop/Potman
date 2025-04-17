using System;
using System.Threading.Tasks;

namespace Potman.Common.Utilities
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs a task without awaiting and safely handles exceptions.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="onException">Optional delegate for handling exceptions.</param>
        public static void Forget(this Task task, Action<Exception> onException)
        {
            task?.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    onException?.Invoke(t.Exception.GetBaseException());
                }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Runs a task without awaiting and ignores exceptions.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        public static void Forget(this Task task)
        {
            task?.ContinueWith(_ =>
            { /* Ignore exceptions */
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Runs a task with a result without awaiting and safely handles exceptions.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task to execute.</param>
        /// <param name="onException">Optional delegate for handling exceptions.</param>
        public static void Forget<T>(this Task<T> task, Action<Exception> onException)
        {
            task?.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    onException?.Invoke(t.Exception.GetBaseException());
                }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Runs a task with a result without awaiting and ignores exceptions.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task to execute.</param>
        public static void Forget<T>(this Task<T> task)
        {
            task?.ContinueWith(_ =>
            { /* Ignore exceptions */
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}