using System;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs
{
    /// <summary>Job that disposes the IDisposable struct passed on its constructor.</summary>
    /// <remarks>Use this with Job dependencies to dispose of a job struct after its job is completed.</remarks>
    public struct DisposeJob<TDisposable> : IJob where TDisposable : struct, IDisposable
    {
        public TDisposable Disposable { get; }

        public DisposeJob(TDisposable disposable)
        {
            Disposable = disposable;
        }

        public void Execute()
        {
            Disposable.Dispose();
        }
    }
}
