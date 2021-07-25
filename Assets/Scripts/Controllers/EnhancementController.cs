using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Analytics;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class EnhancementController : MonoBehaviour
    {
        [NonSerialized]
        public MechanicalPart MechanicalPart;

        private EnhancementType enhancementType;
        private EnhancementService enhancementService;

        private readonly float rotationSpeed = 120;
        private readonly float lifeTime = 10.0f;
        private readonly float startBlinkingTime = 4.0f;
        private readonly float startFasterBlinkingTime = 1.5f;
        private readonly float blinkingRate = 0.5f;
        private readonly float fasterBlinkingRate = 0.25f;
        private readonly float glowingRate = 1f;

        private float startLifeTime;
        private float remainingLifeTime;
        private float blinkIndicator;
        private SpriteRenderer[] sprites;
        private Collider2D[] colliders;
        private bool isHidden = false;
        private SpriteRenderer circle;
        private bool inContact = false;

        [Inject]
        public void Init(TankCreator tankCreator, EnhancementService enhancementService, EnhancementType enhancementType)
        {
            this.enhancementType = enhancementType;
            this.enhancementService = enhancementService;
        }

        private void Awake()
        {
            sprites = GetComponentsInChildren<SpriteRenderer>();
            circle = GetComponent<SpriteRenderer>();
            colliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            startLifeTime = Time.time;

            for (int i = 1; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }

        private void Update()
        {
            CreateBlinkingEffect();
            CreateGlowingEffect();
            
            if (enhancementType == EnhancementType.MechanicalPart)
            {
                Rotate();
            }
        }

        private void CreateGlowingEffect()
        {
            float timeShift;

            var glowingRelativeTime = Time.time % glowingRate;

            if (glowingRelativeTime > glowingRate / 2)
            {
                timeShift = (glowingRate - glowingRelativeTime) / (glowingRate / 2);
            }
            else
            {
                timeShift = glowingRelativeTime / (glowingRate / 2);
            }

            circle.color = Color.Lerp(Color.white, new Color(0.627451f, 0.3921569f, 0.3921569f), timeShift);
        }

        private void CreateBlinkingEffect()
        {
            remainingLifeTime = startLifeTime + lifeTime - Time.time;

            if (remainingLifeTime <= startBlinkingTime)
            {
                if (remainingLifeTime <= startFasterBlinkingTime)
                {
                    if (remainingLifeTime <= 0)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    Blink(fasterBlinkingRate, startFasterBlinkingTime);
                    return;
                }

                Blink(blinkingRate, startBlinkingTime);
            }
        }

        private void Blink(float rate, float startBlinkingTime)
        {
            blinkIndicator = ((startBlinkingTime - remainingLifeTime) % rate) - (rate / 2f);

            if (blinkIndicator < 0 && !isHidden)
            {
                isHidden = true;

                foreach (var spriteRenderer in sprites)
                {
                    spriteRenderer.enabled = false;
                }
            }
            else if (blinkIndicator >= 0 && isHidden)
            {
                isHidden = false;

                foreach (var spriteRenderer in sprites)
                {
                    spriteRenderer.enabled = true;
                }
            }
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }

        async void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(GameObjectTag.Player.ToString()) && !inContact)
            {
                inContact = true;

                switch (enhancementType)
                {
                    case EnhancementType.MechanicalPart:
                        await enhancementService.UpgradeMechanicalPart(MechanicalPart, collider.transform.root.gameObject);
                        break;
                    case EnhancementType.Health:
                        enhancementService.UpdateHealth(collider.transform.root.gameObject);
                        break;
                }
                
                Destroy(gameObject);
            }
        }
    }
}
