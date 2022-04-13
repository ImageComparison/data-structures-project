using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumpyDotNet;

namespace ImageComparison
{
    internal class QueryImage
    {

        public QueryImage() { }

        /**
         * Converts a 2D list into a 2D Multidimentional Array (not a Jagged Array, yes there is a difference)
         * > Note, not used. We switched to multidimentional arrays be defaults, so no more 2D Lists
         * 
         * @deprecated
         */
        private static T[,] ListToArray2D<T>(List<List<T>> input) 
        {
            var width = input[0].Count();
            var height = input.Count();
            T[,] arrays = new T[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    arrays[i, j] = input[i][j];

            return arrays;
        }
        /** 
         * Compare 2 bit strings, find how many bits are the same between them. 
         * The bit distances are range a between 0 and 1, they represent a percentage of the distance.
         * 
         * `-1`, represents an error in HammingDistance 
         */
        public static float HammingDistance(String a, String b) { // O(n)
            var length = a.Length;
            if (length != b.Length)
                return -1;

            var distance = 0;
            for (int i = 0; i < length; i++) { // O(n)
                if (a[i] == b[i])
                    distance++;
            }

            return distance / (float)length;
        }

        /**
         * Create a horizontal projection of 2D Lists
         */
        private static List<int> Project_0DEG(byte[,] data) // O(n^2)
        {
            var size = data.Length;
            var result = new List<int>();
            foreach (var row in Enumerable.Range(0, size)) { // O(n)
                int rowSumData = 0;
                foreach (var cell in Enumerable.Range(0, size)) // O(n)
                    rowSumData += data[row, cell];

                result.Add(rowSumData);
            }

            return result;
        }

        /**
         * Create a 45deg diagonal projection of 2D Lists
         */
        private static List<int> Project_45DEG(byte[,] arrays) // O(n^2)
        {
            var diags = new List<int>();

            var data = np.array(arrays);

            int start = (int)(-data.shape[0] + 1);
            int end = (int)(data.shape[1]);

            int count = (int)(data.shape[0] - 1) + end;

            // Creates a range that starts at `start` but goes for `count` iterations
            // The range is more like `[start...end] = [start...(start + count)]`
            foreach (var i in Enumerable.Range(start, count))
            {
                var list = data
                    .diagonal(i)
                    .ToList<int>();

                var sum = list.Sum();
                diags.Add(sum);
            }

            return diags;
        }

        /**
         * Create a vertical projection of 2D Lists
         */
        private static List<int> Project_90DEG(byte[,] data) // O(n^2)
        {
            var size = data.Length;
            var result = new List<int>();
            foreach (var col in Enumerable.Range(0, size))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, size))
                    colSumData += data[cell,col];

                result.Add(colSumData);
            }

            return result;
        }

        /**
         * Create a 135deg diagonal projection of 2D Lists
         */
        private static List<int> Project_135DEG(byte[,] arrays) // O(n^2)
        {
            var diags = new List<int>();

            var data = np.array(arrays);

            int start = (int)(-data.shape[0] + 1);
            int end = (int)(data.shape[1]);

            int count = (int)(data.shape[0] - 1) + end;

            // Creates a range that starts at `start` but goes for `count` iterations
            // The range is more like `[start...end] = [start...(start + count)]`
            foreach (var i in Enumerable.Range(start, count))
            {
                // Flip the array left to right, so as to get the opposite diagonal
                var list = np.fliplr(data)
                    .diagonal(i)
                    .ToList<int>();

                var sum = list.Sum();
                diags.Add(sum);
            }

            return diags;
        }

        /**
         * Generates the barcode for an image if given its data as a list of bytes, and a width & height
         */
        public static List<int> Generate_Barcode(List<byte> raw, int width, int height) // O(n^2)
        {
            byte[,] data = { };

            // For each row
            for (int i = 0; i < raw.Count; i += width * 4) // O(n)
            {
                // For each column in current row 
                for (int j = i; j < i + width * 4; j += 4) // O(n)
                {
                    // Make pixel grayscale
                    byte pixel = (byte)(raw[j] * 0.11 + raw[j + 1] * 0.59 + raw[j + 2] * 0.3);
                    data[i, j] = pixel; // Add pixel
                }
            }

            // Different projection angles
            var barcode0 = Project_0DEG(data);
            var barcode1 = Project_45DEG(data);
            var barcode2 = Project_90DEG(data);
            var barcode3 = Project_135DEG(data);

            return barcode0
                .Concat(barcode1)
                .Concat(barcode2)
                .Concat(barcode3)
                .ToList();
        }
    }
}
