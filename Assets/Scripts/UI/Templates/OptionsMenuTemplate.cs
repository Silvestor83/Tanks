using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI.Templates
{
    public class OptionsMenuTempalte
    {
        private MainSettings mainSettings;
        private AudioService audioService;

        private SliderInt masterSlider;
        private SliderInt musicSlider;
        private SliderInt effectsSlider;
        private TextElement textMasterVolume;
        private TextElement textMusicVolume;
        private TextElement textEffectsVolume;

        public OptionsMenuTempalte(MainSettings mainSettings, AudioService audioService)
        {
            this.mainSettings = mainSettings;
            this.audioService = audioService;
        }

        public void Init(VisualElement root)
        {
            masterSlider = root.Q<SliderInt>("masterVolume");
            musicSlider = root.Q<SliderInt>("musicVolume");
            effectsSlider = root.Q<SliderInt>("effectsVolume");
            masterSlider.RegisterCallback<ChangeEvent<int>>(MasterSliderChanged);
            musicSlider.RegisterCallback<ChangeEvent<int>>(MusicSliderChanged);
            effectsSlider.RegisterCallback<ChangeEvent<int>>(EffectsSliderChanged);
            textMasterVolume = root.Q<TextElement>("textMasterVolume");
            textMusicVolume = root.Q<TextElement>("textMusicVolume");
            textEffectsVolume = root.Q<TextElement>("textEffectsVolume");

            SetOptions();
        }

        private void SetOptions()
        {
            masterSlider.value = mainSettings.MasterVolume;
            textMasterVolume.text = masterSlider.value.ToString();

            musicSlider.value = mainSettings.MusicVolume;
            textMusicVolume.text = musicSlider.value.ToString();

            effectsSlider.value = mainSettings.EffectsVolume;
            textEffectsVolume.text = effectsSlider.value.ToString();
        }

        private void MasterSliderChanged(ChangeEvent<int> evt)
        {
            textMasterVolume.text = evt.newValue.ToString();

            audioService.ChangeMasterVolume(evt.newValue);
        }

        private void MusicSliderChanged(ChangeEvent<int> evt)
        {
            textMusicVolume.text = evt.newValue.ToString();

            audioService.ChangeMusicVolume(evt.newValue);
        }

        private void EffectsSliderChanged(ChangeEvent<int> evt)
        {
            textEffectsVolume.text = evt.newValue.ToString();

            audioService.ChangeEffectsVolume(evt.newValue);

            audioService.PlaySound(AudioSoundName.ButtonClick3);
        }
    }
}
