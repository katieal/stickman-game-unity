using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Source:  https://github.com/UnityTechnologies/UniteNow20-Persistent-Data/blob/main/FileManager.cs
 */

namespace Managers
{
    public static class FileManager 
    {
        public static bool CheckForFile(string fileName)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            return File.Exists(fullPath);
        }

        public static bool WriteToFile(string a_FileName, string a_FileContents)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

            try
            {
                // this is best way to write small files as opposed to using filestream
                File.WriteAllText(fullPath, a_FileContents);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        public static bool LoadFromFile(string a_FileName, out string result)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

            try
            {
                result = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                result = "";
                return false;
            }
        }
    }
}