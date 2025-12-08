using System.IO;
using UnityEngine;

namespace Utils.Paths
{
    public static class Paths
    {
        public static readonly string PathToSaveData = Path.Combine(Application.persistentDataPath, "save.data");
        public static readonly string PathToSaveDirectory = Path.Combine(Application.persistentDataPath, "saves");
        
        public static string GetPathToProgressData()
        {
            return Path.Combine(PathToSaveDirectory, "progress.data");
        }
        
        public static string GetPathToLevelDataDirectory(string levelName)
        {
            return Path.Combine(PathToSaveDirectory, levelName);
        }
        
        public static string GetPathToLevelData(string levelName)
        {
            return Path.Combine(PathToSaveDirectory, levelName, "save.data");
        }
        
    }
}