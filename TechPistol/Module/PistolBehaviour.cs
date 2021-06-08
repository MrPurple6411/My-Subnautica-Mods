namespace TechPistol.Module
{
    using System.Linq;
    using UnityEngine;
    using UWE;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(EnergyMixin))]
    internal class PistolBehaviour: PlayerTool, IProtoEventListener
    {
        private FMODAsset repulsionCannonFireSound;
        private FMODAsset stasisRifleFireSound;
        private FMOD_StudioEventEmitter stasisRifleEvent;
#if SN1
        private FMODASRPlayer laserShootSound;
#elif BZ
		private FMOD_CustomEmitter laserShootSound;
#endif
        private FMODAsset modeChangeSound;
        public ParticleSystem[] par = new ParticleSystem[7];
        public LineRenderer[] Line = new LineRenderer[3];
        public GameObject LaserParticles;
        [SerializeField] 
        private TextMesh textName;
        [SerializeField] 
        private TextMesh textHealth;
        [SerializeField] 
        private TextMesh textMode;
        [SerializeField] 
        private Rigidbody rigidbody;
        
#if !EDITOR
        private ParticleSystem scaleParticleSystem;
        private ParticleSystem laserEndParticleSystem;

        private bool CannonCharging;
        private bool LaserFiring;
        private bool ScaleBig;
        private bool ScaleSmall;
        private float Charge;
        private int mode;

        private float currentDamage;
        internal static float lastShotDamage;


        private bool PowerCheck => energyMixin.charge > 0f || !GameModeUtils.RequiresPower();

        public const string GunMain = "HandGun/GunMain";
        public const string Point = GunMain + "/GunCylinder/Point";
        public const string CannonMode = Point + "/CannonMode";
        public const string LaserMode = Point + "/LaserMode";
        public const string ScaleMode = Point + "/ScaleMode";

        private Vector3 CurrentMuzzlePosition => gameObject.transform.Find(Point).position;

        public override string animToolName => TechType.Scanner.AsString(true);

        private void TargetLaser(float range, LineRenderer lineRenderer)
        {
            var transform1 = Player.main.camRoot.mainCam.transform;
            var forward = transform1.forward;
            var position = transform1.position;

            if(Targeting.GetTarget(Player.main.gameObject, range, out _, out var num))
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
                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.RepulsionCannon), out var RCFilename))
                {
                    var gameObject1 = Resources.Load<GameObject>(RCFilename);
                    var component = gameObject1.GetComponent<RepulsionCannon>();
                    repulsionCannonFireSound = component.shootSound;
                    gameObject1.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.StasisRifle), out var SRFilename))
                {
                    var gameObject2 = Resources.Load<GameObject>(SRFilename);
                    var component2 = gameObject2.GetComponent<StasisRifle>();
                    stasisRifleFireSound = component2.fireSound;
                    stasisRifleEvent = component2.chargeBegin;
                    gameObject2.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.PropulsionCannon), out var PCFilename))
                {
                    var gameObject3 = Resources.Load<GameObject>(PCFilename);
                    var component3 = gameObject3.GetComponent<PropulsionCannon>();
                    modeChangeSound = component3.shootSound;
                    gameObject3.SetActive(false);
                }

                if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Welder), out var WFilename))
                {
                    var gameObject4 = Resources.Load<GameObject>(WFilename);
                    var component4 = gameObject4.GetComponent<Welder>();
                    laserShootSound = component4.weldSound;
                    gameObject4.SetActive(false);
                }

                Transform transform1;
                laserEndParticleSystem = LaserParticles.transform.Find("Laserend").gameObject.GetComponent<ParticleSystem>();
                scaleParticleSystem = LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>();
            }
            else
            {
                rigidbody.detectCollisions = true;
            }
        }
#elif !EDITOR
		protected void Start()
		{
#pragma warning disable CS0612 // Type or member is obsolete
            if (repulsionCannonFireSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.RepulsionCannon), out var RCFilename))
            {
                AddressablesUtility.LoadAsync<GameObject>(RCFilename).Completed += (x) =>
                {
                    var gameObject1 = x.Result;
                    var component = gameObject1?.GetComponent<RepulsionCannon>();
                    repulsionCannonFireSound = component?.shootSound;
                };
            }

            if ((stasisRifleFireSound is null || stasisRifleEvent is null) && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.StasisRifle), out var SRFilename))
            {
                AddressablesUtility.LoadAsync<GameObject>(SRFilename).Completed += (x) =>
                {
                    var gameObject2 = x.Result;
                    var component2 = gameObject2?.GetComponent<StasisRifle>();
                    stasisRifleFireSound = component2?.fireSound;
                    stasisRifleEvent = component2?.chargeBegin;
                };
            }

            if (modeChangeSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.PropulsionCannon), out var PCFilename))
            {
                AddressablesUtility.LoadAsync<GameObject>(PCFilename).Completed += (x) =>
                {
                    var gameObject3 = x.Result;
                    var component3 = gameObject3?.GetComponent<PropulsionCannon>();
                    modeChangeSound = component3?.shootSound;
                };
            }

            if (laserShootSound is null && PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Welder), out var WFilename))
            {
                AddressablesUtility.LoadAsync<GameObject>(WFilename).Completed += (x) =>
                {
                    var gameObject4 = x.Result;
                    var component4 = gameObject4?.GetComponent<Welder>();
                    laserShootSound = component4?.weldSound;
                };
            }
#pragma warning restore CS0612 // Type or member is obsolete

                laserEndParticleSystem = LaserParticles.transform.Find("Laserend").gameObject.GetComponent<ParticleSystem>();
                scaleParticleSystem = LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>();
		}
