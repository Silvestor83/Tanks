using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI.Templates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class LevelMenu : MonoBehaviour
{
    private VisualElement root;
    private LocalizationService locService;
    private SceneManager sceneManager;
    private MainSettings mainSettings;
    private AudioService audioService;
    private OptionsMenuTempalte optionsMenuTemplate;

    private VisualElement levelMenu;
    private VisualElement optionsMenu;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        levelMenu = root.Q("levelMenu");
        optionsMenu = root.Q("optionsMenu");
        
        root.Q<Button>("options").clicked += OptionsButtonPressed;
        root.Q<Button>("resume").clicked += ResumeButtonPressed;
        root.Q<Button>("menu").clicked += MenuButtonPressed;

        root.Q<Button>("back").clicked += BackButtonPressed;

        optionsMenuTemplate.Init(optionsMenu);

        //ToDo startButton.text = locService.GetString(LocalizationTables.UI, "");
    }

    [Inject]
    public void Init(MainSettings mainSettings, SceneManager sceneManager, AudioService audioService)
    {
        this.mainSettings = mainSettings;
        this.sceneManager = sceneManager;
        this.audioService = audioService;
        optionsMenuTemplate = new OptionsMenuTempalte(mainSettings, audioService);
    }

    private void OptionsButtonPressed()
    {
        audioService.PlaySound(AudioSoundName.ButtonClick3);

        levelMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    private void ResumeButtonPressed()
    {
        audioService.PlaySound(AudioSoundName.ButtonClick3);
        _ = sceneManager.UnloadSceneAdditiveAsync(SceneName.LevelMenu);

        Time.timeScale = 1.0f;
    }

    private void MenuButtonPressed()
    {
        audioService.PlaySound(AudioSoundName.ButtonClick3);
        _ = sceneManager.LoadSceneAsync(SceneName.GameMenu);

        Time.timeScale = 1.0f;
    }

    // Options menu

    private void BackButtonPressed()
    {
        audioService.PlaySound(AudioSoundName.ButtonClick3);

        levelMenu.style.display = DisplayStyle.Flex;
        optionsMenu.style.display = DisplayStyle.None;
    }
}
