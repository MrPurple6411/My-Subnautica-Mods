using System;
using UnityEngine;

namespace BuilderModule
{
    public static class UnityHelper
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        public static Component GetOrAddComponent(this GameObject gameObject, Component component)
        {
            Type componentType = component.GetType();
            return gameObject.GetComponent(componentType) ?? gameObject.AddComponent(componentType);
        }

        public static void AddIfNeedComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.GetComponent<T>().IsNull())
            {
                gameObject.AddComponent<T>();                
            }            
        }

        public static void AddIfNeedComponent(this GameObject gameObject, Type component)
        {
            if (gameObject.GetComponent(component).IsNull())
            {
                gameObject.AddComponent(component);                
            }            
        }        

        public static bool IsRoot(this Transform transform)
        {
            return transform.parent == null ? true : false;
        }

        public static bool IsRoot(this GameObject gameObject)
        {
            return gameObject.transform.parent == null ? true : false;
        }        

        public static void CleanObject(this GameObject gameObject)
        {
            foreach (Component component in gameObject.GetComponents<Component>())
            {
                Type componentType = component.GetType();

                if (componentType == typeof(Transform))
                    continue;
                if (componentType == typeof(Renderer))
                    continue;
                if (componentType == typeof(Mesh))
                    continue;
                if (componentType == typeof(Shader))
                    continue;

                UnityEngine.Object.Destroy(component);
            }
        }
        
        public static bool IsNotNull(this UnityEngine.Object ueObject)
        {
            return ueObject == null ? false : true;
        }        

        public static bool IsNull(this UnityEngine.Object ueObject)
        {
            return ueObject == null ? true : false;
        }
    }
}

