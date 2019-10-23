using System.Collections.Generic;
using System.Text;

namespace Gempoll.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        ///     判断是否为Null或者为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            if (list != null)
                return list.Count == 0;

            return true;
        }

        /// <summary>
        ///     添加唯一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void AddUnique<T>(this IList<T> list, T item)
        {
            if (list == null)
                return;

            if (!list.Contains(item))
                list.Add(item);
        }

        /// <summary>
        ///     安全地获取表项, 如果索引超出范围, 那么返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T Get<T>(this IList<T> list, int index) where T : class
        {
            if (index < 0 || index >= list.Count)
                return null;

            return list[index];
        }

        /// <summary>
        ///     模仿Java风格的toString()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Serialize<T>(this IList<T> list)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                stringBuilder.Append(item);
                if (i < list.Count - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}
