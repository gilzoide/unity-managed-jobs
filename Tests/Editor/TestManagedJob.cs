using System.Collections;
using NUnit.Framework;
using Unity.Jobs;
using UnityEngine.TestTools;

namespace Gilzoide.ManagedJobs.Tests.Editor
{
    public class TestManagedJob
    {
        #region Job property

        [Test]
        public void Job_WhenDefaultConstructed_ReturnsNull()
        {
            var managedJob = new ManagedJob();
            Assert.That(managedJob.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithNull_ReturnsNull()
        {
            var managedJob = new ManagedJob(null);
            Assert.That(managedJob.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithValidJob_ReturnsPassedJob()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(managedJob.Job, Is.SameAs(innerJob));
        }

        [Test]
        public void Job_WhenConstructedWithValidJobThenDisposed_ReturnsNull()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            managedJob.Dispose();
            Assert.That(managedJob.Job, Is.Null);
        }

        #endregion

        #region HasJob property

        [Test]
        public void HasJob_WhenDefaultConstructed_ReturnsFalse()
        {
            var managedJob = new ManagedJob();
            Assert.That(managedJob.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithNull_ReturnsFalse()
        {
            var managedJob = new ManagedJob(null);
            Assert.That(managedJob.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJob_ReturnsTrue()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(managedJob.HasJob, Is.True);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJobThenDisposed_ReturnsFalse()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            managedJob.Dispose();
            Assert.That(managedJob.HasJob, Is.False);
        }

        #endregion

        #region Execute method

        [Test]
        public void Execute_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob().Execute();
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob(null).Execute();
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJob.Execute();
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJobThenDisposed_ThrowsNothing()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            managedJob.Dispose();
            Assert.That(() =>
            {
                managedJob.Execute();
            }, Throws.Nothing);
        }

        #endregion

        #region Schedule method

        [Test]
        public void Schedule_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob().Schedule().Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob(null).Schedule().Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJob.Schedule().Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [UnityTest]
        public IEnumerator Schedule_DisposesOfInnerJob()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(managedJob.HasJob, Is.True);
            managedJob.Schedule().Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJob.HasJob, Is.False);
        }

        #endregion

        #region Run method

        [Test]
        public void Run_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob().Run();
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJob(null).Run();
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJob.Run();
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [Test]
        public void Run_DisposesOfInnerJob()
        {
            var innerJob = new MyClassJob();
            var managedJob = new ManagedJob(innerJob);
            Assert.That(managedJob.HasJob, Is.True);
            managedJob.Run();
            Assert.That(managedJob.HasJob, Is.False);
        }

        #endregion

        private class MyClassJob : IJob
        {
            public bool HasExecuted { get; private set; }
            public string ManagedData { get; set; } = "hello world!";

            public void Execute()
            {
                HasExecuted = true;
            }
        }
    }

}
