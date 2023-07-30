using System;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs
{
    public struct DisposeJob<TJob> : IJob where TJob : struct, IDisposable
    {
        public TJob Disposable { get; }

        public DisposeJob(TJob disposable)
        {
            Disposable = disposable;
        }

        public void Execute()
        {
            Disposable.Dispose();
        }
    }
}
