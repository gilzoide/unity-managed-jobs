using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gilzoide.ManagedJobs.Tests.Editor
{
    public class TestManagedJobFor
    {
        public const int IterationCount = 100;
        public const int BatchSize = 10;

        #region Job property

        [Test]
        public void Job_WhenDefaultConstructed_ReturnsNull()
        {
            var managedJobFor = new ManagedJobFor();
            Assert.That(managedJobFor.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithNull_ReturnsNull()
        {
            var managedJobFor = new ManagedJobFor(null);
            Assert.That(managedJobFor.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithValidJob_ReturnsPassedJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(managedJobFor.Job, Is.SameAs(innerJob));
        }

        [Test]
        public void Job_WhenConstructedWithValidJobThenDisposed_ReturnsNull()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            managedJobFor.Dispose();
            Assert.That(managedJobFor.Job, Is.Null);
        }

        #endregion

        #region HasJob property

        [Test]
        public void HasJob_WhenDefaultConstructed_ReturnsFalse()
        {
            var managedJobFor = new ManagedJobFor();
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithNull_ReturnsFalse()
        {
            var managedJobFor = new ManagedJobFor(null);
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJob_ReturnsTrue()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(managedJobFor.HasJob, Is.True);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJobThenDisposed_ReturnsFalse()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            managedJobFor.Dispose();
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        #endregion

        #region Execute method

        [Test]
        public void Execute_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor().Execute(0);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor(null).Execute(0);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobFor.Execute(0);
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJobThenDisposed_ThrowsNothing()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            managedJobFor.Dispose();
            Assert.That(() =>
            {
                managedJobFor.Execute(0);
            }, Throws.Nothing);
        }

        #endregion

        #region Schedule method

        [Test]
        public void Schedule_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor().Schedule(IterationCount).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor(null).Schedule(IterationCount).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobFor.Schedule(IterationCount).Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [UnityTest]
        public IEnumerator Schedule_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(managedJobFor.HasJob, Is.True);
            managedJobFor.Schedule(IterationCount).Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        #endregion

        #region ScheduleParallel method

        [Test]
        public void ScheduleParallel_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor().ScheduleParallel(IterationCount, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void ScheduleParallel_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor(null).ScheduleParallel(IterationCount, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void ScheduleParallel_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobFor.ScheduleParallel(IterationCount, BatchSize).Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [UnityTest]
        public IEnumerator ScheduleParallel_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(managedJobFor.HasJob, Is.True);
            managedJobFor.ScheduleParallel(IterationCount, BatchSize).Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        #endregion

        #region Run method

        [Test]
        public void Run_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor().Run(IterationCount);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobFor(null).Run(IterationCount);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobFor.Run(IterationCount);
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [Test]
        public void Run_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobFor = new ManagedJobFor(innerJob);
            Assert.That(managedJobFor.HasJob, Is.True);
            managedJobFor.Run(IterationCount);
            Assert.That(managedJobFor.HasJob, Is.False);
        }

        #endregion
    }
}
