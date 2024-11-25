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

            /* create temp lists */
            List<string> L = new List<string>(n1);
            List<string> R = new List<string>(n2);

            /* Copy data to temp lists L[] and R[] */
            for (i = 0; i < n1; i++)
                L.Add(arr[l + i]);

            for (j = 0; j < n2; j++)
                R.Add(arr[m + 1 + j]);

            /* Merge the temp lists back into arr[l..r]*/
            i = 0; // Initial index of first sublist
            j = 0; // Initial index of second sublist
            k = l; // Initial index of merged sublist
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

            /* Copy the remaining elements of L[], if there
            are any */
            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }

            /* Copy the remaining elements of R[], if there
            are any */
            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }

            return arr;
        }

        /* l is for left index and r is right index of the
        sub-list of arr to be sorted */
        public static List<string> MergeSorting(List<string> arr, int l, int r)
        {
            if (l < r)
            {
                // Same as (l+r)/2, but avoids overflow for
                // large l and h
                int m = l + (r - l) / 2;

                // Sort first and second halves
                MergeSorting(arr, l, m);
                MergeSorting(arr, m + 1, r);

                Merge(arr, l, m, r);
            }

            return arr;
        }
    }
}
