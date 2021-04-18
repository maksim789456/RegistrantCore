using System;

namespace CoreUpdater
{
    public static class Logger
    {
        public static void LogWithTime(string text) => Console.WriteLine($"{DateTime.Now} > {text}");

        public static void EmptyString() => Console.WriteLine();
    }
}