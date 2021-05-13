namespace TechPistol.Module
{
    using SMLHelper.V2.Utility;
#if !SUBNAUTICA_STABLE
    using System.Collections;
#endif
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UWE;

    [RequireComponent(typeof(EnergyMixin))]
    internal class PistolBehaviour: PlayerTool, IProtoEventListener
    {
        public FMODAsset repulsionCannonFireSound;
        public FMODAsset stasisRifleFireSound;
        public FMOD_StudioEventEmitter stasisRifleEvent;
#if SN1
        public FMODASRPlayer laserShootSound;
#elif BZ
		public FMOD_CustomEmitter laserShootSound;
#endif
        public FMODAsset modeChangeSound;
        public ParticleSystem[] par = new ParticleSystem[7];
        public LineRenderer[] Line = new LineRenderer[3];
        public GameObject LaserParticles;
        public bool CannonCharging = false;
        public bool LaserFiring = false;
        public bool ScaleBig = false;
        public bool ScaleSmall = false;
        public float Charge = 0f;
        public int mode = 0;

        [SerializeField]
        private TextMesh textName = default;

        [SerializeField]
        private TextMesh textHealth = default;

        [SerializeField]
        private TextMesh textMode = default;

        [SerializeField]
        private Rigidbody rigidbody = default;

        public float currentDamage = 0;
        public static float lastShotDamage = 0;

        private bool PowerCheck => energyMixin.charge > 0f || !GameModeUtils.RequiresPower();

        public const string GunMain = "HandGun/GunMain";
        public const string Point = GunMain + "/GunCylinder/Point";
        public const string CannonMode = Point + "/CannonMode";
        public const string LaserMode = Point + "/LaserMode";
        public const string ScaleMode = Point + "/ScaleMode";

        public Vector3 CurrentMuzzlePosition => base.gameObject.transform.Find(Point).position;

        public override string animToolName => TechType.Scanner.AsString(true);

        private void TargetLaser(float range, LineRenderer lineRenderer)
        {
            Vector3 forward = Player.main.camRoot.mainCam.transform.forward;
            Vector3 position = Player.main.camRoot.mainCam.transform.position;

            if(Targeting.GetTarget(Player.main.gameObject, range, out GameObject gameObject, out float num))
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, CurrentMuzzlePosition);
                lineRenderer.SetPosition(1, (forward * num) + position);
                LaserParticles.transform.position = (forward * num) + position;
            }
            else
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, CurrentMuzzlePosition);
                lineRenderer.SetPosition(1, (forward * range) + position);
                LaserParticles.transform.position = (forward * range) + position;
            }
        }


        public override void OnDraw(Player p)
        {
            rigidbody.detectCollisions = false;
            base.OnDraw(p);
        }

        public override void OnHolster()
        {
            rigidbody.detectCollisions = true;
            Reset();
            base.OnHolster();
        }

        public override void Awake()
        {
            rigidbody.detectCollisions = false;
            base.Awake();
        }

#if SUBNAUTICA_STABLE
        protected void Start()
        {
            if(repulsionCannonFireSound is null)
            {
                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.RepulsionCannon), out string RCFilename))
                {
                    GameObject gameObject1 = Resources.Load<GameObject>(RCFilename);
                    RepulsionCannon component = gameObject1.GetComponent<RepulsionCannon>();
                    repulsionCannonFireSound = component.shootSound;
                    gameObject1.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.StasisRifle), out string SRFilename))
                {
                    GameObject gameObject2 = Resources.Load<GameObject>(SRFilename);
                    StasisRifle component2 = gameObject2.GetComponent<StasisRifle>();
                    stasisRifleFireSound = component2.fireSound;
                    stasisRifleEvent = component2.chargeBegin;
                    gameObject2.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.PropulsionCannon), out string PCFilename))
                {
                    GameObject gameObject3 = Resources.Load<GameObject>(PCFilename);
                    PropulsionCannon component3 = gameObject3.GetComponent<PropulsionCannon>();
                    modeChangeSound = component3.shootSound;
                    gameObject3.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Welder), out string WFilename))
                {
                    GameObject gameObject4 = Resources.Load<GameObject>(WFilename);
                    Welder component4 = gameObject4.GetComponent<Welder>();
                    laserShootSound = component4.weldSound;
                    gameObject4.SetActive(false);
                }

                LaserParticles = GameObject.Instantiate<GameObject>(Main.assetBundle.LoadAsset<GameObject>("LaserParticles.prefab"), base.transform.position, base.transform.rotation);
            }
            else
            {
                rigidbody.detectCollisions = true;
            }
        }
