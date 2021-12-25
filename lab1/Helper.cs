using System;
using System.Collections.Generic;

namespace lab1
{
    public class Helper<T>
    {
        public static T SelectRandomItem(List<T> list)
        {
            int size = list.Count;
            int index = new Random().Next(size);
            int counter = 0;

            foreach (T item in list) {
                if (counter == index) {
                    return item;
                }

                counter++;
            }

            return default;
        }
    }
}