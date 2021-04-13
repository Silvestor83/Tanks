using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZLogger;

namespace Assets.Scripts.Services
{
    public class AudioService
    {
        public AudioSource Audio;
        public AudioMixer AudioMixer;

        private const int MIN_VOLUME = -80;
        private readonly MainSettings mainSettings;
        private readonly LogService logService;
        private Dictionary<AudioSoundName, AudioClip> audioClips;
        private AsyncOperationHandle<IList<AudioClip>> handle;

        public AudioService(MainSettings mainSettings, LogService logService)
        {
            this.mainSettings = mainSettings;
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

        public void ChangeMasterVolume(int volume)
        {
            AudioMixer.SetFloat("MasterVolume", GetAbsoluteVolume(volume));
            mainSettings.MasterVolume = volume;
        }

        public void ChangeMusicVolume(int volume)
        {
            AudioMixer.SetFloat("MusicVolume", GetAbsoluteVolume(volume));
            mainSettings.MusicVolume = volume;
        }

        public void ChangeEffectsVolume(int volume)
        {
            AudioMixer.SetFloat("EffectsVolume", GetAbsoluteVolume(volume));
            mainSettings.EffectsVolume = volume;
        }

        private int GetAbsoluteVolume(int relativeVolume)
        {
            int absoluteVolume;

            if (relativeVolume == 0)
            {
                absoluteVolume = MIN_VOLUME;
            }
            else
            {
                absoluteVolume = (int)(-MIN_VOLUME * Mathf.Log10(relativeVolume)) + MIN_VOLUME;
            }

            return absoluteVolume;
        }
    }
}
