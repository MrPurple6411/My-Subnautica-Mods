using UnityEngine;
using System.Reflection;
using System;
using SeamothClawArm.Patches;

namespace SeamothClawArm.MonoBehaviours
{
    public class SeamothClaw : MonoBehaviour
    {
        public bool toggle;

        private float cooldownTime = Time.time;
        public const float cooldownHit = 1f;
        public const float cooldownPickup = 1.533f;

        public Animator animator;
        public FMODAsset hitTerrainSound;
        public FMODAsset hitFishSound;
        public FMODAsset pickupSound;
        public Transform front;
        public VFXController fxControl;
        
        private static MethodInfo GetArmPrefabMethod =
            typeof(Exosuit).GetMethod("GetArmPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

        private SeaMoth seamoth;

        public void Awake()
        {
            var exosuitPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>();

            var exosuitClawArmGO = (GameObject)GetArmPrefabMethod.Invoke(exosuitPrefab, new object[] { TechType.ExosuitClawArmModule });
            var exosuitClawArm = exosuitClawArmGO.GetComponent<ExosuitClawArm>();
            animator = exosuitClawArm.animator;
            hitTerrainSound = exosuitClawArm.hitTerrainSound;
            hitFishSound = exosuitClawArm.hitFishSound;
            pickupSound = exosuitClawArm.pickupSound;
            front = exosuitClawArm.front;
            fxControl = exosuitClawArm.fxControl;
        }

        void Start()
        {
            // Get Seamoth component on the current GameObject.
            seamoth = GetComponent<SeaMoth>();
        }

        void Update()
        {
            // If its not selected, we don't want to run the rest of the function
            if (!toggle) return;

            // Some checks to see if we can pick up or not.
            if (seamoth.modules.GetCount(SeamothModule.SeamothClawModule) <= 0) return;
            if (!seamoth.GetPilotingMode()) return;
            if (Player.main.GetPDA().isOpen) return;

            // Update hovering.
            UpdateActiveTarget(seamoth);

            // If we let up the Left Mouse Button
            if (GameInput.GetButtonUp(GameInput.Button.LeftHand))
            {
                if (Time.time > this.cooldownTime)
                {
                    // Try Use!
                    TryUse();
                }
            }
        }

        void UpdateActiveTarget(Vehicle vehicle)
        {
            // Get the GameObject we're looking at
            var activeTarget = default(GameObject);
            Targeting.GetTarget(vehicle.gameObject, 6f, out activeTarget, out float dist, null);

            // Check if not null
            if (activeTarget != null)
            {
                // Get the root object, or the hit object if root is null
                var root = UWE.Utils.GetEntityRoot(activeTarget) ?? activeTarget;
                if (root.GetComponentProfiled<Pickupable>())
                    activeTarget = root;
                else
                    root = null;
            }

            // Get the GUIHand component
            var guiHand = Player.main.GetComponent<GUIHand>();
            if (activeTarget)
            {
                // Send the Hover message to the GameObject we're looking at.
                GUIHand.Send(activeTarget, HandTargetEventType.Hover, guiHand);
            }
        }

        public ItemsContainer GetStorageContainer(Pickupable pickupable)
        {
            for (int i = 0; i < 8; i++)
            {
                var storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                if (storage != null && storage.HasRoomFor(pickupable))
                    return storage;
            }

            return null;
        }

        void TryUse()
        {
            Pickupable pickupable = null;
            PickPrefab component = null;
            
            var pos = Vector3.zero;
            var hitObject = default(GameObject);

            UWE.Utils.TraceFPSTargetPosition(seamoth.gameObject, 6f, ref hitObject, ref pos, true);

            if (hitObject)
            {
                pickupable = hitObject.FindAncestor<Pickupable>();
                component = hitObject.FindAncestor<PickPrefab>();
            }
            if (pickupable != null && pickupable.isPickupable)
            {
                if (GetStorageContainer(pickupable) != null)
                {
                    this.animator.SetTrigger("use_tool");
                    OnPickup(pickupable, component);
                }
                else
                {
                    ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                }
            }
            else
            {
                if (component != null)
                {
                    this.animator.SetTrigger("use_tool");
                    OnPickup(pickupable, component);
                }
                this.animator.SetTrigger("bash");
                this.fxControl.Play(0);
                OnHit();
            }
        }

        void OnPickup(Pickupable pickupable, PickPrefab component)
        {
            if (pickupable != null && pickupable.isPickupable && GetStorageContainer(pickupable).HasRoomFor(pickupable))
            {
                pickupable = pickupable.Initialize();
                InventoryItem item = new InventoryItem(pickupable);
                GetStorageContainer(pickupable).UnsafeAdd(item);
                global::Utils.PlayFMODAsset(pickupSound, this.front, 5f);
            }
            else if (component != null && component.AddToContainer(GetStorageContainer(pickupable)))
            {
                component.SetPickedUp();
            }

            this.cooldownTime = Time.time + cooldownPickup;
        }

        void OnHit()
        {
            if (seamoth.CanPilot() && seamoth.GetPilotingMode())
            {
                Vector3 position = default(Vector3);
                GameObject gameObject = null;
                UWE.Utils.TraceFPSTargetPosition(seamoth.gameObject, 6.5f, ref gameObject, ref position, true);
                if (gameObject == null)
                {
                    InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
                    if (component != null && component.GetMostRecent() != null)
                    {
                        gameObject = component.GetMostRecent().gameObject;
                    }
                }
                if (gameObject)
                {
                    LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
                    if (liveMixin)
                    {
                        bool flag = liveMixin.IsAlive();
                        liveMixin.TakeDamage(50f, position, DamageType.Normal, null);
                        global::Utils.PlayFMODAsset(hitFishSound, this.front, 5f);
                    }
                    else
                    {
                        global::Utils.PlayFMODAsset(hitTerrainSound, this.front, 5f);
                    }
                    VFXSurface component2 = gameObject.GetComponent<VFXSurface>();
                    Vector3 euler = MainCameraControl.main.transform.eulerAngles + new Vector3(300f, 90f, 0f);
                    VFXSurfaceTypeManager.main.Play(component2, VFXEventTypes.impact, position, Quaternion.Euler(euler), seamoth.gameObject.transform);
                    gameObject.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                }
            }

            this.cooldownTime = Time.time + cooldownHit;
        }
    }
}