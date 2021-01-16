using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Settings;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public static class FileService
    {
        public static T GetObjFromFile<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    return Serialization.Deserialize<T>(stream);
                }
            }

            return null;
        }

        public static string GetFullPath(string fileName)
        {
            return Path.Combine(Application.dataPath, fileName);
        }

        public static void WriteObjToFile<T>(T obj, string path) where T : class
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                Serialization.Serialize(stream,obj);
            }
        }
    }
}
