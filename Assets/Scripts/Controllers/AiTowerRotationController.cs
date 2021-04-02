using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AiTowerRotationController : AiRotationControllerBase
    {
        void Start()
        {
            rotationSpeed = tower.RotationSpeed;
            parent = transform.parent;
        }

        void Update()
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerData.position);

            if (distanceToPlayer < levelData.EnemyAimingDistance)
            {
                var direction = playerData.position - (Vector2)transform.position;
                var layerMask = LayerMask.GetMask(GameObjectLayer.Obstacles.ToString());
                var hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, layerMask);
                
                if (hit != true)
                {
                    currentAngle = -Vector2.SignedAngle(direction, transform.up);

                    if (currentAngle == 0)
                    {
                        InLineOfSight = true;
                    }
                }
                else
                {
                    currentAngle = -Vector2.SignedAngle(parent.transform.up, transform.up);
                    InLineOfSight = false;
                }
            }
            else
            {
                currentAngle = -Vector2.SignedAngle(parent.transform.up, transform.up);
                InLineOfSight = false;
            }

            if (currentAngle != 0)
            {
                var maxPossibleAngle = rotationSpeed * Time.deltaTime * Mathf.Sign(currentAngle);

                transform.Rotate(Vector3.forward, Mathf.Abs(currentAngle) > Mathf.Abs(maxPossibleAngle) ? maxPossibleAngle : currentAngle);
            }
        }
    }
}