#else
		protected void Start()
		{
#pragma warning disable CS0612 // Type or member is obsolete
            if (repulsionCannonFireSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.RepulsionCannon), out string RCFilename))
           {
				AddressablesUtility.LoadAsync<GameObject>(RCFilename).Completed += (x) =>
				{
					GameObject gameObject1 = x.Result;
					RepulsionCannon component = gameObject1?.GetComponent<RepulsionCannon>();
					repulsionCannonFireSound = component?.shootSound;
				};
			}

			if ((stasisRifleFireSound is null || stasisRifleEvent is null) && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.StasisRifle), out string SRFilename))
			{
				AddressablesUtility.LoadAsync<GameObject>(SRFilename).Completed += (x) =>
				{
					GameObject gameObject2 = x.Result;
					StasisRifle component2 = gameObject2?.GetComponent<StasisRifle>();
					stasisRifleFireSound = component2?.fireSound;
					stasisRifleEvent = component2?.chargeBegin;
				};
			}

			if (modeChangeSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.PropulsionCannon), out string PCFilename))
			{
				AddressablesUtility.LoadAsync<GameObject>(PCFilename).Completed += (x) =>
				{
					GameObject gameObject3 = x.Result;
					PropulsionCannon component3 = gameObject3?.GetComponent<PropulsionCannon>();
					modeChangeSound = component3?.shootSound;
				};
			}

			if (laserShootSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Welder), out string WFilename))
			{
				AddressablesUtility.LoadAsync<GameObject>(WFilename).Completed += (x) =>
				{
					GameObject gameObject4 = x.Result;
					Welder component4 = gameObject4?.GetComponent<Welder>();
					laserShootSound = component4?.weldSound;
				};
			}
#pragma warning restore CS0612 // Type or member is obsolete

            if(LaserParticles is null)
			{
				LaserParticles = GameObject.Instantiate<GameObject>(Main.assetBundle.LoadAsset<GameObject>("LaserParticles.prefab"), base.transform.position, base.transform.rotation);
			}
            else
            {
				rigidbody.detectCollisions = true;
			}
		}
