using Assets.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tank
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private UnityEvent<float, float> stateChanged;

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

        private void OnStateChanged(float speed, float rotationSpeed)
        {
            var animationSpeedDependOnTankSpeed = speed * 6.25f;
            var animationSpeedDependOnRotation = 10f;

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
