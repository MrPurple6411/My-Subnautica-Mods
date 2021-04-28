using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TechPistol.Module
{
    class SpawnerFix : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private ResourceTracker tracker;
        private void Start()
        {
            gameObject.transform.localPosition += Vector3.up * 2;
            rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            tracker = gameObject.GetComponent<ResourceTracker>();
        }

        private void Update()
        {
            rigidbody.isKinematic = Vector3.Distance(Player.main.transform.position, gameObject.transform.position) > 30;
            if (!rigidbody.isKinematic)
                tracker.UpdatePosition();
        }
    }
}
