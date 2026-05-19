namespace MonitorBoard.WPF.Common
{
    public static class NullOrEmptyExpansion
    {
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNull(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }


        public static bool IsNullOrSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
