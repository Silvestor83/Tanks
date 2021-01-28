using System;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Zenject;

namespace Assets.Scripts.Core
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LogService>().AsSingle();
            Container.Bind<SceneService>().AsSingle();
            Container.Bind<SceneManager>().AsSingle();
        }

        private void Awake()
        {

        }

        public override void Start()
        {
            
        }
    }
}