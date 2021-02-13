using Gateway.Util;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Test.Util
{
    [TestFixture]
    class FlowUtilTest
    {
        const string expectedResult = "testResult";

        [Test]
        public void TryCatchFinallyTest()
        {
            {
                var onErrorMock = new Mock<Func<Exception, string>>();
                var finallyMock = new Mock<Action>();

                var result = FlowUtil.TryCatchFinally<string, Exception>(
                    () => expectedResult,
                    onErrorMock.Object,
                    finallyMock.Object);

                Assert.AreEqual(expectedResult, result);
                onErrorMock.VerifyNoOtherCalls();
                finallyMock.Verify(f => f.Invoke());
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();
                var finallyMock = new Mock<Action>();

                var result = FlowUtil.TryCatchFinally<string, Exception>(
                    () => throw new Exception(),
                    onErrorMock.Object,
                    finallyMock.Object);

                Assert.IsNull(result);
                onErrorMock.Verify(f => f.Invoke(It.IsAny<Exception>()));
                finallyMock.Verify(f => f.Invoke());
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();
                var finallyMock = new Mock<Action>();

                var result = FlowUtil.TryCatchFinally(
                    () => expectedResult,
                    onErrorMock.Object,
                    finallyMock.Object);

                Assert.AreEqual(expectedResult, result);
                onErrorMock.VerifyNoOtherCalls();
                finallyMock.Verify(f => f.Invoke());
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();
                var finallyMock = new Mock<Action>();

                var result = FlowUtil.TryCatchFinally(
                    () => throw new Exception(),
                    onErrorMock.Object,
                    finallyMock.Object);

                Assert.IsNull(result);
                onErrorMock.Verify(f => f.Invoke(It.IsAny<Exception>()));
                finallyMock.Verify(f => f.Invoke());
            }
        }

        [Test]
        public void TryCatchTest()
        {
            {
                var onErrorMock = new Mock<Func<Exception, string>>();

                var result = FlowUtil.TryCatch(
                    () => expectedResult,
                    onErrorMock.Object);

                Assert.AreEqual(expectedResult, result);
                onErrorMock.VerifyNoOtherCalls();
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();

                var result = FlowUtil.TryCatch(
                    () => throw new Exception(),
                    onErrorMock.Object);

                Assert.IsNull(result);
                onErrorMock.Verify(f => f.Invoke(It.IsAny<Exception>()));
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();

                var result = FlowUtil.TryCatch(
                    () => expectedResult,
                    onErrorMock.Object);

                Assert.AreEqual(expectedResult, result);
                onErrorMock.VerifyNoOtherCalls();
            }

            {
                var onErrorMock = new Mock<Func<Exception, string>>();

                var result = FlowUtil.TryCatch(
                    () => throw new Exception(),
                    onErrorMock.Object);

                Assert.IsNull(result);
                onErrorMock.Verify(f => f.Invoke(It.IsAny<Exception>()));
            }
        }
    }
}
