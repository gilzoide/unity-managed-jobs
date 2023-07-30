using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gilzoide.ManagedJobs.Tests.Editor
{
    public class TestManagedJobParallelFor
    {
        public const int IterationCount = 100;
        public const int BatchSize = 10;

        #region Job property

        [Test]
        public void Job_WhenDefaultConstructed_ReturnsNull()
        {
            var managedJobParallelFor = new ManagedJobParallelFor();
            Assert.That(managedJobParallelFor.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithNull_ReturnsNull()
        {
            var managedJobParallelFor = new ManagedJobParallelFor(null);
            Assert.That(managedJobParallelFor.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithValidJob_ReturnsPassedJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(managedJobParallelFor.Job, Is.SameAs(innerJob));
        }

        [Test]
        public void Job_WhenConstructedWithValidJobThenDisposed_ReturnsNull()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            managedJobParallelFor.Dispose();
            Assert.That(managedJobParallelFor.Job, Is.Null);
        }

        #endregion

        #region HasJob property

        [Test]
        public void HasJob_WhenDefaultConstructed_ReturnsFalse()
        {
            var managedJobParallelFor = new ManagedJobParallelFor();
            Assert.That(managedJobParallelFor.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithNull_ReturnsFalse()
        {
            var managedJobParallelFor = new ManagedJobParallelFor(null);
            Assert.That(managedJobParallelFor.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJob_ReturnsTrue()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(managedJobParallelFor.HasJob, Is.True);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJobThenDisposed_ReturnsFalse()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            managedJobParallelFor.Dispose();
            Assert.That(managedJobParallelFor.HasJob, Is.False);
        }

        #endregion

        #region Execute method

        [Test]
        public void Execute_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor().Execute(0);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor(null).Execute(0);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelFor.Execute(0);
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJobThenDisposed_ThrowsNothing()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            managedJobParallelFor.Dispose();
            Assert.That(() =>
            {
                managedJobParallelFor.Execute(0);
            }, Throws.Nothing);
        }

        #endregion

        #region Schedule method

        [Test]
        public void Schedule_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor().Schedule(IterationCount, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor(null).Schedule(IterationCount, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelFor.Schedule(IterationCount, BatchSize).Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [UnityTest]
        public IEnumerator Schedule_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(managedJobParallelFor.HasJob, Is.True);
            managedJobParallelFor.Schedule(IterationCount, BatchSize).Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJobParallelFor.HasJob, Is.False);
        }

        #endregion

        #region Run method

        [Test]
        public void Run_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor().Run(IterationCount);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelFor(null).Run(IterationCount);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelFor.Run(IterationCount);
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [Test]
        public void Run_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelFor = new ManagedJobParallelFor(innerJob);
            Assert.That(managedJobParallelFor.HasJob, Is.True);
            managedJobParallelFor.Run(IterationCount);
            Assert.That(managedJobParallelFor.HasJob, Is.False);
        }

        #endregion
    }
}
