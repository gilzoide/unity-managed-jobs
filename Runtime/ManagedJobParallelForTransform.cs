using System;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Gilzoide.ManagedJobs
{
    /// <summary>
    /// A job structure that forwards execution to the managed job passed on its constructor.
    /// The managed job may be of class types, as well as struct types with managed fields.
    /// </summary>
    /// <remarks>
    /// It uses a <see cref="GCHandle"/> for accessing the list of managed objects.
    /// <br/>
    /// If you call <see cref="Schedule"/>, <see cref="ScheduleReadOnly"/> or <see cref="RunReadOnly"/>, the GCHandle will be freed automatically.
    /// Otherwise, make sure to call <see cref="Dispose"/> when appropriate to free the internal object handle.
    /// </remarks>
    public struct ManagedJobParallelForTransform : IJobParallelForTransform, IDisposable
    {
        /// <summary>The managed job that this instance will forward job execution to.</summary>
        /// <remarks>May be null.</remarks>
        public IJobParallelForTransform Job => _managedJobGcHandle.IsAllocated
            ? (IJobParallelForTransform) _managedJobGcHandle.Target
            : null;

        /// <summary>Whether this instance has a managed job.</summary>
        public bool HasJob => Job != null;

        private GCHandle _managedJobGcHandle;

        public ManagedJobParallelForTransform(IJobParallelForTransform managedJob)
        {
            _managedJobGcHandle = managedJob != null
                ? GCHandle.Alloc(managedJob)
                : default;
        }

        /// <summary>Executes the managed job with a specific iteration index.</summary>
        /// <remarks>If no managed job was passed, or the handle was already freed, this method is a no-op.</remarks>
        public void Execute(int index, TransformAccess transform)
        {
            Job?.Execute(index, transform);
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

        /// <summary>
        /// Schedule the job with read-write access to the transform data.
        /// This method parallelizes access to transforms in different hierarchies.
        /// Transforms with a shared root object are always processed on the same thread.
        /// </summary>
        public JobHandle Schedule(TransformAccessArray transforms, JobHandle dependsOn = default)
        {
            var jobHandle = IJobParallelForTransformExtensions.Schedule(this, transforms, dependsOn);
            new DisposeJob<ManagedJobParallelForTransform>(this).Schedule(jobHandle);
            return jobHandle;
        }

        /// <summary>
        /// Schedule the job with read-only access to the transform data.
        /// This method provides better parallelization because it can read all transforms in parallel instead of just parallelizing over different hierarchies.
        /// </summary>
        public JobHandle ScheduleReadOnly(TransformAccessArray transforms, int batchSize, JobHandle dependsOn = default)
        {
            var jobHandle = IJobParallelForTransformExtensions.ScheduleReadOnly(this, transforms, batchSize, dependsOn);
            new DisposeJob<ManagedJobParallelForTransform>(this).Schedule(jobHandle);
            return jobHandle;
        }

        /// <summary>
        /// Run the job with read-only access to the transform data.
        /// This method makes the job run on the calling thread instead of spreading it out over multiple threads.
        /// </summary>
        public void RunReadOnly(TransformAccessArray transforms)
        {
            try
            {
                IJobParallelForTransformExtensions.RunReadOnly(this, transforms);
            }
            finally
            {
                Dispose();
            }
        }

        #endregion
    }
}