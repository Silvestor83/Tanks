using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Services;
using Zenject;

namespace Assets.Scripts.Core
{
    class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TankCreator>().AsSingle();
            Container.Bind<CannonCreator>().AsSingle();
            Container.Bind<MechanicalPartsBuilder>().AsSingle();
            Container.Bind(typeof(ProjectileCreator), typeof(IDisposable)).To<ProjectileCreator>().AsSingle();
            Container.Bind(typeof(ExplosionCreator), typeof(IDisposable)).To<ExplosionCreator>().AsSingle();
        }
    }
}
