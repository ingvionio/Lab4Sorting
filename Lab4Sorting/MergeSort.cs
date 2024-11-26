using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4Sorting
{
    public class MergeSort
    {
        public static List<string> Merge(List<string> arr, int l, int m, int r)
        {
            int i, j, k;
            int n1 = m - l + 1;
            int n2 = r - m;

            List<string> L = new List<string>(n1);
            List<string> R = new List<string>(n2);

            for (i = 0; i < n1; i++)
                L.Add(arr[l + i]);

            for (j = 0; j < n2; j++)
                R.Add(arr[m + 1 + j]);

            i = 0; 
            j = 0;
            k = l; 
            while (i < n1 && j < n2)
            {
                if (String.Compare(L[i], R[j]) <= 0)
                {
                    arr[k] = L[i];
                    i++;
                }
                else
                {
                    arr[k] = R[j];
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }

            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }

            return arr;
        }

        public static List<string> MergeSorting(List<string> arr, int l, int r)
        {
            if (l < r)
            {
                int m = l + (r - l) / 2;

                MergeSorting(arr, l, m);
                MergeSorting(arr, m + 1, r);

                Merge(arr, l, m, r);
            }

            return arr;
        }
    }
}
