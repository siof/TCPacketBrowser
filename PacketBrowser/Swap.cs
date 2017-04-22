namespace PacketBrowser
{
    public static class Helpers
    {
        public static void Swap<T>(ref T obj1, ref T obj2) where T : class
        {
            T tmpObj = obj1;
            obj1 = obj2;
            obj2 = tmpObj;
            tmpObj = null;
        }
    }
}
