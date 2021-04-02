using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Controllers
{
    public class EngineSoundController : MonoBehaviour
    {
        private UnityEvent<Track, float, float> stateChanged;
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            stateChanged = gameObject.GetComponentInParent<MoveController>()?.StateChanged ?? gameObject
                .GetComponentInParent<AiMoveController>()?.StateChanged;
        }

        private void OnEnable()
        {
            stateChanged.AddListener(OnStateChanged);
        }

        private void OnDisable()
        {
            stateChanged.RemoveAllListeners();
        }

        private void OnStateChanged(Track track, float currentSpeed, float rotationSpeed)
        {
            // pitch changes from 1.0 to 1.7 (from Idle to MaxSpeed)  
            var pitchFromSpeed = (0.7f * (currentSpeed / track.MaxSpeed)) + 1;

            // Change for the pitch when rotate 
            var pitchFromRotation = 1.25f;

            if (pitchFromSpeed > pitchFromRotation || rotationSpeed == 0)
            {
                audioSource.pitch = pitchFromSpeed;
            }
            else
            {
                audioSource.pitch = pitchFromRotation;
            }
        }
    }
}
