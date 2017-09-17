using System;
using NUnit.Framework;
using MultithreadProducerConsumer;

namespace MultithreadProducerConsumerUnitTest
{
    [TestFixture]
    public class ProducerConsumerUnitTest
    {
        private IProducerConsumer _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new ProducerConsumer();
        }

        [Test]
        public void StartingAndStoppingAConsumer_ShouldNotThrowExceptions()
        {
            Assert.DoesNotThrow(() =>
            {
                _testObject.StartConsumer();
                _testObject.StopConsumer();
            });
        }
    }
}
