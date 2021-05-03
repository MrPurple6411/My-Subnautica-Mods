namespace TechPistol.Module
{
    using System.Linq;
    using UnityEngine;

    internal class ExplosionBehaviour: MonoBehaviour
    {
        protected void OnParticleCollision(GameObject target)
        {
            int num = UWE.Utils.OverlapSphereIntoSharedBuffer(target.transform.position, Main.Config.CannonExplosionSize, -1, 0);
            for(int i = 0; i < num; i++)
            {
                GameObject gameObject = UWE.Utils.sharedColliderBuffer[i].gameObject;
                GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject) ?? gameObject;

                if(entityRoot.TryGetComponent<LiveMixin>(out LiveMixin liveMixin))
                {
                    liveMixin.TakeDamage(PistolBehaviour.lastShotDamage, entityRoot.transform.position, DamageType.Explosive, null);
                }

                if(entityRoot.TryGetComponent<BreakableResource>(out BreakableResource breakableResource))
                {
                    while(breakableResource.hitsToBreak > 0)
                        breakableResource.HitResource();
                }

                if(entityRoot.TryGetComponent<Drillable>(out Drillable drillable))
                {
                    while(drillable.health.Sum() > 0)
                        drillable?.OnDrill(entityRoot.transform.position, null, out GameObject _);
                }
            }
        }
    }
}
