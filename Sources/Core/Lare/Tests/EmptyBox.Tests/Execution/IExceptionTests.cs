using EmptyBox.Execution;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmptyBox.Tests.Execution;

[TestClass]
public sealed class IExceptionTests
{
    private const string MESSAGE = "___TEST MESSAGE___";

    [TestMethod]
    public void MessageSettingCorrectness()
    {
        static void IterationBody<E>(string message)
            where E : Exception, new()
        {
            try
            {
                IException.Throw<E>(message);
            }
            catch (E exception) when (exception.Message != message)
            {
                Assert.Fail();
            }
            catch
            {

            }
        }


        Action<string> iterationBodyDelegate = IterationBody<Exception>;
        MethodInfo iterationBodyInfo = iterationBodyDelegate.Method.GetGenericMethodDefinition();
        IEnumerable<Type> exceptionTypes = from type in typeof(object).Assembly.GetTypes()
                                           where type.IsAssignableTo(typeof(Exception)) && type.GetConstructors().Any(static constructor => constructor.GetParameters().Length == 0)
                                           select type;

        foreach (Type exceptionType in exceptionTypes)
        {
            iterationBodyDelegate = iterationBodyInfo.MakeGenericMethod(exceptionType).CreateDelegate<Action<string>>();
            iterationBodyDelegate(MESSAGE);
        }
    }

    [TestMethod]
    public void InnerExceptionSettingCorrectness()
    {
        static void IterationBody<E>(Exception innerException)
            where E : Exception, new()
        {
            try
            {
                IException.Throw<E>(innerException: innerException);
            }
            catch (E exception) when (exception.InnerException != innerException)
            {
                Assert.Fail();
            }
            catch (AggregateException aggregate) when (aggregate.InnerExceptions.Count != 1 || aggregate.InnerExceptions[0] != innerException)
            {
                Assert.Fail();
            }
            catch
            {

            }
        }

        Exception innerException = new(MESSAGE);
        Action<Exception> iterationBodyDelegate = IterationBody<Exception>;
        MethodInfo iterationBodyInfo = iterationBodyDelegate.Method.GetGenericMethodDefinition();
        IEnumerable<Type> exceptionTypes = from type in typeof(object).Assembly.GetTypes()
                                           where type.IsAssignableTo(typeof(Exception)) && type.GetConstructors().Any(static constructor => constructor.GetParameters().Length == 0)
                                           select type;

        foreach (Type exceptionType in exceptionTypes)
        {
            iterationBodyDelegate = iterationBodyInfo.MakeGenericMethod(exceptionType).CreateDelegate<Action<Exception>>();
            iterationBodyDelegate(innerException);
        }
    }
}
