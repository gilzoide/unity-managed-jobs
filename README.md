# Managed Jobs
Use classes and other managed types with [Unity's Job System](https://docs.unity3d.com/Manual/JobSystemOverview.html).


## Features
- Easily schedule jobs implemented by class types, as well as struct types with managed fields
- Use [ManagedJob](Runtime/ManagedJob.cs) for managed `IJob` types
- Use [ManagedJobFor](Runtime/ManagedJobFor.cs) for managed `IJobFor` types
- Use [ManagedJobParallelFor](Runtime/ManagedJobParallelFor.cs) for managed `IJobParallelFor` types
- Use [ManagedJobParallelForTransform](Runtime/ManagedJobParallelForTransform.cs) for managed `IJobParallelForTransform` types
- Automatic disposal of the allocated `GCHandle` if you call `Schedule` / `Run` methods and their variations directly on the wrapper structs


## Caveats
- Managed jobs are not compatible with [Burst](https://docs.unity3d.com/Packages/com.unity.burst@latest)


## Usage
```cs
using Unity.Jobs;
using Gilzoide.ManagedJobs;

// 1. Create you managed job type
public class MyManagedJobClass : IJob
{
    public string Message = "Any managed types are supported!";

    public void Execute()
    {
        Debug.Log($"Job is being executed! Here's the message: '{Message}'");
    }
}

// 2. Schedule the job by using the wrapper ManagedJob struct type
var myManagedJobObject = new MyManagedJobClass();
var jobHandle = new ManagedJob(myManagedJobObject).Schedule();
// 3. Complete the jobHandle or use it as dependency to other jobs as usual
// ...
```