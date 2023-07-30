using System.Threading;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Gilzoide.ManagedJobs.Tests.Editor
{
    public class CounterManagedJob : IJob, IJobFor, IJobParallelFor, IJobParallelForTransform
    {
        public int Counter => _counter;
        public bool HasExecuted => _counter > 0;

        private int _counter;

        #pragma warning disable CS0414
        private string _managedData = "hello world!";
        #pragma warning restore CS0414

        public void Execute()
        {
            _counter++;
        }

        public void Execute(int index)
        {
            Interlocked.Increment(ref _counter);
        }

        public void Execute(int index, TransformAccess transform)
        {
            Interlocked.Increment(ref _counter);
        }
    }
}
