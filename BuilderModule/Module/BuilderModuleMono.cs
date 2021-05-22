namespace BuilderModule.Module
{
    using BuilderModule.Patches;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UWE;

    public class BuilderModuleMono: MonoBehaviour
    {
        public int ModuleSlotID;

        public Vehicle vehicle;
#if BZ
        public SeaTruckUpgrades seaTruck;
        public SeaTruckLights lights;
        public Hoverbike hoverbike;
#endif
        public EnergyMixin energyMixin;
        public EnergyInterface energyInterface;
        public PowerRelay powerRelay;

        public bool isToggle;

        public float powerConsumptionConstruct = 0.5f;
        public float powerConsumptionDeconstruct = 0.5f;
        private static FMODAsset completeSound;
        private int handleInputFrame = -1;
        private string deconstructText;
        private string constructText;
        private string noPowerText;

        public void Awake()
        {
            if(completeSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Builder), out string BuilderFilename))
            {
#if SUBNAUTICA_STABLE
                BuilderTool builderPrefab = Resources.Load<GameObject>(BuilderFilename).GetComponent<BuilderTool>();
                completeSound = Instantiate(builderPrefab.completeSound, gameObject.transform);
#else
                AddressablesUtility.LoadAsync<GameObject>(BuilderFilename).Completed += (x) =>
                {
                    GameObject gameObject1 = x.Result;
                    BuilderTool builderPrefab = gameObject1?.GetComponent<BuilderTool>();
                    completeSound = Instantiate(builderPrefab.completeSound, gameObject.transform);
                };
#endif
            }
        }

        public void OnToggle(int slotID, bool state)
        {
            TechType techType = TechType.None;

            if(vehicle != null)
                techType = vehicle.GetSlotBinding(slotID);

            if(Main.builderModules.Contains(techType))
            {
                isToggle = state;
                if(!isToggle)
                    OnDisable();
            }
        }

        public void Toggle()
        {
                isToggle = !isToggle;
            if(isToggle)
            {
                if(energyMixin?.charge > 0f || powerRelay?.GetPower() > 0f || energyInterface?.TotalCanProvide(out _) > 0f)
                {
                    ErrorMessage.AddMessage($"BuilderModule Enabled");
                    Player.main.GetPDA().Close();
                    uGUI_BuilderMenu.Show();
                    handleInputFrame = Time.frameCount;
                    return;
                }
                else
                {
                    ErrorMessage.AddMessage($"Insufficient Power");
                    Toggle();
                    return;
                }
            }

            ErrorMessage.AddMessage($"BuilderModule Disabled");
            OnDisable();

        }

        public void OnDisable()
        {
            if(uGUI_BuilderMenu.IsOpen())
            uGUI_BuilderMenu.Hide();
            if(Builder.prefab != null)
            Builder.End();
        }


        protected void Update()
        {
            if(isToggle && !(Player.main.GetVehicle() != null
#if BZ
                || Player.main.IsPilotingSeatruck() || Player.main.inHovercraft
#endif
                ))
            {
                Toggle();
                return;
            }

#if BZ
            if(isToggle && lights != null && powerRelay.IsPowered())
            {
                lights.lightsActive = true;
                lights.floodLight.SetActive(true);
            }
#endif



            if(VehicleCheck())
            {
                if(GameInput.GetButtonDown(GameInput.Button.Reload))
                {
                    Toggle();
                    return;
                }
            }


            UpdateText();
            if(isToggle)
            {
                if(GameInput.GetButtonDown(GameInput.Button.RightHand) && !Player.main.GetPDA().isOpen)
                {
                    if(Builder.isPlacing)
                        Builder.End();

                    if(energyMixin?.charge > 0f || powerRelay?.GetPower() > 0f || energyInterface?.TotalCanProvide(out _) > 0f)
                    {
                        Player.main.GetPDA().Close();
                        uGUI_BuilderMenu.Show();
                        handleInputFrame = Time.frameCount;
                    }
                    return;
                }
                if(Builder.isPlacing)
                {
                    if(GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    {
                        if(Builder.TryPlace())
                            Builder.End();
                        return;
                    }
                    else if(handleInputFrame != Time.frameCount && GameInput.GetButtonDown(GameInput.Button.Exit))
                    {
                        Builder.End();
                        return;
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

        private bool VehicleCheck()
        {
            if(vehicle is not null && vehicle == Player.main.GetVehicle())
            {
                return true;
            }
#if BZ
            else if(hoverbike is not null && hoverbike == Player.main.GetComponentInParent<Hoverbike>())
            {
                return true;
            }
            else if(seaTruck is not null && seaTruck == Player.main.GetComponentInParent<SeaTruckUpgrades>())
            {
                return true;
            }
#endif
            return false;
        }

        private void HandleInput()
        {
            if(handleInputFrame == Time.frameCount)
                return;

            handleInputFrame = Time.frameCount;

            if(!AvatarInputHandler.main.IsEnabled() || TryDisplayNoPowerTooltip())
                return;

            Targeting.AddToIgnoreList(Player.main.gameObject);
            GameObject gameObject = null;
            float num = 9000;
            if(vehicle != null)
                Targeting.GetTarget(vehicle.gameObject, 60f, out gameObject, out num);
#if BZ
            if(seaTruck != null)
                Targeting.GetTarget(seaTruck.gameObject, 60f, out gameObject, out num);
            if(hoverbike != null)
                Targeting.GetTarget(hoverbike.gameObject, 60f, out gameObject, out num);
#endif

            if(gameObject is null)
                return;

            bool buttonHeld = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
            bool buttonDown = GameInput.GetButtonDown(GameInput.Button.Deconstruct);
            bool buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.Deconstruct);
            Constructable constructable = gameObject.GetComponentInParent<Constructable>();
            if(constructable != null && num > constructable.placeMaxDistance * 2)
                constructable = null;

            if(constructable != null)
            {
                OnHover(constructable);
                if(buttonHeld)
                {
                    Construct(constructable, true);
                    Construct(constructable, true);
                    Construct(constructable, true);
                    return;
                }
                else if(constructable.DeconstructionAllowed(out string text))
                {
                    if(buttonHeld2)
                    {
                        if(!constructable.constructed)
                        {
                            Construct(constructable, false);
                            Construct(constructable, false);
                            Construct(constructable, false);
                            return;
                        }

                        constructable.SetState(false, false);
                        return;
                    }
                }
                else if(buttonDown && !string.IsNullOrEmpty(text))
                {
                    ErrorMessage.AddMessage(text);
                    return;
                }
            }
            else
            {
                BaseDeconstructable baseDeconstructable = gameObject.GetComponentInParent<BaseDeconstructable>();
                if(baseDeconstructable is null)
                    baseDeconstructable = gameObject.GetComponentInParent<BaseExplicitFace>()?.parent;

                if(baseDeconstructable is not null)
                {
                    if(!baseDeconstructable.DeconstructionAllowed(out string text))
                    {
                        if(buttonDown && !string.IsNullOrEmpty(text))
                            ErrorMessage.AddMessage(text);
                        return;
                    }

                    OnHover(baseDeconstructable);
                    if(buttonDown)
                        baseDeconstructable.Deconstruct();
                    return;
                }
            }
        }

        private bool TryDisplayNoPowerTooltip()
        {
            if((energyInterface?.TotalCanProvide(out _) ?? 0f) <= 0f && (powerRelay?.GetPower()??0f) <= 0f && (energyMixin?.charge ?? 0f) <= 0f)
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
            if(c != null && !c.constructed && ((energyInterface?.TotalCanProvide(out _) ?? 0f) > 0f || (powerRelay?.GetPower() ?? 0f) > 0f || (energyMixin?.charge ?? 0f) > 0f))
                base.StartCoroutine(ConstructAsync(c, state));
        }

        private IEnumerator ConstructAsync(Constructable c, bool state)
        {
            float amount = ((!state) ? powerConsumptionDeconstruct : powerConsumptionConstruct) * Time.deltaTime;
            float consumed = energyInterface?.ConsumeEnergy(amount) ?? 0f;
            bool energyMixinConsumed = energyMixin?.ConsumeEnergy(amount) ?? false;
            if(energyMixinConsumed || consumed > 0f || (powerRelay?.ConsumeEnergy(amount, out consumed) ?? false))
            {

                if(!energyMixinConsumed && consumed < amount)
                {
                    if(energyInterface is not null)
                        energyInterface.AddEnergy(consumed);
                    else if(powerRelay is not null)
                        powerRelay.AddEnergy(consumed, out _);
                    yield break;
                }

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
                    FMODUWE.PlayOneShot(completeSound, c.transform.position, 20f);
            }
            
            yield break;
        }

        private void OnHover(Constructable constructable)
        {
            if(isToggle)
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
                            stringBuilder.AppendLine(Language.main.GetFormat<string, int>("RequireMultipleFormat", text, value));
                        else
                            stringBuilder.AppendLine(text);
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
            if(isToggle)
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
            if(vehicle is not null)
                vehicle.onToggle -= OnToggle;
        }
    }
}