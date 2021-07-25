using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Controllers
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private UnityEvent<Track, float, float> stateChanged;
        
        void Awake()
        {
            animator = gameObject.GetComponent<Animator>();
            stateChanged = gameObject.GetComponentInParent<MoveController>()?.StateChanged ?? gameObject
                .GetComponentInParent<AiMoveController>()?.StateChanged;
        }

        private void Start()
        {
            animator.speed = 0;
        }

        private void OnEnable()
        {
            stateChanged.AddListener(OnStateChanged);
        }

        private void OnDisable()
        {
            stateChanged.RemoveAllListeners();
        }

        private void OnStateChanged(Track track, float speed, float rotationSpeed)
        {
            // Animation speed determination. Where 10 is the animation sample rate from the animation window
            var animationSpeedDependOnTankSpeed = speed / (10 * track.AnimationStep);

            // Take into account that the coefficient 0.00125f is obtained by dividing track.AnimationStep = 0.15 on rotationSpeed = 120.
            // This parameters give us animationSpeedDependOnRotation = 1f.
            var animationSpeedDependOnRotation = 0.00125f * (rotationSpeed / track.AnimationStep);

            if (animationSpeedDependOnTankSpeed > animationSpeedDependOnRotation || rotationSpeed == 0)
            {
                animator.speed = animationSpeedDependOnTankSpeed;
            }
            else
            {
                animator.speed = animationSpeedDependOnRotation;
            }
        }
    }
}
