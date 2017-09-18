using NUnit.Framework;
using MultithreadProducerConsumer;
using System.IO;
using Moq;
using System.Threading;

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

        [TearDown]
        public void TearDown()
        {
            _testObject.Dispose();
        }

        [Test]
        public void StartingAndStopping_ShouldNotThrowExceptions()
        {
            Assert.DoesNotThrow(() =>
            {
                _testObject.Writer = new Mock<TextWriter>().Object;
                _testObject.StartConsumer();
                _testObject.StartProducers();
            });
        }

        [Test]
        [TestCase(5, 1)]
        [TestCase(3, 5)]
        [TestCase(1, 25)]
        [TestCase(4, 50)]
        public void TextWriter_ShouldBeCalledOnce_PerProducedData_PerProducer(int producersQuantity, int producedDataQuantityPerProducer)
        {
            // ARRANGE
            var goal = producersQuantity * producedDataQuantityPerProducer;
            var counter = 0;
            var wait = new ManualResetEvent(false);

            var writerMock = new Mock<TextWriter>();
            writerMock
                .Setup(_ => _.WriteLine(It.IsAny<string>()))
                .Callback(() =>
                {
                    if (Interlocked.Increment(ref counter) >= goal)
                    {
                        wait.Set();
                    }
                });

            _testObject.ProducersQuantity = producersQuantity;
            _testObject.ProducedDataQuantityPerProducer = producedDataQuantityPerProducer;
            _testObject.Writer = writerMock.Object;

            // ACT
            _testObject.StartConsumer();
            _testObject.StartProducers();

            wait.WaitOne(1000);
            wait.Dispose();

            // ASSERT
            writerMock.Verify(_ => _.WriteLine(It.IsAny<string>()), Times.Exactly(goal));
        }
    }
}
