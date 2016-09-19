using System;

namespace Wintellect.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool IsNotNull(this object o)
        {
            return o != null;
        }

        public static void ThrowIfNull(this object o)
        {
            if (o == null) throw new Exception();
        }

        public static void ThrowIfNull(this object o, Exception exception)
        {
            if (o == null) throw exception;
        }

        public static void ThrowIfNull(this object o, string message)
        {
            if (o == null) throw new Exception(message);
        }

        public static void ThrowIfNull(this object o, string message, params object[] args)
        {
            if (o == null) throw new Exception(string.Format(message, args));
        }

        public static void ThrowIfNull(this object o, Exception innerException, string message, params object[] args)
        {
            if (o == null) throw new Exception(string.Format(message, args), innerException);
        }
    }
}
