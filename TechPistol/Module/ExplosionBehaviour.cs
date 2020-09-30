using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;

namespace TechPistol.Module
{
	class ExplosionBehaviour : MonoBehaviour
	{
		private void OnParticleCollision(GameObject target)
		{
			int num = UWE.Utils.OverlapSphereIntoSharedBuffer(target.transform.position, Main.config.CannonExplosionSize, -1, 0);
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = UWE.Utils.sharedColliderBuffer[i].gameObject;
				GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject) ?? gameObject;
				
				if(entityRoot.TryGetComponent<LiveMixin>(out LiveMixin liveMixin ))
				{
					liveMixin.TakeDamage(PistolBehaviour.lastShotDamage, entityRoot.transform.position, DamageType.Explosive, null);
				}

				if (entityRoot.TryGetComponent<BreakableResource>(out BreakableResource breakableResource))
				{
					while (breakableResource.hitsToBreak > 0)
						breakableResource.HitResource();
				}

				if (entityRoot.TryGetComponent<Drillable>(out Drillable drillable))
				{
					while (drillable.health.Sum() > 0)
						drillable?.OnDrill(entityRoot.transform.position, null, out var _);
				}
			}
		}
	}
}
