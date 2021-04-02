using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZLogger;

namespace Assets.Scripts.Services
{
    public class AudioService
    {
        public AudioSource Audio;

        private readonly LogService logService;
        private Dictionary<AudioSoundName, AudioClip> audioClips;
        private AsyncOperationHandle<IList<AudioClip>> handle;

        public AudioService(LogService logService)
        {
            this.logService = logService;
            handle = Addressables.LoadAssetsAsync<AudioClip>("Sound", (n) => { });
            handle.Completed += HandleOnCompleted;
        }

        private void HandleOnCompleted(AsyncOperationHandle<IList<AudioClip>> obj)
        {
            var clips = obj.Result;
            audioClips = new Dictionary<AudioSoundName, AudioClip>();

            foreach (AudioSoundName soundName in Enum.GetValues(typeof(AudioSoundName)))
            {
                var clip = clips.FirstOrDefault(a => a.name == soundName.GetLongString());
                audioClips.Add(soundName, clip);
            }
        }

        public void PlaySound(AudioSoundName audioSound)
        {
            if (audioClips != null && Audio != null)
            {
                var clip = audioClips.First(a => a.Key == audioSound).Value;

                if (clip != null)
                {
                    Audio.clip = clip;
                    Audio.Play();
                }
                else
                {
                    logService.Loggger.ZLogCritical($"Sound ({audioSound.ToString()}) not found");
                }
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                logService.Loggger.ZLogError("Failed to get projectiles prefab");
            }
        }
    }
}
