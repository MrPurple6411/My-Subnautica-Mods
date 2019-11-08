using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeamothDrillArm.MonoBehaviours
{
    /* Just ignore this class
     * This is a complete copy-paste of the Drillable class using dnSpy.
     * I did this so that I could have complete control over the Drillable component without using Reflectin
     * I know this is a very messy class, but it doesn't really matter
     * - AHK1221
     */
    public class BetterDrillable : MonoBehaviour
    {
        public Drillable drillable;

        public event Drillable.OnDrilled onDrilled;

        public Drillable.ResourceType[] resources;

        public GameObject breakFX;

        public GameObject breakAllFX;

        public string primaryTooltip;

        public string secondaryTooltip;

        public bool deleteWhenDrilled = true;

        public GameObject modelRoot;

        public int minResourcesToSpawn = 1;

        public int maxResourcesToSpawn = 3;

        public bool lootPinataOnSpawn = true;

        public MeshRenderer[] renderers;

        public const float drillDamage = 5f;

        public const float maxHealth = 200f;

        public float timeLastDrilled;

        public List<GameObject> lootPinataObjects = new List<GameObject>();

        public Vehicle drillingVehicle;

        public float kChanceToSpawnResources = 1f;

        public const int currentVersion = 1;

        public float[] health;

        public void Start()
        {
            renderers = GetComponentsInChildren<MeshRenderer>();
            if (health == null)
            {
                health = new float[renderers.Length];
                for (int i = 0; i < health.Length; i++)
                {
                    health[i] = Main.DrillNodeHealth;
                }
            }
            else
            {
                if (health.Length != renderers.Length)
                {
                    float[] array = (float[])health.Clone();
                    health = new float[renderers.Length];
                    for (int j = 0; j < health.Length; j++)
                    {
                        if (j < array.Length)
                        {
                            health[j] = array[j];
                        }
                        else
                        {
                            health[j] = Main.DrillNodeHealth;
                        }
                    }
                }
                for (int k = 0; k < health.Length; k++)
                {
                    renderers[k].gameObject.SetActive(health[k] > 0f);
                }
            }
            TechType dominantResourceType = GetDominantResourceType();
            if (string.IsNullOrEmpty(primaryTooltip))
            {
                primaryTooltip = dominantResourceType.AsString(false);
            }
            if (string.IsNullOrEmpty(secondaryTooltip))
            {
                string arg = Language.main.Get(dominantResourceType);
                string arg2 = Language.main.Get(TooltipFactory.techTypeTooltipStrings.Get(dominantResourceType));
                secondaryTooltip = Language.main.GetFormat<string, string>("DrillResourceTooltipFormat", arg, arg2);
            }
        }

        public void HoverDrillable()
        {
            var vehicle = Player.main.GetVehicle();

            var canDrill = false;
            var hand = HandReticle.Hand.Left;

            if (!vehicle)
                canDrill = false;
            else if (vehicle.GetType().Equals(typeof(Exosuit)))
            {
                var exosuit = (Exosuit)vehicle;

                if(exosuit.HasDrill())
                {
                    canDrill = true;
                    hand = (exosuit.leftArmType == TechType.ExosuitDrillArmModule) ? HandReticle.Hand.Left : HandReticle.Hand.Right;
                }
            }
            else if(vehicle.GetType().Equals(typeof(SeaMoth)))
            {
                var seamoth = (SeaMoth)vehicle;
                if (seamoth.modules.GetCount(SeamothModule.SeamothDrillModule) > 0)
                {
                    canDrill = true;
                    hand = HandReticle.Hand.Left;
                }
            }
            if (canDrill)
            {
                HandReticle.main.SetInteractText(Language.main.GetFormat<string>("DrillResource", Language.main.Get(primaryTooltip)), secondaryTooltip, false, true, hand);
                HandReticle.main.SetIcon(HandReticle.IconType.Drill, 1f);
            }
            else
            {
                HandReticle.main.SetInteractText(primaryTooltip, "NeedExoToMine");
            }
        }

        public TechType GetDominantResourceType()
        {
            TechType result = TechType.None;
            float num = 0f;
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].chance > num)
                {
                    num = resources[i].chance;
                    result = resources[i].techType;
                }
            }
            return result;
        }

        public void Restore()
        {
            for (int i = 0; i < health.Length; i++)
            {
                health[i] = Main.DrillNodeHealth;
                renderers[i].gameObject.SetActive(true);
            }
        }

        public void OnDrill(Vector3 position, Vehicle veh, out GameObject hitObject)
        {
            float num = 0f;
            for (int i = 0; i < health.Length; i++)
            {
                num += health[i];
            }
            this.drillingVehicle = veh;
            Vector3 zero = Vector3.zero;
            int num2 = FindClosestMesh(position, out zero);
            hitObject = this.renderers[num2].gameObject;
            this.timeLastDrilled = Time.time;
            if (num > 0f)
            {
                float num3 = health[num2];
                this.health[num2] = Mathf.Max(0f, this.health[num2] - 5f);
                num -= num3 - health[num2];
                if (num3 > 0f && this.health[num2] <= 0f)
                {
                    this.renderers[num2].gameObject.SetActive(false);
                    this.SpawnFX(breakFX, zero);
                    if (UnityEngine.Random.value < kChanceToSpawnResources)
                    {
                        SpawnLoot(zero);
                    }
                }
                if (num <= 0f)
                {
                    this.gameObject.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);
                    this.SpawnFX(this.breakAllFX, zero);
                    onDrilled.Invoke(drillable);
                    if (deleteWhenDrilled)
                    {
                        float time = (!lootPinataOnSpawn) ? 0f : 6f;
                        this.Invoke("DestroySelf", time);
                    }
                }
            }
            BehaviourUpdateUtils.Register(drillable);
            drillable.health = health;
        }

        public void DestroySelf()
        {
            // https://github.com/Vlad-00003/SubnauticaMods/blob/master/LargeDepositsFix/Drillable_DestroySelf_Patch.cs
            base.gameObject.SendMessage("OnBreakResource",null, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }

        public void ClipWithTerrain(ref Vector3 position)
        {
            Vector3 origin = position;
            origin.y = base.transform.position.y + 5f;
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 10f, Voxeland.GetTerrainLayerMask(), QueryTriggerInteraction.Ignore))
            {
                position.y = Mathf.Max(position.y, raycastHit.point.y + 0.3f);
            }
        }

        public void SpawnLoot(Vector3 position)
        {
            if (resources.Length > 0)
            {
                var def = this.GetComponent<Drillable>();
                if (def != null)
                {
                    minResourcesToSpawn = def.minResourcesToSpawn;
                    maxResourcesToSpawn = def.maxResourcesToSpawn;
                }
                int num = UnityEngine.Random.Range(minResourcesToSpawn, maxResourcesToSpawn);
                for (int i = 0; i < num; i++)
                {
                    GameObject gameObject = ChooseRandomResource();
                    if (gameObject)
                    {
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                        Vector3 position2 = position;
                        float num2 = 1f;
                        position2.x += UnityEngine.Random.Range(-num2, num2);
                        position2.z += UnityEngine.Random.Range(-num2, num2);
                        position2.y += UnityEngine.Random.Range(-num2, num2);
                        ClipWithTerrain(ref position2);
                        gameObject2.transform.position = position2;
                        Vector3 vector = UnityEngine.Random.onUnitSphere;
                        vector.y = 0f;
                        vector = Vector3.Normalize(vector);
                        vector.y = 1f;
                        gameObject2.GetComponent<Rigidbody>().isKinematic = false;
                        gameObject2.GetComponent<Rigidbody>().AddForce(vector);
                        gameObject2.GetComponent<Rigidbody>().AddTorque(Vector3.right * UnityEngine.Random.Range(3f, 6f));
                        if (lootPinataOnSpawn)
                        {
                            StartCoroutine(AddResourceToPinata(gameObject2));
                        }
                    }
                }
            }
        }

        public IEnumerator AddResourceToPinata(GameObject resource)
        {
            yield return new WaitForSeconds(1.5f);
            lootPinataObjects.Add(resource);
            yield break;
        }

        public int FindClosestMesh(Vector3 position, out Vector3 center)
        {
            int result = 0;
            float num = float.PositiveInfinity;
            center = Vector3.zero;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].gameObject.activeInHierarchy)
                {
                    Bounds encapsulatedAABB = UWE.Utils.GetEncapsulatedAABB(renderers[i].gameObject, -1);
                    float sqrMagnitude = (encapsulatedAABB.center - position).sqrMagnitude;
                    if (sqrMagnitude < num)
                    {
                        num = sqrMagnitude;
                        result = i;
                        center = encapsulatedAABB.center;
                        if (sqrMagnitude <= 0.5f)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public GameObject ChooseRandomResource()
        {
            GameObject result = null;
            for (int i = 0; i < resources.Length; i++)
            {
                Drillable.ResourceType resourceType = resources[i];
                if (resourceType.chance >= 1f)
                {
                    result = CraftData.GetPrefabForTechType(resourceType.techType, true);
                    break;
                }
                PlayerEntropy component = Player.main.gameObject.GetComponent<PlayerEntropy>();
                if (component.CheckChance(resourceType.techType, resourceType.chance))
                {
                    result = CraftData.GetPrefabForTechType(resourceType.techType, true);
                    break;
                }
            }
            return result;
        }

        public void SpawnFX(GameObject fx, Vector3 position)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fx);
            gameObject.transform.position = position;
        }

        public void ManagedUpdate()
        {
            if (timeLastDrilled + 0.5f > Time.time)
            {
                modelRoot.transform.position = transform.position + new Vector3(Mathf.Sin(Time.time * 60f), Mathf.Cos(Time.time * 58f + 0.5f), Mathf.Cos(Time.time * 64f + 2f)) * 0.011f;
            }
            if (lootPinataObjects.Count > 0 && drillingVehicle)
            {
                List<GameObject> list = new List<GameObject>();
                foreach (GameObject gameObject in lootPinataObjects)
                {
                    if (gameObject == null)
                    {
                        list.Add(gameObject);
                    }
                    else
                    {
                        Vector3 b = drillingVehicle.transform.position + new Vector3(0f, 0.8f, 0f);
                        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, b, Time.deltaTime * 5f);
                        float num = Vector3.Distance(gameObject.transform.position, b);
                            Pickupable pickupable = gameObject.GetComponentInChildren<Pickupable>();
                            if (pickupable)
                            {
                                var storage = GetStorageContainer(drillingVehicle, pickupable);
                                if(storage == null)
                                {
                                    ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                                }
                                else
                                {
                                    var name = Language.main.Get(pickupable.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                                    uGUI_IconNotifier.main.Play(pickupable.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);

                                    pickupable = pickupable.Initialize();

                                    var item = new InventoryItem(pickupable);
                                    storage.UnsafeAdd(item);
                                    pickupable.PlayPickupSound();
                                }
                                list.Add(gameObject);
                            }
                    }
                }
                if (list.Count > 0)
                {
                    foreach (GameObject item2 in list)
                    {
                        lootPinataObjects.Remove(item2);
                    }
                }
            }
        }

        public ItemsContainer GetStorageContainer(Vehicle veh, Pickupable pickupable)
        {
            if(veh.GetType().Equals(typeof(Exosuit)))
            {
                var storageContainer = ((Exosuit)veh).storageContainer;

                if (storageContainer.container.HasRoomFor(pickupable))
                    return storageContainer.container;
            }
            else
            {
                var seamoth = (SeaMoth)veh;

                for(int i = 0; i < 8; i++)
                {
                    var storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                    if (storage != null && storage.HasRoomFor(pickupable))
                        return storage;
                }
            }

            return null;
        }
    }
}
