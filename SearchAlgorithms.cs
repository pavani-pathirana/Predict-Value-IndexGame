using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictValueIndexGame
{
    public class SearchAlgorithms
    {
        private static long GetElapsedMicroseconds(Stopwatch stopwatch)
        {
            return stopwatch.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;

        }

        public static (int index, long time) BinarySearch(List<int> list, int value)
        {
            var stopwatch = Stopwatch.StartNew();
            int low = 0, high = list.Count - 1;
            int result = -1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (list[mid] == value)
                {
                    result = mid;
                    break;
                }
                if (list[mid] < value)
                    low = mid + 1;
                else
                    high = mid - 1;
            }

            stopwatch.Stop();
            return (result, GetElapsedMicroseconds(stopwatch));
        }

        public static (int index, long time) JumpSearch(List<int> list, int value)
        {
            var stopwatch = Stopwatch.StartNew();
            int length = list.Count;
            int step = (int)Math.Sqrt(length);
            int prev = 0;

            while (list[Math.Min(step, length) - 1] < value)
            {
                prev = step;
                step += (int)Math.Sqrt(length);
                if (prev >= length)
                    return (-1, GetElapsedMicroseconds(stopwatch));
            }

            while (list[prev] < value)
            {
                prev++;
                if (prev == Math.Min(step, length))
                    return (-1, GetElapsedMicroseconds(stopwatch));
            }

            if (list[prev] == value)
            {
                stopwatch.Stop();
                return (prev, GetElapsedMicroseconds(stopwatch));
            }

            stopwatch.Stop();
            return (-1, GetElapsedMicroseconds(stopwatch));
        }

       
        public static (int index, long time) ExponentialSearch(List<int> list, int value)
        {
            var stopwatch = Stopwatch.StartNew();
            int length = list.Count;
            if (list[0] == value) return (0, GetElapsedMicroseconds(stopwatch));

            int index = 1;
            while (index < length && list[index] <= value)
                index *= 2;

            int low = index / 2;
            // int high = Math.min(index, length - 1);
            int high = Math.Min(index, length - 1);


            (int subIndex, long binaryTime) = BinarySearch(list.GetRange(low, high - low + 1), value);
            stopwatch.Stop();

            if (subIndex != -1)
                return (subIndex + low, GetElapsedMicroseconds(stopwatch));  // Corrected index
            else
                return (-1, GetElapsedMicroseconds(stopwatch));  // Not found
        }

        public static (int index, long time) FibonacciSearch(List<int> list, int value)
        {
           
            var stopwatch = Stopwatch.StartNew();
            int length = list.Count;
            int fibM2 = 0;
            int fibM1 = 1;
            int fibM = fibM2 + fibM1;

            while (fibM < length)
            {
                fibM2 = fibM1;
                fibM1 = fibM;
                fibM = fibM2 + fibM1;
            }

            int offset = -1;

            while (fibM > 1)
            {
                int i = Math.Min(offset + fibM2, length - 1);

                if (list[i] < value)
                {
                    fibM = fibM1;
                    fibM1 = fibM2;
                    fibM2 = fibM - fibM1;
                    offset = i;
                }
                else if (list[i] > value)
                {
                    fibM = fibM2;
                    fibM1 = fibM1 - fibM2;
                    fibM2 = fibM - fibM1;
                }
                else
                {
                    stopwatch.Stop();
                    return (i, GetElapsedMicroseconds(stopwatch));
                }
            }

            if (fibM1 == 1 && list[offset + 1] == value)
            {
                stopwatch.Stop();
                return (offset + 1, GetElapsedMicroseconds(stopwatch));
            }

            stopwatch.Stop();
            return (-1, GetElapsedMicroseconds(stopwatch));
        }

        public static (int index, long time) InterpolationSearch(List<int> list, int value)
        {
            var stopwatch = Stopwatch.StartNew();
            int low = 0, high = list.Count - 1;

            while (low <= high && value >= list[low] && value <= list[high])
            {
                if (low == high)
                {
                    if (list[low] == value)
                        return (low, GetElapsedMicroseconds(stopwatch));
                    return (-1, GetElapsedMicroseconds(stopwatch));
                }

                int pos = low + (((high - low) / (list[high] - list[low])) * (value - list[low]));

                if (list[pos] == value)
                {
                    stopwatch.Stop();
                    return (pos, GetElapsedMicroseconds(stopwatch));
                }

                if (list[pos] < value)
                    low = pos + 1;
                else
                    high = pos - 1;
            }

            stopwatch.Stop();
            return (-1, GetElapsedMicroseconds(stopwatch));
        }
    }
}
