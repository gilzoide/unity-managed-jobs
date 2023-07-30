using System;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs
{
    /// <summary>
    /// A job structure that forwards execution to the managed job passed on its constructor.
    /// The managed job may be of class types, as well as struct types with managed fields.
    /// </summary>
    /// <remarks>
    /// It uses a <see cref="GCHandle"/> for accessing the managed object.
    /// <br/>
    /// If you call <see cref="Schedule"/> or <see cref="Run"/>, the GCHandle will be freed automatically.
    /// Otherwise, make sure to call <see cref="Dispose"/> when appropriate to free the internal object handle.
    /// </remarks>
    public struct ManagedJob : IJob, IDisposable
    {
        /// <summary>The managed job that this instance will forward job execution to.</summary>
        /// <remarks>May be null.</remarks>
        public IJob Job => _managedJobGcHandle.IsAllocated
            ? (IJob) _managedJobGcHandle.Target
            : null;

        /// <summary>Whether this instance has a managed job.</summary>
        public bool HasJob => _managedJobGcHandle.IsAllocated;

        private readonly GCHandle _managedJobGcHandle;

        public ManagedJob(IJob managedJob)
        {
            _managedJobGcHandle = managedJob != null
                ? GCHandle.Alloc(managedJob)
                : default;
        }

        /// <summary>Executes the managed job.</summary>
        /// <remarks>If no managed job was passed, or the handle was already freed, this method is a no-op.</remarks>
        public void Execute()
        {
            if (_managedJobGcHandle.IsAllocated)
            {
                ((IJob) _managedJobGcHandle.Target).Execute();
            }
        }

        /// <summary>Frees the internal object GCHandle.</summary>
        /// <remarks>If no managed job was passed, or the handle was already freed, this method is a no-op.</remarks>
        public void Dispose()
        {
            if (_managedJobGcHandle.IsAllocated)
            {
                _managedJobGcHandle.Free();
            }
        }

        #region Job scheduling/running

        /// <summary>Schedule the job for execution on a worker thread.</summary>
        public JobHandle Schedule(JobHandle dependsOn = default)
        {
            var handle = IJobExtensions.Schedule(this, dependsOn);
            new DisposeJob<ManagedJob>(this).Schedule(handle);
            return handle;
        }

        /// <summary>Perform the job's Execute method immediately on the same thread.</summary>
        public void Run()
        {
            try
            {
                IJobExtensions.Run(this);
            }
            finally
            {
                Dispose();
            }
        }

        #endregion
    }
}
