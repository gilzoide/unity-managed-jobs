#if UNITY_2019_3_OR_NEWER
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
    /// It uses a <see cref="GCHandle"/> for accessing the list of managed objects.
    /// <br/>
    /// If you call <see cref="Schedule"/>, <see cref="ScheduleParallel"/> or <see cref="Run"/>, the GCHandle will be freed automatically.
    /// Otherwise, make sure to call <see cref="Dispose"/> when appropriate to free the internal object handle.
    /// </remarks>
    public struct ManagedJobFor : IJobFor, IDisposable
    {
        /// <summary>The managed job that this instance will forward job execution to.</summary>
        /// <remarks>May be null.</remarks>
        public IJobFor Job => _managedJobGcHandle.IsAllocated
            ? (IJobFor) _managedJobGcHandle.Target
            : null;

        /// <summary>Whether this instance has a managed job.</summary>
        public bool HasJob => _managedJobGcHandle.IsAllocated;

        private GCHandle _managedJobGcHandle;

        public ManagedJobFor(IJobFor managedJob)
        {
            _managedJobGcHandle = managedJob != null
                ? GCHandle.Alloc(managedJob)
                : default;
        }

        /// <summary>Executes the managed job with a specific iteration index.</summary>
        /// <remarks>If no managed job was passed, or the handle was already freed, this method is a no-op.</remarks>
        public void Execute(int index)
        {
            if (_managedJobGcHandle.IsAllocated)
            {
                ((IJobFor) _managedJobGcHandle.Target).Execute(index);
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

        /// <summary>Schedule the job for execution on a single worker thread.</summary>
        public JobHandle Schedule(int arrayLength, JobHandle dependsOn = default)
        {
            var handle = IJobForExtensions.Schedule(this, arrayLength, dependsOn);
            new DisposeJob<ManagedJobFor>(this).Schedule(handle);
            return handle;
        }

        /// <summary>Schedule the job for concurrent execution on a number of worker threads.</summary>
        public JobHandle ScheduleParallel(int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default)
        {
            var handle = IJobForExtensions.ScheduleParallel(this, arrayLength, innerloopBatchCount, dependsOn);
            new DisposeJob<ManagedJobFor>(this).Schedule(handle);
            return handle;
        }

        /// <summary>Perform the job's Execute method immediately on the main thread.</summary>
        public void Run(int arrayLength)
        {
            try
            {
                IJobForExtensions.Run(this, arrayLength);
            }
            finally
            {
                Dispose();
            }
        }

        #endregion
    }
}
#endif