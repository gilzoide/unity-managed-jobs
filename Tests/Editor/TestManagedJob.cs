using NUnit.Framework;
using Unity.Jobs;

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

        private class MyClassJob : IJob
        {
            public bool HasExecuted { get; private set; }
            public void Execute()
            {
                HasExecuted = true;
            }
        }
    }

}
