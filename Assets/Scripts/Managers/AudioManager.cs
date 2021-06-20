using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZLogger;

namespace Assets.Scripts.Managers
{
    public class AudioManager
    {
        private readonly LogService logService;
        public AudioSource AudioSource;
        private AudioTheme audioTheme;
        private AsyncOperationHandle<AudioClip> handler;
        private AsyncOperationHandle<AudioClip> previousHandler;

        public AudioManager(LogService logService)
        {
            this.logService = logService;
        }

        public void PlayAudioTheme(AudioTheme theme)
        {
            if (theme != audioTheme)
            {
                previousHandler = handler;
                AudioSource.Stop();

                if (theme != AudioTheme.None)
                {
                    handler = Addressables.LoadAssetAsync<AudioClip>(theme.GetLongString());
                    handler.Completed += HandleOnCompleted;
                    audioTheme = theme;
                }
            }
            else if (theme != AudioTheme.None)
            {
                AudioSource.Play();
            }
        }

        private void HandleOnCompleted(AsyncOperationHandle<AudioClip> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                AudioSource.clip = obj.Result;

                AudioSource.Play();


                if (previousHandler.IsValid())
                {
                    Addressables.Release(previousHandler);
                }
                
            }
            else
            {
                logService.Loggger.ZLogError($"Can't Load AudioClip {audioTheme.ToString()}");
            }
        }
    }
}
