using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

//Future Access List Manipulation

namespace UWP_APP
{
    internal class FALManip
    {
        //FAL Functions
        /*
         * Save FAL when:
         * Loading new ref images ./
         * 
         * Get FAL when:
         * Getting image raw ./
         * Updating image display ./
         * 
         * Clear FAL when:
         * MainWindow initializes ./
         * 
         * Remove token from FAL when:
         * File is removed from ref list ./
         */
        public static string RememberFile(StorageFile file) //create token
        {
            var fal = StorageApplicationPermissions.FutureAccessList;
            string token = Guid.NewGuid().ToString();
            fal.AddOrReplace(token, file);
            return token;
        }
        public static async Task<StorageFile> GetFileForToken(string token) //returns file from specific token
        {
            var fal = StorageApplicationPermissions.FutureAccessList;
            if (!fal.ContainsItem(token)) return null;
            return await fal.GetFileAsync(token);
        }
        public static void ClearFAL() //removes all tokens
        {
            var fal = StorageApplicationPermissions.FutureAccessList;
            fal.Clear();
        }
        public static List<string> RemoveFileByToken(string token, List<string> tokenList) //removes specific token from FAL and tokenlist
        {
            var fal = StorageApplicationPermissions.FutureAccessList;
            fal.Remove(token);
            tokenList.Remove(token);
            return tokenList;
        }
    }
}
