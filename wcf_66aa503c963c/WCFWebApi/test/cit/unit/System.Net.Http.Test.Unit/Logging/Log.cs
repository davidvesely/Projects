using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace System.Net.Test.Common.Logging
{
    public static class Log
    {
        private static ReaderWriterLockSlim lockObject;
        private static List<ILogger> loggerList;

        static Log()
        {
            lockObject = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            loggerList = new List<ILogger>();

            AddLogger(new TraceLogger()); // default logger
            LogTestRunInfo();
        }

        public static void AddLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            lockObject.EnterWriteLock();
            try
            {
                loggerList.Add(logger);
            }
            finally
            {
                lockObject.ExitWriteLock();
            }

            Info("Added logger: {0}", logger.ToString());
        }

        public static void AddLogger(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName))
            {
                throw new ArgumentNullException("loggerName");
            }

            // currently support only loading loggers from common assembly. Consider adding the assembly name
            // to the config file and load the corresponding assembly.
            Assembly current = Assembly.GetAssembly(typeof(Log));
            Type defaultLoggerType = current.GetType(loggerName);

            if (defaultLoggerType == null)
            {
                throw new InvalidOperationException(TestUtils.Format(
                    "Can't load logger type: '{0}'", loggerName));
            }

            ILogger defaultLogger = Activator.CreateInstance(defaultLoggerType) as ILogger;

            if (defaultLogger == null)
            {
                throw new InvalidOperationException(TestUtils.Format(
                    "Unknown logger type found in app.config (DefaultLogger): '{0}'",
                    loggerName));
            }

            AddLogger(defaultLogger);
        }

        public static void Info(string message)
        {
            Write(LogLevel.Info, message, null);
        }

        public static void Info(string message, object arg)
        {
            Write(LogLevel.Info, message, arg);
        }

        public static void Info(string message, params object[] args)
        {
            Write(LogLevel.Info, message, args);
        }

        public static void Warn(string message)
        {
            Write(LogLevel.Warn, message, null);
        }

        public static void Warn(string message, object arg)
        {
            Write(LogLevel.Warn, message, arg);
        }

        public static void Warn(string message, params object[] args)
        {
            Write(LogLevel.Warn, message, args);
        }

        public static void Error(string message)
        {
            Write(LogLevel.Error, message, null);
        }

        public static void Error(string message, object arg)
        {
            Write(LogLevel.Error, message, arg);
        }

        public static void Error(string message, params object[] args)
        {
            Write(LogLevel.Error, message, args);
        }

        public static void Exception(Exception e)
        {
            lockObject.EnterReadLock();
            try
            {
                foreach (ILogger logger in loggerList)
                {
                    logger.DumpException(e);
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        private static void LogTestRunInfo()
        {
            Assembly asm = Assembly.GetAssembly(typeof(Log));

            Info("Test run binaries location: '{0}'", Path.GetDirectoryName(asm.Location));
        }

        private static void Write(LogLevel level, string message, params object[] args)
        {
            lockObject.EnterReadLock();
            try
            {
                if (args == null)
                {
                    foreach (ILogger logger in loggerList)
                    {
                        logger.WriteLine(level, message);
                    }
                }
                else
                {
                    foreach (ILogger logger in loggerList)
                    {
                        logger.WriteLine(level, TestUtils.Format(message, args));
                    }
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
    }
}
