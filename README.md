# Managed Jobs
[![openupm](https://img.shields.io/npm/v/com.gilzoide.managed-jobs?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gilzoide.managed-jobs/)

Use classes and other managed types with [Unity's C# Job System](https://docs.unity3d.com/Manual/JobSystemOverview.html).

The Job System only accepts [blittable](https://en.wikipedia.org/wiki/Blittable_types) struct types for jobs.
This package makes it easy to use managed types as jobs by providing blittable structs that reference managed objects using `GCHandle` and forward job execution to them.


## Features
- Easily schedule jobs implemented by class types, as well as struct types with managed fields
- Schedule managed `IJob` types using [ManagedJob](Runtime/ManagedJob.cs)
- Schedule managed `IJobFor` types using [ManagedJobFor](Runtime/ManagedJobFor.cs)
- Schedule managed `IJobParallelFor` types using [ManagedJobParallelFor](Runtime/ManagedJobParallelFor.cs)
- Schedule managed `IJobParallelForTransform` types using [ManagedJobParallelForTransform](Runtime/ManagedJobParallelForTransform.cs)
- Automatic disposal of the allocated `GCHandle` if you call `Schedule` / `Run` methods and their variations directly on the wrapper structs


## Caveats
- Managed jobs are not compatible with [Burst](https://docs.unity3d.com/Packages/com.unity.burst@latest)


## Installing
Either:
- Use the [openupm registry](https://openupm.com/) and install this package using the [openupm-cli](https://github.com/openupm/openupm-cli):
  ```
  openupm add com.gilzoide.managed-jobs
  ```
- Install via [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using this repository URL and tag:
  ```
  https://github.com/gilzoide/unity-managed-jobs.git#1.0.0
  ```
- Clone this repository directly inside your project's `Assets` or `Packages` folder.

## Basic Usage
```cs
using Unity.Jobs;
using Gilzoide.ManagedJobs;

// 1. Create your managed job type
public class MyManagedJobClass : IJob
{
    public string Message = "Fields with managed types are supported!";

    public void Execute()
    {
        Debug.Log($"Job is being executed! Here's the message: '{Message}'");
    }
}

// 2. Schedule the job by using the wrapper ManagedJob struct type
var myManagedJobObject = new MyManagedJobClass();
var jobHandle = new ManagedJob(myManagedJobObject).Schedule();
// 3. Complete the jobHandle or use it as dependency to other jobs as usual
jobHandle.Complete();

// 4. Enjoy 🍾
```