using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AiPlatformRotationController : AiRotationControllerBase
    {
        private int layerMask;

        void Start()
        {
            rotationSpeed = tower.RotationSpeed;
            parent = transform.parent;
            layerMask = LayerMask.GetMask(GameObjectLayer.Obstacles.ToString());
        }

        void Update()
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerData.position);
            
            if (distanceToPlayer < levelData.CannonAimingDistance)
            {
                var direction = playerData.position - (Vector2)transform.position;
                var hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, layerMask);
                
                if (hit != true)
                {
                    currentAngle = -Vector2.SignedAngle(direction, transform.up);

                    if (currentAngle != 0)
                    {
                        var maxPossibleAngle = rotationSpeed * Time.deltaTime * Mathf.Sign(currentAngle);

                        transform.Rotate(Vector3.forward, Mathf.Abs(currentAngle) > Mathf.Abs(maxPossibleAngle) ? maxPossibleAngle : currentAngle);
                    }
                    else
                    {
                        InLineOfSight = true;

                        return;
                    }
                }
            }

            InLineOfSight = false;
        }
    }
}
