using System;

namespace Wintellect.Extensions
{
    public static class BooleanExtensions
    {
        public static void ThrowIfTrue(this bool b)
        {
            if (b) throw new Exception();
        }

        public static void ThrowIfTrue(this bool b, Exception exception)
        {
            if (b) throw exception;
        }

        public static void ThrowIfTrue(this bool b, string message)
        {
            if (b) throw new Exception(message);
        }

        public static void ThrowIfTrue(this bool b, string message, params object[] args)
        {
            if (b) throw new Exception(string.Format(message, args));
        }

        public static void ThrowIfTrue(this bool b, Exception innerException, string message, params object[] args)
        {
            if (b) throw new Exception(string.Format(message, args), innerException);
        }

        public static void ThrowIfFalse(this bool b)
        {
            if (!b) throw new Exception();
        }

        public static void ThrowIfFalse(this bool b, Exception exception)
        {
            if (!b) throw exception;
        }

        public static void ThrowIfFalse(this bool b, string message)
        {
            if (!b) throw new Exception(message);
        }

        public static void ThrowIfFalse(this bool b, string message, params object[] args)
        {
            if (!b) throw new Exception(string.Format(message, args));
        }

        public static void ThrowIfFalse(this bool b, Exception innerException, string message, params object[] args)
        {
            if (!b) throw new Exception(string.Format(message, args), innerException);
        }
    }
}