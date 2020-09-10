using System;
using System.IO;
using UnityEngine;
using UWE;

namespace TechPistol.Module
{
	class ExplosionBehaviour : MonoBehaviour
	{
		private void OnParticleCollision(GameObject taget)
		{
			int num = UWE.Utils.OverlapSphereIntoSharedBuffer(taget.transform.position, Main.config.CannonExplosionDamageRange, -1, 0);
			for (int i = 0; i < num; i++)
			{
				GameObject entityRoot = UWE.Utils.GetEntityRoot(UWE.Utils.sharedColliderBuffer[i].gameObject);
				bool flag = entityRoot != null && entityRoot.GetComponent<LiveMixin>() != null;
				if (flag)
				{
					entityRoot.GetComponent<LiveMixin>().TakeDamage(Main.config.CannonDamage, entityRoot.transform.position, DamageType.Explosive, null);
				}
			}
		}
	}
}
