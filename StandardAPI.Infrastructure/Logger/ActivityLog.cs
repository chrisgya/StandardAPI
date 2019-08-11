using Serilog;

namespace StandardAPI.Infrastructure.Logger
{
    public static class ActivityLog<T>
    {
        public static void Logger(T value)
        {
            Log.Information("{Activity}", value);
        }

        public static void Logger(string prefix, T value)
        {
            Log.Information("{prefix} {Activity}", prefix, value);
        }

        public static void Logger(string prefix, T value, string suffice)
        {
            Log.Information("{prefix} {Activity} {suffice}", prefix, value, suffice);
        }
    }
}
