using System;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI.Templates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.Scripts.UI
{
    public class GameOver : MonoBehaviour
    {
        private SceneManager sceneManager;
        private Label esc;
        private bool onExit = false;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            esc = root.Q<Label>("esc");
        }

        [Inject]
        public void Init(MainSettings mainSettings, SceneManager sceneManager, AudioService audioService)
        {
            this.sceneManager = sceneManager;
        }

        private async void Update()
        {
            if (Keyboard.current.escapeKey.isPressed && !onExit)
            {
                onExit = true;
                await sceneManager.LoadSceneAsync(SceneName.GameMenu);
            }
        }

        private void FixedUpdate()
        {
            float timeShift = (Time.frameCount / 300f) - (Time.frameCount / 300);
            esc.style.color = new StyleColor(Color.Lerp(Color.black, Color.white, timeShift));

            Debug.Log(timeShift);
        }
    }
}