#endif

        public override bool OnAltDown()
        {
            if(PowerCheck)
            {
                par[0].Play();
                Reset();
                mode++;
                FMODUWE.PlayOneShot(modeChangeSound, transform.position, 0.5f);

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
            if (!isDrawn || !PowerCheck) return;
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

        protected void LateUpdate()
        {
            if (!isDrawn) return;
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

        /// <summary>
        /// Handles the code for scaling up and down targets when using Scale Mode.
        /// </summary>
        private void ResizeMode()
        {
            try
            {
                if(Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out var go, out _))
                {
                    var entityRoot = Utils.GetEntityRoot(go) ?? go;

                    // Handles the Target display on the top of the gun.
                    var techType = CraftData.GetTechType(entityRoot);
                    var techTypeString = techType != TechType.None ? techType.AsString() : "";
                    var translatedName = techTypeString != "" ? Language.main.GetOrFallback(techTypeString, techTypeString) + " size" : "";
                    var scale = translatedName != ""
                        ? System.Math.Round(entityRoot.transform.localScale.x,
                                2)
                            .ToString()
                        : "";

                    textName.text = translatedName;
                    textHealth.text = scale;

                    if(GameInput.GetButtonDown(GameInput.Button.Deconstruct))
                    {
                        entityRoot.transform.localScale = Vector3.one;
                    }
                    else if(GameInput.GetButtonHeld(GameInput.Button.RightHand) && ScaleBig && (energyMixin.ConsumeEnergy(1f * Main.Config.ScaleUpspeed) || !GameModeUtils.RequiresPower()))
                    {
                        par[5].gameObject.transform.Rotate(Vector3.forward * 5f);
                        var changeSpeed = Main.Config.ScaleUpspeed;
                        var oldScale = entityRoot.transform.localScale;
                        var newScale = new Vector3(oldScale.x + changeSpeed, oldScale.y + changeSpeed, oldScale.z + changeSpeed);

                        entityRoot.transform.localScale = newScale;

                        if (!Main.Config.LethalResizing || (!(newScale.x >= Main.Config.ScaleKillSize) &&
                                                            !(newScale.y >= Main.Config.ScaleKillSize) &&
                                                            !(newScale.z >= Main.Config.ScaleKillSize))) return;
                        entityRoot.GetComponentInChildren<LiveMixin>()?.Kill();
                        entityRoot.GetComponentInChildren<BreakableResource>()?.HitResource();
                        var drillable = entityRoot.GetComponent<Drillable>();

                        if (drillable is null) return;
                        while(drillable.health.Sum() > 0)
                        {
                            drillable.OnDrill(entityRoot.transform.position, null, out var _);
                        }
                    }
                    else if (GameInput.GetButtonHeld(GameInput.Button.RightHand) &&
                             ScaleSmall &&
                             (energyMixin.ConsumeEnergy(1f * Main.Config.ScaleDownspeed) ||
                              !GameModeUtils.RequiresPower()))
                    {
                        par[6].gameObject.transform.Rotate(-Vector3.forward * 5f);

                        var changeSpeed = Main.Config.ScaleDownspeed;
                        var oldScale = entityRoot.transform.localScale;
                        var newScale = new Vector3(oldScale.x - changeSpeed, oldScale.y - changeSpeed, oldScale.z - changeSpeed);

                        entityRoot.transform.localScale = newScale.x > changeSpeed ? newScale : new Vector3(0.01f, 0.01f, 0.01f);
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
            if(Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out var gameObject4, out _) && Utils.GetEntityRoot(gameObject4) != null)
            {
                if(Utils.GetEntityRoot(gameObject4).TryGetComponent<LiveMixin>(out var liveMixin))
                {
                    var techName = CraftData.GetTechType(liveMixin.gameObject).AsString();
                    var translatedName = Language.main.GetOrFallback(techName, techName);

                    if(translatedName.ToLower().Contains("school") && liveMixin.health == 0)
                    {
                        Destroy(liveMixin.gameObject);
                    }

                    var health = liveMixin.health.ToString();

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
                if (!Targeting.GetTarget(Player.main.gameObject, Main.Config.TargetingRange, out var go,
                    out _)) return;
                var entityRoot = Utils.GetEntityRoot(go) ?? go;
                entityRoot?.GetComponentInChildren<LiveMixin>()?.TakeDamage(Main.Config.LaserDamage, go.transform.position, DamageType.Heat);
                entityRoot?.GetComponentInChildren<BreakableResource>()?.HitResource();
                entityRoot?.GetComponentInChildren<Drillable>()?.OnDrill(entityRoot.transform.position, null, out var _);
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
                    var position = transform.position;
                    FMODUWE.PlayOneShot(repulsionCannonFireSound, position);
                    FMODUWE.PlayOneShot(stasisRifleFireSound, position);
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
            if (!PowerCheck) return true;
            switch(mode)
            {
                case 1:
                    laserEndParticleSystem.Play();
                    FMODUWE.PlayOneShot(repulsionCannonFireSound, transform.position);
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
                    scaleParticleSystem.Play();
                    FMODUWE.PlayOneShot(repulsionCannonFireSound, transform.position);
                    par[5].Play();
                    ScaleBig = true;
                    break;
                case 4:
                    scaleParticleSystem.Play();
                    FMODUWE.PlayOneShot(repulsionCannonFireSound, transform.position);
                    par[6].Play();
                    ScaleSmall = true;
                    break;
            }
            return true;
        }

        public void Reset()
        {
            if(LaserFiring || CannonCharging || ScaleBig || ScaleSmall)
            {
                scaleParticleSystem.Stop();
                laserEndParticleSystem.Stop();

                laserShootSound?.Stop();
                stasisRifleEvent?.Stop();
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
#endif
        
        public void OnProtoSerialize(ProtobufSerializer serializer)
        {

        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {

        }
    }
}