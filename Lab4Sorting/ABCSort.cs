using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4Sorting
{
    static class ABCSort
    {
        public static List<string> ABCSorting(List<string> collection, int rank)
        {
            if (collection.Count < 2) return collection;

            Dictionary<char, List<string>> table = new Dictionary<char, List<string>>(); // key - символ в позиции rank, список слов с символом key в позиции rank
            List<string> listResult = new List<string>();
            int shortWordsCounter = 0;

            foreach (var str in collection)
            {
                if (rank < str.Length)
                {
                    if (table.ContainsKey(str[rank]))
                    {
                        table[str[rank]].Add(str);
                    }
                    else
                    {
                        table.Add(str[rank], new List<string> { str });
                    }
                }
                else
                {
                    listResult.Add(str);
                    shortWordsCounter++;
                }
            }

            if (shortWordsCounter == collection.Count) return collection;

            for (var i = 'A'; i <= 'z'; i++)
            {
                if (table.ContainsKey(i))
                {
                    foreach (string str in ABCSorting(table[i], rank + 1))
                    {
                        listResult.Add(str);
                    }
                }
            }

            return listResult;
        }
    }
}
