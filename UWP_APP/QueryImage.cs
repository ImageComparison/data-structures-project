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
        public static float HammingDistance(String a, String b) { // O(n)
            var length = a.Length;
            if (length != b.Length)
                return -1;

            var distance = 0;
            for (int i = 0; i < length; i++) { // O(n)
                if (a[i] == b[i])
                    distance++;
            }

            return distance/(float)length;
        }

        private static List<int> Project_0DEG(List<List<byte>> data) // O(n^2)
        {
            List<int> result = new List<int>();
            foreach (var row in Enumerable.Range(0, 28)) { // O(n)
                int rowSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28)) // O(n)
                    rowSumData += data[row][cell];

                result.Add(rowSumData);
            }

            return result;
        }

        private static List<int> Project_45DEG(List<List<byte>> data) // O(n^2)
        {
            List<int> result = new List<int>();
            foreach (var col in Enumerable.Range(0, 28))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28))
                    colSumData += data[cell][col];

                result.Add(colSumData);
            }

            return result;
        }

        private static List<int> Project_90DEG(List<List<byte>> data) // O(n^2)
        {
            List<int> result = new List<int>();
            foreach (var col in Enumerable.Range(0, 28))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28))
                    colSumData += data[cell][col];

                result.Add(colSumData);
            }

            return result;
        }

        private static List<int> Project_135DEG(List<List<byte>> data) // O(n^2)
        {
            // var rot_data = data
                // .SelectMany(inner => inner.Select((item, index) => new { item, index }))
                // .GroupBy(i => i.index, i => i.item)
                // .Select(g => g.ToList())
                // .ToList();
            
            // ROT_DATA IS INCORRECT, IT WILL YEILD THE SAME RESULTS AS 45 DEG
            // BECAUSE THE MATRIX IS BEING ROTATED 180 DEGREES AND NOT 90 DEGREES
            
            
            
            List<int> result = new List<int>();
            foreach (var col in Enumerable.Range(0, 28))
            {
                int colSumData = 0;
                foreach (var cell in Enumerable.Range(0, 28))
                    // colSumData += rot_data[cell][col]; // uncomment when working
                    colSumData += data[cell][col]; // delete when working


                result.Add(colSumData);
            }

            return result;
        }
        
        public static List<int> Generate_Barcode(List<byte> raw, int width, int height) // O(n^2)
        {
            List<List<byte>> formatted_data = new List<List<byte>>();
            
            for (int i = 0; i < raw.Count; i+=width*4) //for each row // O(n)
            {
                List<byte> temp = new List<byte>();
                for (int j = i; j < i + width*4; j+=4) //for each column in current row // O(n)
                {
                    byte new_pixel = (byte)(raw[j] * 0.11 + raw[j + 1] * 0.59 + raw[j + 2] * 0.3); //make pixel grayscale
                    temp.Add(new_pixel); //add pixel to row
                    // temp.Append(new_pixel); // ** replaced with line above **
                    
                    // List<int> result = new List<int>();
                    // Console.WriteLine(result.Count);
                    // result.Add(3);
                    // Console.WriteLine(result.Count);

                    //NEED TO USE VECTORS NOT ARRAYS
                }
                formatted_data.Add(temp); //add row
            }

            //getproj0
            List<int> barcode0 = Project_0DEG(formatted_data);
            //getproj45
            List<int> barcode1 = Project_45DEG(formatted_data);
            //getproj90
            List<int> barcode2 = Project_90DEG(formatted_data);
            //getproj135
            List<int> barcode3 = Project_135DEG(formatted_data);

            return (barcode0.Concat(barcode1.Concat(barcode2.Concat(barcode3).ToList()).ToList()).ToList());
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
