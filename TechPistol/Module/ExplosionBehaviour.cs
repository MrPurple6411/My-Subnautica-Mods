namespace TechPistol.Module
{
    using System.Linq;
    using UnityEngine;

    internal class ExplosionBehaviour: MonoBehaviour
    {
        protected void OnParticleCollision(GameObject target)
        {
#if !EDITOR
            var num = UWE.Utils.OverlapSphereIntoSharedBuffer(target.transform.position, Main.Config.CannonExplosionSize);
            for(var i = 0; i < num; i++)
            {
                var go = UWE.Utils.sharedColliderBuffer[i].gameObject;
                var entityRoot = UWE.Utils.GetEntityRoot(go) ?? go;

                if(entityRoot.TryGetComponent<LiveMixin>(out var liveMixin))
                {
                    liveMixin.TakeDamage(PistolBehaviour.lastShotDamage, entityRoot.transform.position, DamageType.Explosive);
                }

                if(entityRoot.TryGetComponent<BreakableResource>(out var breakableResource))
                {
                    while(breakableResource.hitsToBreak > 0)
                        breakableResource.HitResource();
                }

                if (!entityRoot.TryGetComponent<Drillable>(out var drillable)) continue;
                while(drillable.health.Sum() > 0)
                    drillable.OnDrill(entityRoot.transform.position, null, out var _);
            }
            
#endif
        }
    }
}
