using UnityEngine;
using UWE;
using System.Text;
using System.Collections.Generic;

namespace BuilderModule
{
    public class BuilderModuleSeamoth : MonoBehaviour
    {
        public BuilderModuleSeamoth Instance { get; private set; }
        public int moduleSlotID { get; set; }
        private SeaMoth thisSeamoth { get; set; }
        private Player playerMain { get; set; }
        private EnergyMixin energyMixin { get; set; }

        private bool isToggle;
        private bool isActive;
        private bool isPlayerInThisSeamoth;

        public float powerConsumptionConstruct = 0.5f;
        public float powerConsumptionDeconstruct = 0.5f;
        public FMOD_CustomLoopingEmitter buildSound;
        private FMODAsset completeSound;
        private bool isConstructing;
        private Constructable constructable;
        private int handleInputFrame = -1;
        private string deconstructText;
        private string constructText;
        private string noPowerText;




        public void Awake()
        {
            Instance = this;
            thisSeamoth = Instance.GetComponent<SeaMoth>();
            energyMixin = thisSeamoth.GetComponent<EnergyMixin>();
            playerMain = Player.main;
            isPlayerInThisSeamoth = playerMain.GetVehicle() == thisSeamoth ? true : false;
            OnPlayerModeChanged(Player.main.GetMode());
            var builderPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Builder").GetComponent<BuilderTool>();
            completeSound = Instantiate(builderPrefab.completeSound, gameObject.transform);
        }


        private void Start()
        {
            thisSeamoth.onToggle += OnToggle;
            thisSeamoth.modules.onAddItem += OnAddItem;
            thisSeamoth.modules.onRemoveItem += OnRemoveItem;
            playerMain.playerModeChanged.AddHandler(gameObject, new Event<Player.Mode>.HandleFunction(OnPlayerModeChanged));
            OnPlayerModeChanged(Player.main.GetMode());
        }

        private void OnRemoveItem(InventoryItem item)
        {
            if (item.item.GetTechType() == BuilderModulePrefab.TechTypeID)
            {                
                moduleSlotID = -1;
                Instance.enabled = false;
            }
        }

        private void OnAddItem(InventoryItem item)
        {
            if (item.item.GetTechType() == BuilderModulePrefab.TechTypeID)
            {
                moduleSlotID = thisSeamoth.GetSlotByItem(item);
                Instance.enabled = true;
            }
        }

        private void OnPlayerModeChanged(Player.Mode playerMode)
        {
            if (playerMode == Player.Mode.LockedPiloting)
            {
                if (playerMain.GetVehicle() == thisSeamoth)
                {
                    isPlayerInThisSeamoth = true;
                    OnEnable();
                    return;
                }
                else
                {
                    isPlayerInThisSeamoth = false;
                    OnDisable();
                    return;
                }
            }
            else
            {
                isPlayerInThisSeamoth = false;
                OnDisable();
            }
        }


