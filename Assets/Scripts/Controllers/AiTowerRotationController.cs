using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class AiTowerRotationController : AiRotationControllerBase
    {
        private readonly float directionChangeRate = 5f;
        private float lastTimeChangeDirection = 0;
        private GameObjectTag currentAim = GameObjectTag.Untagged;
        private int layerMask;

        void Start()
        {
            rotationSpeed = tower.RotationSpeed;
            parent = transform.parent;
            layerMask = LayerMask.GetMask(GameObjectLayer.Obstacles.ToString());
        }

        void Update()
        {
            distanceToAkvila = Vector2.Distance(transform.position, levelData.AkvilaPosition);
            distanceToPlayer = Vector2.Distance(transform.position, playerData.position);

            if (distanceToAkvila <= levelData.EnemyAimingDistance)
            {
                if (distanceToPlayer <= levelData.EnemyAimingDistance)
                {
                    if (Time.time - lastTimeChangeDirection > directionChangeRate)
                    {
                        lastTimeChangeDirection = Time.time;

                        ChooseRandomAim();
                    }

                    CheckHit();
                }
                else
                {
                    currentAim = GameObjectTag.Akvila;
                    CheckHit();
                }
            }
            else if (distanceToPlayer <= levelData.EnemyAimingDistance)
            {
                currentAim = GameObjectTag.Player;
                CheckHit();
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

        private void ChooseRandomAim()
        {
            int randomAim = Random.Range(1, 3);

            if (randomAim == 1)
            {
                currentAim = GameObjectTag.Player;
            }
            else if (randomAim == 2)
            {
                currentAim = GameObjectTag.Akvila;
            }
        }

        private void CheckHit()
        {
            Vector2 direction;

            if (currentAim == GameObjectTag.Player)
            {
                direction = playerData.position - (Vector2)transform.position;
            }
            else
            {
                direction = levelData.AkvilaPosition - (Vector2)transform.position;
            }

            var distance = currentAim == GameObjectTag.Player ? distanceToPlayer : distanceToAkvila;
            var hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);

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
    }
}
