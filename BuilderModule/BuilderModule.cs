using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BuilderModule
{
    public class BuilderModule : MonoBehaviour
    {
        public BuilderModule Instance { get; private set; }
        public int ModuleSlotID { get; set; }
        private Vehicle ThisVehicle { get; set; }
        private Player PlayerMain { get; set; }
        private EnergyMixin EnergyMixin { get; set; }

        private bool isToggle;
        private bool isActive;

        public float powerConsumptionConstruct = 0.5f;
        public float powerConsumptionDeconstruct = 0.5f;
        public FMOD_CustomLoopingEmitter buildSound;
        private FMODAsset completeSound;
        private int handleInputFrame = -1;
        private string deconstructText;
        private string constructText;
        private string noPowerText;




        public void Awake()
        {
            Instance = this;
            ThisVehicle = Instance.GetComponent<Vehicle>();
            EnergyMixin = ThisVehicle.GetComponent<EnergyMixin>();
            PlayerMain = Player.main;
            var builderPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Builder").GetComponent<BuilderTool>();
            completeSound = Instantiate(builderPrefab.completeSound, gameObject.transform);
        }


        private void Start()
        {
            ThisVehicle.onToggle += OnToggle;
            ThisVehicle.modules.onAddItem += OnAddItem;
            ThisVehicle.modules.onRemoveItem += OnRemoveItem;
        }

        private void OnRemoveItem(InventoryItem item)
        {
            if (item.item.GetTechType() == BuilderModulePrefab.TechTypeID)
            {
                ModuleSlotID = -1;
                Instance.enabled = false;
            }
        }

        private void OnAddItem(InventoryItem item)
        {
            if (item.item.GetTechType() == BuilderModulePrefab.TechTypeID)
            {
                ModuleSlotID = ThisVehicle.GetSlotByItem(item);
                Instance.enabled = true;
            }
        }

        private void OnToggle(int slotID, bool state)
        {
            if (ThisVehicle.GetSlotBinding(slotID) == BuilderModulePrefab.TechTypeID)
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
            isActive = PlayerMain.isPiloting && isToggle && ModuleSlotID > -1;
        }

        public void OnDisable()
        {
            isActive = false;
            uGUI_BuilderMenu.Hide();
            Builder.End();
        }


        private void Update()
        {
            this.UpdateText();
            if (isActive)
            {
                if (ThisVehicle.GetActiveSlotID() != ModuleSlotID)
                {
                    ThisVehicle.SlotKeyDown(ThisVehicle.GetActiveSlotID());
                    ThisVehicle.SlotKeyUp(ThisVehicle.GetActiveSlotID());
                }
                if (GameInput.GetButtonDown(GameInput.Button.PDA) && !Player.main.GetPDA().isOpen && !Builder.isPlacing)
                {
                    if (EnergyMixin.charge > 0f)
                    {
                        Player.main.GetPDA().Close();
                        uGUI_BuilderMenu.Show();
                        handleInputFrame = Time.frameCount;
                    }
                }
                if (Builder.isPlacing)
                {
                    if (Player.main.GetLeftHandDown())
                    {
                        UWE.Utils.lockCursor = true;
                    }
                    if (UWE.Utils.lockCursor && GameInput.GetButtonDown(GameInput.Button.AltTool))
                    {
                        if (Builder.TryPlace())
                        {
                            Builder.End();
                        }
                    }
                    else if (this.handleInputFrame != Time.frameCount && GameInput.GetButtonDown(GameInput.Button.Deconstruct))
                    {
                        Builder.End();
                    }
                    FPSInputModule.current.EscapeMenu();
                    Builder.Update();
                }
                if (!uGUI_BuilderMenu.IsOpen() && !Builder.isPlacing)
                {
                    this.HandleInput();
                }
            }
        }

        private void HandleInput()
        {
            if (this.handleInputFrame == Time.frameCount)
            {
                return;
            }
            this.handleInputFrame = Time.frameCount;
            if (!AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            bool flag = this.TryDisplayNoPowerTooltip();
            if (flag)
            {
                return;
            }
            Targeting.AddToIgnoreList(Player.main.gameObject);
            Targeting.GetTarget(60f, out GameObject gameObject, out float num);
            if (gameObject == null)
            {
                return;
            }
            bool buttonHeld = GameInput.GetButtonHeld(GameInput.Button.AltTool);
            bool buttonDown = GameInput.GetButtonDown(GameInput.Button.Deconstruct);
            bool buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.Deconstruct);
            bool quickbuild = GameInput.GetButtonHeld(GameInput.Button.Sprint);
            Constructable constructable = gameObject.GetComponentInParent<Constructable>();
            if (constructable != null && num > constructable.placeMaxDistance * 2)
            {
                constructable = null;
            }
            if (constructable != null)
            {
                this.OnHover(constructable);
                if (buttonHeld)
                {
                    this.Construct(constructable, true);
                    if (quickbuild)
                    {
                        this.Construct(constructable, true);
                        this.Construct(constructable, true);
                        this.Construct(constructable, true);
                    }
                }
                else if (constructable.DeconstructionAllowed(out string text))
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
                            if (quickbuild)
                            {
                                this.Construct(constructable, false);
                                this.Construct(constructable, false);
                                this.Construct(constructable, false);
                            }
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
                else
                {
                    if (baseDeconstructable.DeconstructionAllowed(out string text))
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
            if (this.EnergyMixin.charge <= 0f)
            {
                HandReticle main = HandReticle.main;
#if SUBNAUTICA
                main.SetInteractText(this.noPowerText, false, HandReticle.Hand.None);
#elif BELOWZERO
                main.SetText(HandReticle.TextType.Hand, this.noPowerText, true);
#endif
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
            if (c != null && !c.constructed && this.EnergyMixin.charge > 0f)
            {
                float amount = ((!state) ? this.powerConsumptionDeconstruct : this.powerConsumptionConstruct) * Time.deltaTime;
                this.EnergyMixin.ConsumeEnergy(amount);
                bool constructed = c.constructed;
                _ = (!state) ? c.Deconstruct() : c.Construct();
                if (state && !constructed)
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
#if SUBNAUTICA
                    main.SetInteractText(Language.main.Get(constructable.techType), this.deconstructText, false, false, HandReticle.Hand.Left);
#elif BELOWZERO
                    main.SetText(HandReticle.TextType.Hand, this.deconstructText, false);
#endif
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
#if SUBNAUTICA
                    main.SetInteractText(Language.main.Get(constructable.techType), stringBuilder.ToString(), false, false, HandReticle.Hand.Left);
#elif BELOWZERO
                    main.SetText(HandReticle.TextType.Hand, stringBuilder.ToString(), false);
#endif
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
#if SUBNAUTICA
                main.SetInteractText(deconstructable.Name, this.deconstructText);
#elif BELOWZERO
                main.SetText(HandReticle.TextType.Hand, this.deconstructText, false);
#endif
            }
        }

        private void OnDestroy()
        {
            OnDisable();
            ThisVehicle.onToggle -= OnToggle;
            ThisVehicle.modules.onAddItem -= OnAddItem;
            ThisVehicle.modules.onRemoveItem -= OnRemoveItem;
        }
    }
}

