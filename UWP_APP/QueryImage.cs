using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using NumSharp;

namespace UWP_APP
{
    internal class QueryImage
    {
        // Compare 2 bit strings, find how many bits are the same between them
        //public int HammingDistance(String a, String b)
        //{
        //    var length = a.Length;
        //    if (length != b.Length)
        //        return -1;

        //    List<char> chars1 = a.ToList();
        //    List<char> chars2 = b.ToList();

        //    var d = 0;
        //    foreach ((char c1, char c2) in chars1.Zip(chars2, (a, b) => (a, b)))
        //    {
        //        if (c1 != c2) d++;
        //    }
        //    return 1 - (d / length);
        //}

        // Compare 2 bit strings, find how many bits are the same between them
        public static int HammingDistance(String a, String b) {
            var length = a.Length;
            if (length != b.Length)
                return -1;

            var counter = 0;
            var distance = 0;
            while (counter < length) {
                if (a[counter] == b[counter])
                    distance++;
                counter++;
            }

            return distance;
        }

        private static int[] Project_0DEG(byte[][] data)
        {
            int[] result = { };
            foreach (var row in Enumerable.Range(0, 28)) {
                int rowSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28)) 
                    rowSumData += data[row][cell];

                result.Append(rowSumData);
            }

            return result;
        }

        private static int[] Project_45DEG(byte[][] data)
        {
            int[] result = { };
            foreach (var col in Enumerable.Range(0, 28))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28))
                    colSumData += data[cell][col];

                result.Append(colSumData);
            }

            return result;
        }

        private static int[] Project_90DEG(byte[][] data)
        {
            int[] result = { };
            foreach (var col in Enumerable.Range(0, 28))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28))
                    colSumData += data[cell][col];

                result.Append(colSumData);
            }

            return result;
        }

        public static byte[] Generate_Barcode(byte[] raw, int width, int height)
        {
            byte[][] formatted_data = new byte[height][];
            for (int i = 0; i < raw.Length; i+=width*4) //for each row
            {
                byte[] temp = {};
                for (int j = i; j < i + width*4; j+=4) //for each column in current row
                {
                    byte new_pixel = (byte)(raw[j] * 0.11 + raw[j + 1] * 0.59 + raw[j + 2] * 0.3); //make pixel grayscale
                    temp.Append(new_pixel); //add pixel to row
                }
                formatted_data[i] = temp; //add row
            }

            //getproj0
            //getproj45
            //getproj90
            //getproj135


            return null;
        }

        QueryImage() {



//def project_45deg(data = []):
//    diags = [data[::1,:].diagonal(i) for i in range(-data.shape[0]+1,data.shape[1])]
//    counter = 0
//    while counter < len(diags):
//        diags[counter] = list(diags[counter])
//        counter += 1
//    counter = 0
//    while counter < len(diags):
//        if len(diags[counter]) == 1:
//            diags.pop(counter)
//        counter += 1
//    return diags
//def project_135deg(data = []):
//    diags = [data[::-1,:].diagonal(i) for i in range(-data.shape[0]+1,data.shape[1])]
//    counter = 0
//    while counter < len(diags):
//        diags[counter] = list(diags[counter])
//        counter += 1
//    counter = 0
//    while counter < len(diags):
//        if len(diags[counter]) == 1:
//            diags.pop(counter)
//        counter += 1
//    return diags

        }
    }
}
