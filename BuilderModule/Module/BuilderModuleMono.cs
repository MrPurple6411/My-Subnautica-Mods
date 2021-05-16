namespace BuilderModule.Module
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class BuilderModuleMono: MonoBehaviour
    {
        public BuilderModuleMono Instance { get; private set; }
        public int ModuleSlotID { get; set; }
        private Vehicle ThisVehicle { get; set; }
        private Player PlayerMain { get; set; }
        private EnergyMixin EnergyMixin { get; set; }

        public bool isToggle;
        public bool isActive;

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
            BuilderTool builderPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Builder").GetComponent<BuilderTool>();
            completeSound = Instantiate(builderPrefab.completeSound, gameObject.transform);
        }

        protected void Start()
        {
            ThisVehicle.onToggle += OnToggle;
            ThisVehicle.modules.onAddItem += OnAddItem;
            ThisVehicle.modules.onRemoveItem += OnRemoveItem;
        }

        private void OnRemoveItem(InventoryItem item)
        {
            if(item.item.GetTechType() == Main.buildermodule.TechType)
            {
                ModuleSlotID = -1;
                Instance.enabled = false;
            }
        }

        private void OnAddItem(InventoryItem item)
        {
            if(item.item.GetTechType() == Main.buildermodule.TechType)
            {
                ModuleSlotID = ThisVehicle.GetSlotByItem(item);
                Instance.enabled = true;
            }
        }

        private void OnToggle(int slotID, bool state)
        {
            if(ThisVehicle.GetSlotBinding(slotID) == Main.buildermodule.TechType)
            {
                isToggle = state;

                if(isToggle)
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

        protected void Update()
        {
            UpdateText();
            if(isActive)
            {
                if(ThisVehicle.GetActiveSlotID() != ModuleSlotID)
                {
                    ThisVehicle.SlotKeyDown(ThisVehicle.GetActiveSlotID());
                    ThisVehicle.SlotKeyUp(ThisVehicle.GetActiveSlotID());
                }
                if(GameInput.GetButtonDown(GameInput.Button.RightHand) && !Player.main.GetPDA().isOpen && !Builder.isPlacing)
                {
                    if(EnergyMixin.charge > 0f)
                    {
                        Player.main.GetPDA().Close();
                        uGUI_BuilderMenu.Show();
                        handleInputFrame = Time.frameCount;
                    }
                }
                if(Builder.isPlacing)
                {
                    if(GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    {
                        if(Builder.TryPlace())
                        {
                            Builder.End();
                        }
                    }
                    else if(handleInputFrame != Time.frameCount && GameInput.GetButtonDown(GameInput.Button.RightHand))
                    {
                        Builder.End();
                    }
                    FPSInputModule.current.EscapeMenu();
                    Builder.Update();
                }
                if(!uGUI_BuilderMenu.IsOpen() && !Builder.isPlacing)
                {
                    HandleInput();
                }
            }
        }

        private void HandleInput()
        {
            if(handleInputFrame == Time.frameCount)
            {
                return;
            }
            handleInputFrame = Time.frameCount;
            if(!AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            bool flag = TryDisplayNoPowerTooltip();
            if(flag)
            {
                return;
            }
            Targeting.AddToIgnoreList(Player.main.gameObject);
            Targeting.GetTarget(60f, out GameObject gameObject, out float num);
            if(gameObject == null)
            {
                return;
            }
            bool buttonHeld = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
            bool buttonDown = GameInput.GetButtonDown(GameInput.Button.Deconstruct);
            bool buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.Deconstruct);
            bool quickbuild = GameInput.GetButtonHeld(GameInput.Button.Sprint);
            Constructable constructable = gameObject.GetComponentInParent<Constructable>();
            if(constructable != null && num > constructable.placeMaxDistance * 2)
            {
                constructable = null;
            }
            if(constructable != null)
            {
                OnHover(constructable);
                if(buttonHeld)
                {
                    Construct(constructable, true);
                    if(quickbuild)
                    {
                        Construct(constructable, true);
                        Construct(constructable, true);
                        Construct(constructable, true);
                    }
                }
                else if(constructable.DeconstructionAllowed(out string text))
                {
                    if(buttonHeld2)
                    {
                        if(constructable.constructed)
                        {
                            constructable.SetState(false, false);
                        }
                        else
                        {
                            Construct(constructable, false);
                            if(quickbuild)
                            {
                                Construct(constructable, false);
                                Construct(constructable, false);
                                Construct(constructable, false);
                            }
                        }
                    }
                }
                else if(buttonDown && !string.IsNullOrEmpty(text))
                {
                    ErrorMessage.AddMessage(text);
                }
            }
            else
            {
                BaseDeconstructable baseDeconstructable = gameObject.GetComponentInParent<BaseDeconstructable>();
                if(baseDeconstructable == null)
                {
                    BaseExplicitFace componentInParent = gameObject.GetComponentInParent<BaseExplicitFace>();
                    if(componentInParent != null)
                    {
                        baseDeconstructable = componentInParent.parent;
                    }
                }
                else
                {
                    if(baseDeconstructable.DeconstructionAllowed(out string text))
                    {
                        OnHover(baseDeconstructable);
                        if(buttonDown)
                        {
                            baseDeconstructable.Deconstruct();
                        }
                    }
                    else if(buttonDown && !string.IsNullOrEmpty(text))
                    {
                        ErrorMessage.AddMessage(text);
                    }
                }
            }
        }

        private bool TryDisplayNoPowerTooltip()
        {
            if(EnergyMixin.charge <= 0f)
            {
                HandReticle main = HandReticle.main;
#if SN1
                main.SetInteractText(noPowerText, false, HandReticle.Hand.None);
#elif BZ
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
            constructText = Language.main.GetFormat<string, string>("ConstructDeconstructFormat", buttonFormat, buttonFormat2);
            deconstructText = buttonFormat2;
            noPowerText = Language.main.Get("NoPower");
        }

        private void Construct(Constructable c, bool state)
        {
            if(c != null && !c.constructed && EnergyMixin.charge > 0f)
            {

                base.StartCoroutine(ConstructAsync(c, state));
            }
        }

        private IEnumerator ConstructAsync(Constructable c, bool state)
        {
            float amount = ((!state) ? powerConsumptionDeconstruct : powerConsumptionConstruct) * Time.deltaTime;
            EnergyMixin.ConsumeEnergy(amount);
            bool constructed = c.constructed;
            bool wasConstructed = c.constructed;
            bool flag;
            if(state)
            {
                flag = c.Construct();
            }
            else
            {
#if SUBNAUTICA_EXP || BZ
                TaskResult<bool> result = new TaskResult<bool>();
#if SUBNAUTICA_EXP
                yield return c.DeconstructAsync(result);
#else
                TaskResult<string> reason = new TaskResult<string>();
                yield return c.DeconstructAsync(result, reason);
#endif
                flag = result.Get();
                result = null;
#elif SUBNAUTICA_STABLE
                flag = c.Deconstruct();
#endif
            }

            if(!flag && state && !wasConstructed)
            {
                Utils.PlayFMODAsset(completeSound, c.transform, 20f);
            }
            yield break;
        }

        private void OnHover(Constructable constructable)
        {
            if(isActive)
            {
                HandReticle main = HandReticle.main;
                if(constructable.constructed)
                {
#if SN1
                    main.SetInteractText(Language.main.Get(constructable.techType), deconstructText, false, false, HandReticle.Hand.Left);
#elif BZ
                    main.SetText(HandReticle.TextType.Hand, this.deconstructText, false);
#endif
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(constructText);
                    foreach(KeyValuePair<TechType, int> keyValuePair in constructable.GetRemainingResources())
                    {
                        TechType key = keyValuePair.Key;
                        string text = Language.main.Get(key);
                        int value = keyValuePair.Value;
                        if(value > 1)
                        {
                            stringBuilder.AppendLine(Language.main.GetFormat<string, int>("RequireMultipleFormat", text, value));
                        }
                        else
                        {
                            stringBuilder.AppendLine(text);
                        }
                    }
#if SN1
                    main.SetInteractText(Language.main.Get(constructable.techType), stringBuilder.ToString(), false, false, HandReticle.Hand.Left);
#elif BZ
                    main.SetText(HandReticle.TextType.Hand, stringBuilder.ToString(), false);
#endif
                    main.SetProgress(constructable.amount);
                    main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                }
            }
        }

        private void OnHover(BaseDeconstructable deconstructable)
        {
            if(isActive)
            {
                HandReticle main = HandReticle.main;
#if SN1
                main.SetInteractText(deconstructable.Name, deconstructText);
#elif BZ
                main.SetText(HandReticle.TextType.Hand, this.deconstructText, false);
#endif
            }
        }

        protected void OnDestroy()
        {
            OnDisable();
            ThisVehicle.onToggle -= OnToggle;
            ThisVehicle.modules.onAddItem -= OnAddItem;
            ThisVehicle.modules.onRemoveItem -= OnRemoveItem;
        }
    }
}