        private void OnToggle(int slotID, bool state)
        {
            if (thisSeamoth.GetSlotBinding(slotID) == BuilderModulePrefab.TechTypeID)
            {
                isToggle = state;

                if (isToggle)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        public void OnEnable()
        {
            isActive = isPlayerInThisSeamoth && playerMain.isPiloting && isToggle && moduleSlotID > -1;
        }

        public void OnDisable()
        {
            isActive = false;
            isConstructing = false;
        }


        private void Update()
        {
            if (isActive)
            {
                if (GameInput.GetButtonDown(GameInput.Button.LeftHand) && !isConstructing && !GameInput.GetButtonHeld(GameInput.Button.RightHand))
                {
                    isConstructing = true;
                }

                if (GameInput.GetButtonHeld(GameInput.Button.LeftHand) && !GameInput.GetButtonHeld(GameInput.Button.RightHand))
                {
                    isConstructing = true;
                }


                if (GameInput.GetButtonUp(GameInput.Button.LeftHand))
                {
                    isConstructing = false;
                }

                if (GameInput.GetButtonDown(GameInput.Button.RightHand) && !Builder.isPlacing)
                {
                    if (energyMixin.charge > 0f)
                    {
                        uGUI_BuilderMenu.Show();
                    }
                }
                this.UpdateText();
                this.HandleInput();
            }
        }

        private void HandleInput()
        {
            if (this.handleInputFrame == Time.frameCount)
            {
                return;
            }
            this.handleInputFrame = Time.frameCount;
            if (Builder.isPlacing || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            bool flag = this.TryDisplayNoPowerTooltip();
            if (flag)
            {
                return;
            }
            Targeting.AddToIgnoreList(Player.main.gameObject);
            GameObject gameObject;
            float num;
            Targeting.GetTarget(30f, out gameObject, out num, null);
            if (gameObject == null)
            {
                return;
            }
            bool buttonHeld = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
            bool buttonDown = GameInput.GetButtonDown(GameInput.Button.Deconstruct);
            bool buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.Deconstruct);
            Constructable constructable = gameObject.GetComponentInParent<Constructable>();
            if (constructable != null && num > constructable.placeMaxDistance)
            {
                constructable = null;
            }
            if (constructable != null)
            {
                this.OnHover(constructable);
                string text;
                if (buttonHeld)
                {
                    this.Construct(constructable, true);
                }
                else if (constructable.DeconstructionAllowed(out text))
                {
                    if (buttonHeld2)
                    {
                        if (constructable.constructed)
                        {
                            constructable.SetState(false, false);
                        }
                        else
                        {
                            this.Construct(constructable, false);
                        }
                    }
                }
                else if (buttonDown && !string.IsNullOrEmpty(text))
                {
                    ErrorMessage.AddMessage(text);
                }
            }
            else
            {
                BaseDeconstructable baseDeconstructable = gameObject.GetComponentInParent<BaseDeconstructable>();
                if (baseDeconstructable == null)
                {
                    BaseExplicitFace componentInParent = gameObject.GetComponentInParent<BaseExplicitFace>();
                    if (componentInParent != null)
                    {
                        baseDeconstructable = componentInParent.parent;
                    }
                }
                if (baseDeconstructable != null)
                {
                    string text;
                    if (baseDeconstructable.DeconstructionAllowed(out text))
                    {
                        this.OnHover(baseDeconstructable);
                        if (buttonDown)
                        {
                            baseDeconstructable.Deconstruct();
                        }
                    }
                    else if (buttonDown && !string.IsNullOrEmpty(text))
                    {
                        ErrorMessage.AddMessage(text);
                    }
                }
            }
        }

        private bool TryDisplayNoPowerTooltip()
        {
            if (this.energyMixin.charge <= 0f)
            {
                HandReticle main = HandReticle.main;
                main.SetInteractText(this.noPowerText, false, HandReticle.Hand.None);
                main.SetIcon(HandReticle.IconType.Default, 1f);
                return true;
            }
            return false;
        }

        private void UpdateText()
        {
            string buttonFormat = LanguageCache.GetButtonFormat("ConstructFormat", GameInput.Button.LeftHand);
            string buttonFormat2 = LanguageCache.GetButtonFormat("DeconstructFormat", GameInput.Button.Deconstruct);
            this.constructText = Language.main.GetFormat<string, string>("ConstructDeconstructFormat", buttonFormat, buttonFormat2);
            this.deconstructText = buttonFormat2;
            this.noPowerText = Language.main.Get("NoPower");
        }
        private bool Construct(Constructable c, bool state)
        {
            if (c != null && !c.constructed && this.energyMixin.charge > 0f)
            {
                float amount = ((!state) ? this.powerConsumptionDeconstruct : this.powerConsumptionConstruct) * Time.deltaTime;
                this.energyMixin.ConsumeEnergy(amount);
                bool constructed = c.constructed;
                bool flag = (!state) ? c.Deconstruct() : c.Construct();
                if (flag)
                {
                    this.constructable = c;
                }
                else if (state && !constructed)
                {
                    global::Utils.PlayFMODAsset(this.completeSound, c.transform, 20f);
                }
                return true;
            }
            return false;
        }

        private void OnHover(Constructable constructable)
        {
            if (isActive)
            {
                HandReticle main = HandReticle.main;
                if (constructable.constructed)
                {
                    main.SetInteractText(Language.main.Get(constructable.techType), this.deconstructText, false, false, HandReticle.Hand.Left);
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(this.constructText);
                    foreach (KeyValuePair<TechType, int> keyValuePair in constructable.GetRemainingResources())
                    {
                        TechType key = keyValuePair.Key;
                        string text = Language.main.Get(key);
                        int value = keyValuePair.Value;
                        if (value > 1)
                        {
                            stringBuilder.AppendLine(Language.main.GetFormat<string, int>("RequireMultipleFormat", text, value));
                        }
                        else
                        {
                            stringBuilder.AppendLine(text);
                        }
                    }
                    main.SetInteractText(Language.main.Get(constructable.techType), stringBuilder.ToString(), false, false, HandReticle.Hand.Left);
                    main.SetProgress(constructable.amount);
                    main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                }
            }
        }

        private void OnHover(BaseDeconstructable deconstructable)
        {
            if (isActive)
            {
                HandReticle main = HandReticle.main;
                main.SetInteractText(deconstructable.Name, this.deconstructText);
            }
        }

        private void OnDestroy()
        {
            playerMain.playerModeChanged.RemoveHandler(gameObject, OnPlayerModeChanged);
            thisSeamoth.onToggle -= OnToggle;
            thisSeamoth.modules.onAddItem -= OnAddItem;
            thisSeamoth.modules.onRemoveItem -= OnRemoveItem;
            OnDisable();
        } 
    }
}

