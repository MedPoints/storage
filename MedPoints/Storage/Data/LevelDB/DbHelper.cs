using System.Text;
using Newtonsoft.Json;

namespace Storage.Data.LevelDB
{
    public static class DbHelper
    {
        public static Slice ToSlice<T>(this T t)
        {
            var converted = JsonConvert.SerializeObject(t);
            return (Slice)Encoding.UTF8.GetBytes(converted);
        }

        public static Slice ToSlice<T>(this string input)
        {
            return (Slice)Encoding.UTF8.GetBytes(input);
        }
    }
}
