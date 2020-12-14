using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        private VisualElement root;

        public void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            var startButton = root.Q<Button>("start");
            var optionsButton = root.Q<Button>("options");
            var exitButton = root.Q<Button>("exit");

            startButton.text = "START";

            //root.Q<Button>("exit").RegisterCallback<MouseDownEvent>();



            startButton.clicked += StartButtonPressed;
            optionsButton.clicked += StartButtonPressed;
            exitButton.clicked += ExitButtonPressed;
        }

        private void StartButtonPressed()
        {
            SceneManager.LoadSceneAsync("Level 1");
        }

        private void ExitButtonPressed()
        {
            Application.Quit();
        }
    }
}
