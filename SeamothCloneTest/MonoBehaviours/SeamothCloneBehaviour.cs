using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UWE;

namespace SeamothCloneTest.MonoBehaviours
{
    public class SeamothCloneBehaviour : MonoBehaviour
    {
        public  void Start()
        {
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                Console.WriteLine($"{renderer.gameObject.name} {renderer.GetType()} {renderer.material.mainTexture?.name ?? "null"}");//: {JsonConvert.SerializeObject(renderer.materials, new JsonSerializerSettings(){ Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MaxDepth = 1})}");
            }
        }

    }
}