#endif

        public override bool OnAltDown()
        {
            if(PowerCheck)
            {
                par[0].Play();
                Reset();
                mode++;
                FMODUWE.PlayOneShot(modeChangeSound, base.transform.position, 0.5f);

                switch(mode)
                {
                    case 1:
                        textMode.text = "Laser";
                        break;
                    case 2:
                        textMode.text = "Cannon";
                        break;
                    case 3:
                        textMode.text = "Big";
                        break;
                    case 4:
                        textMode.text = "Small";
                        break;
                    case 5:
                        textMode.text = "Standby";
                        mode = 0;
                        break;
                }
            }
            else
            {
                textMode.text = "No Power";
                mode = 0;
            }
            return true;
        }

        protected void Update()
        {
            if(base.isDrawn && PowerCheck)
            {
                if(LaserFiring)
                {
                    TargetLaser(Main.Config.TargetingRange, Line[0]);
                }
                else if(ScaleBig)
                {
                    TargetLaser(Main.Config.TargetingRange, Line[1]);
                }
                else if(ScaleSmall)
                {
                    TargetLaser(Main.Config.TargetingRange, Line[2]);
                }
            }
        }

        protected void LateUpdate()
        {
            if(base.isDrawn)
            {
                if(PowerCheck)
                {
                    switch(mode)
                    {
                        case 0:
                            textMode.text = "Standby";
                            break;
                        case 1:
                        case 2:
                            HarmMode();
                            break;
                        case 3:
                        case 4:
                            ResizeMode();
                            break;
                    }


                }
                else
                {
                    textMode.text = "No Power";
                    mode = 0;
                    Reset();
                }
            }
        }

        /// <summary>
        /// Handles the code for scaling up and down targets when using Scale Mode.
        /// </summary>
        private void ResizeMode()
        {
            try
            {
                if(Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out GameObject gameObject, out float num2))
                {
                    GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject) ?? gameObject;

                    // Handles the Target display on the top of the gun.
                    TechType techType = CraftData.GetTechType(entityRoot);
                    string name = techType != TechType.None ? techType.AsString() : "";
                    string translatedName = name != "" ? Language.main.GetOrFallback(name, name) + " size" : "";
                    string scale = translatedName != "" ? System.Math.Round(entityRoot.transform.localScale.x, 2).ToString() : "";

                    textName.text = translatedName;
                    textHealth.text = scale;

                    if(GameInput.GetButtonDown(GameInput.Button.Deconstruct))
                    {
                        entityRoot.transform.localScale = Vector3.one;
                        return;
                    }
                    else if(GameInput.GetButtonHeld(GameInput.Button.RightHand) && ScaleBig && (energyMixin.ConsumeEnergy(1f * Main.Config.ScaleUpspeed) || !GameModeUtils.RequiresPower()))
                    {
                        par[5].gameObject.transform.Rotate(Vector3.forward * 5f);
                        float changespeed = Main.Config.ScaleUpspeed;
                        Vector3 oldScale = entityRoot.transform.localScale;
                        Vector3 newScale = new Vector3(oldScale.x + changespeed, oldScale.y + changespeed, oldScale.z + changespeed);

                        entityRoot.transform.localScale = newScale;

                        if(Main.Config.LethalResizing && (newScale.x >= Main.Config.ScaleKillSize || newScale.y >= Main.Config.ScaleKillSize || newScale.z >= Main.Config.ScaleKillSize))
                        {
                            entityRoot.GetComponentInChildren<LiveMixin>()?.Kill(DamageType.Normal);
                            entityRoot.GetComponentInChildren<BreakableResource>()?.HitResource();
                            Drillable drillable = entityRoot?.GetComponent<Drillable>();

                            if(drillable != null)
                            {
                                while(drillable.health.Sum() > 0)
                                {
                                    drillable?.OnDrill(entityRoot.transform.position, null, out GameObject _);
                                }
                            }
                        }
                    }
                    else if(GameInput.GetButtonHeld(GameInput.Button.RightHand) && ScaleSmall && (energyMixin.ConsumeEnergy(1f * Main.Config.ScaleDownspeed) || !GameModeUtils.RequiresPower()))
                    {
                        par[6].gameObject.transform.Rotate(-Vector3.forward * 5f);

                        float changespeed = Main.Config.ScaleDownspeed;
                        Vector3 oldScale = entityRoot.transform.localScale;
                        Vector3 newScale = new Vector3(oldScale.x - changespeed, oldScale.y - changespeed, oldScale.z - changespeed);

                        entityRoot.transform.localScale = newScale.x > changespeed ? newScale : new Vector3(0.01f, 0.01f, 0.01f);
                    }

                }
                else
                {
                    textName.text = "";
                    textHealth.text = "";
                }
            }
            catch
            {
                textName.text = "";
                textHealth.text = "";
            }
        }

        /// <summary>
        /// Handles the Cannon and Laser functions of the Harm Mode.
        /// </summary>
        private void HarmMode()
        {
            // Handles the Target display on the top of the gun.
            if(Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out GameObject gameObject4, out float num4) && UWE.Utils.GetEntityRoot(gameObject4) != null)
            {
                if(UWE.Utils.GetEntityRoot(gameObject4).TryGetComponent<LiveMixin>(out LiveMixin liveMixin))
                {
                    string name = CraftData.GetTechType(liveMixin.gameObject).AsString();
                    string translatedName = Language.main.GetOrFallback(name, name);

                    if(translatedName.ToLower().Contains("school") && liveMixin.health == 0)
                    {
                        GameObject.Destroy(liveMixin.gameObject);
                    }

                    string health = liveMixin.health.ToString() ?? "";

                    textName.text = translatedName;
                    textHealth.text = health;
                }
                else
                {
                    textName.text = "";
                    textHealth.text = "";
                }
            }
            else
            {
                textName.text = "";
                textHealth.text = "";
            }

            if(GameInput.GetButtonHeld(GameInput.Button.RightHand) && CannonCharging)
            {
                Charge += Time.deltaTime * Main.Config.CannonChargeSpeed;
                currentDamage = Main.Config.CannonDamage / Main.Config.CannonExplosionSize * Charge;
                textName.text = "Cannon Fire Cost";
                textHealth.text = $"{System.Math.Round(Charge * 10, 2)}";
                par[3].transform.rotation = Player.main.camRoot.mainCam.transform.rotation;
            }
            else if(!GameInput.GetButtonHeld(GameInput.Button.RightHand) && CannonCharging)
            {
                OnRightHandUp();
            }
            else if(GameInput.GetButtonHeld(GameInput.Button.RightHand) && LaserFiring && (energyMixin.ConsumeEnergy(Main.Config.LaserDamage * Time.deltaTime) || !GameModeUtils.RequiresPower()))
            {
                par[4].gameObject.transform.Rotate(Vector3.forward * 5f);
                if(Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out GameObject gameObject, out float num))
                {
                    GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject);
                    entityRoot?.GetComponentInChildren<LiveMixin>()?.TakeDamage(Main.Config.LaserDamage, gameObject.transform.position, DamageType.Heat, null);
                    entityRoot?.GetComponentInChildren<BreakableResource>()?.HitResource();
                    entityRoot?.GetComponentInChildren<Drillable>()?.OnDrill(entityRoot.transform.position, null, out GameObject _);
                }
            }
        }

        public override bool OnRightHandUp()
        {
            if(CannonCharging)
            {
                lastShotDamage = (int)currentDamage;
                currentDamage = 0;

                if((System.Math.Round(Charge * 10, 2) >= 20f && energyMixin.ConsumeEnergy(Charge * 10)) || !GameModeUtils.RequiresPower())
                {
                    Charge = 0;
                    FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
                    FMODUWE.PlayOneShot(stasisRifleFireSound, base.transform.position, 1f);
                    par[1].Stop();
                    par[1].Clear();
                    par[2].Stop();
                    par[3].Play();
                    CannonCharging = false;
                }
                else if(System.Math.Round(Charge * 10, 2) < 20f)
                {
                    ErrorMessage.AddMessage("Cannon Ball must charge to at least 20 power.");
                    textName.text = "";
                    textHealth.text = "";
                }
                else
                {
                    ErrorMessage.AddMessage("Insufficient Power in battery to Launch Cannon Ball");
                    textName.text = "";
                    textHealth.text = "";
                }
            }

            Reset();
            return true;
        }

        public override bool OnRightHandDown()
        {
            if(PowerCheck)
            {
                switch(mode)
                {
                    case 1:
                        LaserParticles.transform.Find("Laserend").GetComponent<ParticleSystem>().Play();
                        FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
                        laserShootSound.Play();
                        par[4].Play();
                        LaserFiring = true;
                        break;
                    case 2:
                        CannonCharging = true;
                        par[1].Play();
                        par[2].Play();
                        Charge = 0f;
                        stasisRifleEvent.StartEvent();
                        break;
                    case 3:
                        LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>().Play();
                        FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
                        par[5].Play();
                        ScaleBig = true;
                        break;
                    case 4:
                        LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>().Play();
                        FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
                        par[6].Play();
                        ScaleSmall = true;
                        break;
                }
            }
            return true;
        }

        public void Reset()
        {
            if(LaserFiring || CannonCharging || ScaleBig || ScaleSmall)
            {
                LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>().Stop();
                LaserParticles.transform.Find("Laserend").gameObject.GetComponent<ParticleSystem>().Stop();

                laserShootSound?.Stop();
                stasisRifleEvent?.Stop(true);
                LaserFiring = false;
                CannonCharging = false;
                ScaleBig = false;
                ScaleSmall = false;

                par.ForEach((x) =>
                {
                    if(x.isPlaying)
                        x.Stop();
                });

                Line.ForEach((x) =>
                {
                    x.SetPosition(0, new Vector3(0f, 0f, 0f));
                    x.SetPosition(1, new Vector3(0f, 0f, 0f));
                    x.enabled = false;
                });
            }
            textName.text = "";
            textHealth.text = "";
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {

        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {

        }
    }
}