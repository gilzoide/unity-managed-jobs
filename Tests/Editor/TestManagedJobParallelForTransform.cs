using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.TestTools;

namespace Gilzoide.ManagedJobs.Tests.Editor
{
    public class TestManagedJobParallelForTransform
    {
        public const int IterationCount = 100;
        public const int BatchSize = 10;

        #region Fixture setup

        private static TransformAccessArray _transforms;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            TransformAccessArray.Allocate(IterationCount, BatchSize, out _transforms);
            for (int i = 0; i < IterationCount; i++)
            {
                _transforms.Add(new GameObject($"{nameof(TestManagedJobParallelForTransform)} {i}").transform);
            }
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            for (int i = 0; i < _transforms.length; i++)
            {
                Object.DestroyImmediate(_transforms[i].gameObject);
            }
            _transforms.Dispose();
        }

        #endregion

        #region Job property

        [Test]
        public void Job_WhenDefaultConstructed_ReturnsNull()
        {
            var managedJobParallelForTransform = new ManagedJobParallelForTransform();
            Assert.That(managedJobParallelForTransform.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithNull_ReturnsNull()
        {
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(null);
            Assert.That(managedJobParallelForTransform.Job, Is.Null);
        }

        [Test]
        public void Job_WhenConstructedWithValidJob_ReturnsPassedJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(managedJobParallelForTransform.Job, Is.SameAs(innerJob));
        }

        [Test]
        public void Job_WhenConstructedWithValidJobThenDisposed_ReturnsNull()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            managedJobParallelForTransform.Dispose();
            Assert.That(managedJobParallelForTransform.Job, Is.Null);
        }

        #endregion

        #region HasJob property

        [Test]
        public void HasJob_WhenDefaultConstructed_ReturnsFalse()
        {
            var managedJobParallelForTransform = new ManagedJobParallelForTransform();
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithNull_ReturnsFalse()
        {
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(null);
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJob_ReturnsTrue()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(managedJobParallelForTransform.HasJob, Is.True);
        }

        [Test]
        public void HasJob_WhenConstructedWithValidJobThenDisposed_ReturnsFalse()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            managedJobParallelForTransform.Dispose();
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        #endregion

        #region Execute method

        [Test]
        public void Execute_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform().Execute(0, default);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform(null).Execute(0, default);
            }, Throws.Nothing);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelForTransform.Execute(0, default);
            Assert.That(innerJob.HasExecuted, Is.True);
        }

        [Test]
        public void Execute_WhenConstructedWithValidJobThenDisposed_ThrowsNothing()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            managedJobParallelForTransform.Dispose();
            Assert.That(() =>
            {
                managedJobParallelForTransform.Execute(0, default);
            }, Throws.Nothing);
        }

        #endregion

        #region Schedule method

        [Test]
        public void Schedule_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform().Schedule(_transforms).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform(null).Schedule(_transforms).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void Schedule_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelForTransform.Schedule(_transforms).Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [UnityTest]
        public IEnumerator Schedule_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(managedJobParallelForTransform.HasJob, Is.True);
            managedJobParallelForTransform.Schedule(_transforms).Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        #endregion

        #region ScheduleReadOnly method

        [Test]
        public void ScheduleReadOnly_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform().ScheduleReadOnly(_transforms, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void ScheduleReadOnly_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform(null).ScheduleReadOnly(_transforms, BatchSize).Complete();
            }, Throws.Nothing);
        }

        [Test]
        public void ScheduleReadOnly_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelForTransform.ScheduleReadOnly(_transforms, BatchSize).Complete();
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [UnityTest]
        public IEnumerator ScheduleReadOnly_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(managedJobParallelForTransform.HasJob, Is.True);
            managedJobParallelForTransform.ScheduleReadOnly(_transforms, BatchSize).Complete();
            yield return null;  // give time for the DisposeJob to complete
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        #endregion

        #region RunReadOnly method

        [Test]
        public void Run_WhenDefaultConstructed_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform().RunReadOnly(_transforms);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithNull_ThrowsNothing()
        {
            Assert.That(() =>
            {
                new ManagedJobParallelForTransform(null).RunReadOnly(_transforms);
            }, Throws.Nothing);
        }

        [Test]
        public void Run_WhenConstructedWithValidJob_RunsInnerJobExecute()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(innerJob.HasExecuted, Is.False);
            managedJobParallelForTransform.RunReadOnly(_transforms);
            Assert.That(innerJob.HasExecuted, Is.True);
            Assert.That(innerJob.Counter, Is.EqualTo(IterationCount));
        }

        [Test]
        public void Run_DisposesOfInnerJob()
        {
            var innerJob = new CounterManagedJob();
            var managedJobParallelForTransform = new ManagedJobParallelForTransform(innerJob);
            Assert.That(managedJobParallelForTransform.HasJob, Is.True);
            managedJobParallelForTransform.RunReadOnly(_transforms);
            Assert.That(managedJobParallelForTransform.HasJob, Is.False);
        }

        #endregion
    }
}
