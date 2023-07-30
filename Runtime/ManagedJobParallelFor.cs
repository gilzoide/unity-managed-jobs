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
    /// If you call <see cref="Schedule"/> or <see cref="Run"/>, the GCHandle will be freed automatically.
    /// Otherwise, make sure to call <see cref="Dispose"/> when appropriate to free the internal object handle.
    /// </remarks>
    public struct ManagedJobParallelFor : IJobParallelFor, IDisposable
    {
        /// <summary>The managed job that this instance will forward job execution to.</summary>
        /// <remarks>May be null.</remarks>
        public IJobParallelFor Job => _managedJobGcHandle.IsAllocated
            ? (IJobParallelFor) _managedJobGcHandle.Target
            : null;

        /// <summary>Whether this instance has a managed job.</summary>
        public bool HasJob => _managedJobGcHandle.IsAllocated;

        private readonly GCHandle _managedJobGcHandle;

        public ManagedJobParallelFor(IJobParallelFor managedJob)
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
                ((IJobParallelFor) _managedJobGcHandle.Target).Execute(index);
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

        /// <summary>Schedule the job for concurrent execution on a number of worker threads.</summary>
        public JobHandle Schedule(int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default)
        {
            var handle = IJobParallelForExtensions.Schedule(this, arrayLength, innerloopBatchCount, dependsOn);
            new DisposeJob<ManagedJobParallelFor>(this).Schedule(handle);
            return handle;
        }

        /// <summary>Perform the job's Execute method immediately on the main thread.</summary>
        public void Run(int arrayLength)
        {
            try
            {
                IJobParallelForExtensions.Run(this, arrayLength);
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