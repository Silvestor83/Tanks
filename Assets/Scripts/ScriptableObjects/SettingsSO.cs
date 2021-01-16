using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsSO", menuName = "Installers/Settings")]
    public class SettingsSO : ScriptableObjectInstaller<SettingsSO>
    {
        [SerializeField]
        private MainSettings mainSettings;
        
        public bool rewriteSettingFiles;
        
        public override void InstallBindings()
        {
            UpdateSettingsFromFiles();

            Container.BindInstance(mainSettings);
        }

        private void UpdateSettingsFromFiles()
        {
            UpdateSettingsFromFile(ref mainSettings);
        }

        private void UpdateSettingsFromFile<T>(ref T settings) where T : class
        {
            var path = FileService.GetFullPath(typeof(T).Name + ".xml");
            var obj = FileService.GetObjFromFile<T>(path);

            if (obj != null)
            {
                settings = obj;
            }
        }

        public void UpdateSettingFiles()
        {
            UpdateSettingFile(mainSettings);
        }

        private void UpdateSettingFile<T>(T settings) where T : class
        {
            var path = FileService.GetFullPath(typeof(T).Name + ".xml");

            if (rewriteSettingFiles)
            {
                FileService.WriteObjToFile(settings, path);
            }
            else
            {
                // to show previous values from files in editor (don't show values in editor which we have tested)
                UpdateSettingsFromFiles();
            }
        }
    }
}