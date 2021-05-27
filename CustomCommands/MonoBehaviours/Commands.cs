namespace CustomCommands.MonoBehaviours
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UWE;

    public class Commands: MonoBehaviour
    {
        private Quaternion spawnRotation;

        private Vector3 spawnPosition;

        private float x;

        private float y;

        private float z;
        
        public void Awake()
        {
            SceneManager.sceneLoaded += Command;
        }

        public void OnDestroy()
        {
            Placeholder.Awake();
            SceneManager.sceneLoaded -= Command;
        }

        public void Command(Scene off, LoadSceneMode on)
        {
            DevConsole.RegisterConsoleCommand(this, "spse");
            DevConsole.RegisterConsoleCommand(this, "pickup");
            DevConsole.RegisterConsoleCommand(this, "friend");
            DevConsole.RegisterConsoleCommand(this, "playse");
            DevConsole.RegisterConsoleCommand(this, "size");
            DevConsole.RegisterConsoleCommand(this, "suse");
        }

        private static bool GetTarget(out GameObject target)
        {
            var cameraTransform = MainCamera.camera.transform;
            var forward = cameraTransform.forward;
            Physics.Raycast(new Ray(cameraTransform.position + (forward * 0.15f), forward), out var raycastHit, 100f);
            var flag = raycastHit.collider != null;
            if(flag)
            {
                var gameObject = raycastHit.collider.gameObject;
                var prefabIdentifier = gameObject.GetComponentInParent<PrefabIdentifier>();
                if(prefabIdentifier != null)
                {
                    target = prefabIdentifier.gameObject;
                    return true;
                }
            }
            target = null;
            return false;
        }

        public void OnConsoleCommand_pickup()
        {
            if(GetTarget(out var target))
            {
                var pickupable = target.GetComponent<Pickupable>();
                if(pickupable is null)
                {
                    target.AddComponent<Pickupable>();
                    ErrorMessage.AddDebug(target.name + " add pick up");
                }
                else
                {
                    Destroy(pickupable);
                    ErrorMessage.AddDebug(target.name + " remove pick up");
                }
            }
            else
            {
                ErrorMessage.AddDebug("Target not found");
            }
        }

        public void OnConsoleCommand_size(NotificationCenter.Notification n)
        {
            if (!GetTarget(out var target))
            {
                ErrorMessage.AddDebug("Target not found");
                return;
            }
            
            x = 1f;
            y = 1f;
            z = 1f;
            var flag = n.data.Count == 1;
            if (flag)
            {
                x = float.Parse((string) n.data[0]);
                y = float.Parse((string) n.data[0]);
                z = float.Parse((string) n.data[0]);
            }

            var flag2 = n.data.Count == 2;
            if (flag2)
            {
                x = float.Parse((string) n.data[0]);
                y = float.Parse((string) n.data[1]);
                z = float.Parse((string) n.data[1]);
            }

            var flag3 = n.data.Count == 3;
            if (flag3)
            {
                x = float.Parse((string) n.data[0]);
                y = float.Parse((string) n.data[1]);
                z = float.Parse((string) n.data[2]);
            }

            target.transform.localScale = new Vector3(x, y, z);
            ErrorMessage.AddDebug(target.name + " Size changed complete");

        }

        public void OnConsoleCommand_playse(NotificationCenter.Notification n)
        {
            var num = float.Parse((string)n.data[0]);
            Player.mainObject.transform.localScale = new Vector3(num, num, num);
            ErrorMessage.AddDebug("Warning!!!! This feature may cause you to be unable to move and other issues");
        }

        public void OnConsoleCommand_friend()
        {
            if (!GetTarget(out var target))
            {
                ErrorMessage.AddDebug("Target not found");
                return;
            }
            var creature = target.GetComponent<Creature>();
            if (creature is null)
            {
                ErrorMessage.AddDebug($"{target.name} is not a creature");
                return;
            }
            
#if SN1
            if(creature.friend != Player.main.gameObject)
#elif BZ
            if(creature.GetFriend() != Player.main.gameObject)
#endif
            {
                creature.Friendliness.Add(1f);
#if SN1
                creature.friend = Player.main.gameObject;
#elif BZ
				creature.SetFriend(Player.main.gameObject);
#endif
                ErrorMessage.AddDebug(target.name + " set friend");
            }
            else
            {
                creature.Friendliness.Value = 0f;
#if SN1
                creature.friend = null;
#elif BZ
				creature.SetFriend(null);
#endif
                ErrorMessage.AddDebug(target.name + " remove friend");
            }
            
        }

        public void OnConsoleCommand_spse(NotificationCenter.Notification n)
        {
            if (n is not {data: {Count: > 0}}) return;
            var text = (string)n.data[0];
            if(Utils.TryParseEnum<TechType>(text, out var techType))
            {
                if(CraftData.IsAllowed(techType))
                {
                    CoroutineHost.StartCoroutine(SpawnTechType(n, techType));
                }
            }
            else
            {
                ErrorMessage.AddDebug("Unable to read " + text + " as TechType");
            }
        }

        private IEnumerator SpawnTechType(NotificationCenter.Notification n, TechType techType)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(techType, false);
            yield return task;
            var prefabForTechType = task.GetResult();
            if (prefabForTechType == null)
            {
                ErrorMessage.AddDebug("Prefab not found for " + techType);
                yield break;
            }
            
            x = 1f;
            y = 1f;
            z = 1f;
            var num = 6;
            var num2 = 1;

            switch (n.data.Count)
            {
                case > 5:
                    num = int.Parse((string) n.data[5]);
                    z = float.Parse((string) n.data[4]);
                    y = float.Parse((string) n.data[3]);
                    x = float.Parse((string) n.data[2]);
                    num2 = int.Parse((string) n.data[1]);
                    break;
                case > 4:
                    z = float.Parse((string) n.data[4]);
                    y = float.Parse((string) n.data[3]);
                    x = float.Parse((string) n.data[2]);
                    num2 = int.Parse((string) n.data[1]);
                    break;
                case 4:
                    z = float.Parse((string) n.data[3]);
                    y = float.Parse((string) n.data[3]);
                    x = float.Parse((string) n.data[2]);
                    num2 = int.Parse((string) n.data[1]);
                    break;
                case 3:
                    x = float.Parse((string) n.data[2]);
                    y = float.Parse((string) n.data[2]);
                    z = float.Parse((string) n.data[2]);
                    num2 = int.Parse((string) n.data[1]);
                    break;
                case > 1:
                    num2 = int.Parse((string) n.data[1]);
                    break;
            }
                
            for (var i = 0; i < num2; i++)
            {
                var go = global::Utils.CreatePrefab(prefabForTechType, num, i > 0);
                var rootGO = Utils.GetEntityRoot(go)?? go;
                rootGO.transform.localScale = new Vector3(x, y, z);
                rootGO.AddComponent<ResizerMono>().Setsize(x, y, z);
                LargeWorldEntity.Register(rootGO);
                CrafterLogic.NotifyCraftEnd(rootGO, techType);
                rootGO.SendMessage("StartConstruction", 1);
            }
        }

        public void OnConsoleCommand_suse(NotificationCenter.Notification n)
        {
            var text = (string)n.data[0];
            var flag = !string.IsNullOrEmpty(text);
            if(flag)
            {
                x = 1f;
                y = 1f;
                z = 1f;
                var num = 6;
                
                switch (n.data.Count)
                {
                    case > 4:
                        num = int.Parse((string)n.data[4]);
                        z = float.Parse((string)n.data[3]);
                        y = float.Parse((string)n.data[2]);
                        x = float.Parse((string)n.data[1]);
                        break;
                    case > 3:
                        z = float.Parse((string)n.data[3]);
                        y = float.Parse((string)n.data[2]);
                        x = float.Parse((string)n.data[1]);
                        break;
                    case 3:
                        x = float.Parse((string)n.data[1]);
                        y = float.Parse((string)n.data[2]);
                        z = float.Parse((string)n.data[2]);
                        break;
                    case 2:
                        x = float.Parse((string)n.data[1]);
                        y = float.Parse((string)n.data[1]);
                        z = float.Parse((string)n.data[1]);
                        break;
                }

                var cameraTransform = MainCamera.camera.transform;
                spawnPosition = cameraTransform.position + num * cameraTransform.forward;
                spawnRotation = Quaternion.LookRotation(cameraTransform.right);
                LightmappedPrefabs.main.RequestScenePrefab(text, OnSubPrefabLoaded);
            }
            else
            {
                ErrorMessage.AddDebug("Must specify name (cyclops beetle aurora)");
            }
        }

        private void OnSubPrefabLoaded(GameObject prefab)
        {
            var go = global::Utils.SpawnPrefabAt(prefab, null, spawnPosition);
            var rootGO = Utils.GetEntityRoot(go)?? go;
            rootGO.transform.localScale = new Vector3(x, y, z);
            rootGO.AddComponent<ResizerMono>().Setsize(x, y, z);
            rootGO.transform.rotation = spawnRotation;
            rootGO.SetActive(true);
            rootGO.SendMessage("StartConstruction", 1);
            LargeWorldEntity.Register(rootGO);
            CrafterLogic.NotifyCraftEnd(rootGO, CraftData.GetTechType(rootGO));
        }

    }
}
