using System;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class AudioThemeController : MonoBehaviour
    {
        [SerializeField]
        private AudioTheme AudioTheme;

        private AudioSource audioSource;
        private AudioManager audioManager;

        [Inject]
        public void Inject(AudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        private void Awake()
        {
            if (audioManager.AudioSource == null)
            {
                audioManager.AudioSource = GetComponent<AudioSource>();
            }
        }

        async void Start()
        {
            // This for getting rid of audio spikes
            await UniTask.Delay(TimeSpan.FromSeconds(0.3d));

            audioManager.PlayAudioTheme(AudioTheme);
        }
    }
}
