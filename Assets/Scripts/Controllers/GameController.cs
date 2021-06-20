using System;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        private bool onPause = false;
        private PlayerInput[] playerInputs;
        private SceneManager sceneManager;

        [Inject]
        public void Init(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        private void Awake()
        {
            UpdatePlayersInput();
        }

        public void UpdatePlayersInput()
        {
            playerInputs = GetComponentsInChildren<PlayerInput>();
        }
    
        private async void FixedUpdate()
        {
            await OtherActions();
        }

        private async UniTask OtherActions()
        {
            if (onPause && Time.timeScale != 0)
            {
                onPause = false;
                UpdatePlayersInput();

                foreach (var playerInput in playerInputs)
                {
                    playerInput.ActivateInput();
                }
            }

            if (Keyboard.current.escapeKey.isPressed && !onPause)
            {
                onPause = true;
                Time.timeScale = 0;
                UpdatePlayersInput();

                foreach (var playerInput in playerInputs)
                {
                    playerInput.DeactivateInput();
                }

                await sceneManager.LoadSceneAdditiveAsync(SceneName.LevelMenu);
            }
        }
    }
}
