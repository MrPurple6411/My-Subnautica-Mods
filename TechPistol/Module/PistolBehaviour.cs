using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SMLHelper.V2.Utility;
using UnityEngine;
using UWE;

namespace TechPistol.Module
{
	[RequireComponent(typeof(EnergyMixin))]
	class PistolBehaviour : PlayerTool, IProtoEventListener
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
		public ParticleSystem[] par = new ParticleSystem[10];
		public LineRenderer[] Line = new LineRenderer[10];
		public GameObject LaserParticles;
		public bool CannonCharging = false;
		public bool LaserFiring = false;
		public bool ScaleBig = false;
		public bool ScaleSmall = false;
		public float time;
		public float time2;
		public int mode;
		public TextMesh textname;
		public TextMesh textblood;
		public TextMesh textmode;

		public const string GunMain = "HandGun/GunMain";
		public const string ModeChange = GunMain + "/ModeChange";
		public const string Point = GunMain + "/GunCylinder/Point";
		public const string CannonMode = Point + "/CannonMode";
		public const string LaserMode = Point + "/LaserMode";
		public const string ScaleMode = Point + "/ScaleMode";

		public Vector3 currentMuzzlePosition => base.gameObject.transform.Find(Point).position;

		public override bool DoesOverrideHand() => true;

		public override string animToolName => TechType.Scanner.AsString(true);

		private void TargetLaser(float range, LineRenderer lineder)
		{
#if SN1
			Vector3 forward = Player.main.camRoot.mainCamera.transform.forward;
			Vector3 position = Player.main.camRoot.mainCamera.transform.position;
#elif BZ
			Vector3 forward = Player.main.camRoot.mainCam.transform.forward;
			Vector3 position = Player.main.camRoot.mainCam.transform.position;
#endif
			if (Targeting.GetTarget(Player.main.gameObject, range, out GameObject gameObject, out float num))
			{
				lineder.SetPosition(0, currentMuzzlePosition);
				lineder.SetPosition(1, (forward * num) + position);
				LaserParticles.transform.position = (forward * num) + position;
			}
			else
			{
				lineder.SetPosition(0, currentMuzzlePosition);
				lineder.SetPosition(1, (forward * range) + position);
				LaserParticles.transform.position = (forward * range) + position;
			}
		}

		public override void OnHolster()
		{
			Reset();
		}

		private void Start()
		{
			par[0] = base.gameObject.transform.Find(ModeChange)?.gameObject.GetComponent<ParticleSystem>();
			textmode = base.gameObject.transform.Find(ModeChange + "/ModeHud").gameObject.GetComponent<TextMesh>();
			textname = base.gameObject.transform.Find(GunMain + "/TargetDisplay/Name").gameObject.GetComponent<TextMesh>();
			textblood = base.gameObject.transform.Find(GunMain + "/TargetDisplay/Health").gameObject.GetComponent<TextMesh>();

			par[1] = base.gameObject.transform.Find(CannonMode + "/BlueOrb").gameObject.GetComponent<ParticleSystem>();
			par[2] = base.gameObject.transform.Find(CannonMode + "/Charge").gameObject.GetComponent<ParticleSystem>();
			par[3] = base.gameObject.transform.Find(CannonMode + "/Shoot").gameObject.GetComponent<ParticleSystem>();
			base.gameObject.transform.Find(CannonMode + "/Shoot").gameObject.EnsureComponent<ExplosionBehaviour>();
			base.gameObject.transform.Find(CannonMode + "/Shoot/Orb").gameObject.EnsureComponent<ExplosionBehaviour>();

			par[4] = base.gameObject.transform.Find(LaserMode + "/Laser").gameObject.GetComponent<ParticleSystem>();
			par[5] = base.gameObject.transform.Find(ScaleMode + "/LaserBig").gameObject.GetComponent<ParticleSystem>();
			par[6] = base.gameObject.transform.Find(ScaleMode + "/LaserSmall").gameObject.GetComponent<ParticleSystem>();

			Line[1] = base.gameObject.transform.Find(LaserMode + "/LaserLine").gameObject.GetComponent<LineRenderer>();
			Line[2] = base.gameObject.transform.Find(ScaleMode + "/LineBig").gameObject.GetComponent<LineRenderer>();
			Line[3] = base.gameObject.transform.Find(ScaleMode + "/LineSmall").gameObject.GetComponent<LineRenderer>();


			VFXFabricating vfxfabricating = base.gameObject.transform.Find("HandGun")?.gameObject.EnsureComponent<VFXFabricating>();
			vfxfabricating.localMinY = -3f;
			vfxfabricating.localMaxY = 3f;
			vfxfabricating.posOffset = new Vector3(0f, 0f, 0f);
			vfxfabricating.eulerOffset = new Vector3(0f, 90f, -90f);
			vfxfabricating.scaleFactor = 1f;

			GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("LaserParticles.prefab");
			LaserParticles = GameObject.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
		}

		public override bool OnAltDown()
		{
			if (energyMixin.charge > 0f || !GameModeUtils.RequiresPower())
			{
				par[0].Play();
				Reset();
				mode++;
				FMODUWE.PlayOneShot(modeChangeSound, base.transform.position, 0.5f);

				switch (mode)
				{
					case 1:
						textmode.text = "Harm";
						time = 10f;
						time2 = 10f;
						break;
					case 2:
						textmode.text = "Resize";
						break;
					case 3:
						textmode.text = "Standby";
						mode = 0;
						break;
				}
			}
			else
			{
				textmode.text = "No Power";
				mode = 0;
			}
			return true;
		}

		private void Update()
		{
			if (base.isDrawn && (energyMixin.charge > 0f || !GameModeUtils.RequiresPower()))
			{
				if (LaserFiring)
				{
					TargetLaser(Main.config.TargetingRange, Line[1]);
				}
				else if (ScaleBig)
				{
					TargetLaser(Main.config.TargetingRange, Line[2]);
				}
				else if (ScaleSmall)
				{
					TargetLaser(Main.config.TargetingRange, Line[3]);
				}
			}
		}

		private void LateUpdate()
		{
			if (base.isDrawn)
			{
				if (energyMixin.charge > 0f || !GameModeUtils.RequiresPower())
				{
					switch (mode)
					{
						case 1:
							HarmMode();
							break;
						case 2:
							ResizeMode();
							break;
					}
				}
				else
				{
					textmode.text = "No Power";
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
				if(Targeting.GetTarget(Player.main.gameObject, Main.config.TargetingRange, out GameObject gameObject, out float num2))
				{
					GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject) ?? gameObject;
					if (energyMixin.ConsumeEnergy(0.1f))
					{
						if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
						{
							entityRoot.transform.localScale = Vector3.one;
						}
						else if (ScaleBig)
						{
							par[5].gameObject.transform.Rotate(Vector3.forward * 5f);
							float changespeed = Main.config.ScaleUpspeed;
							Vector3 oldScale = entityRoot.transform.localScale;
							Vector3 newScale = new Vector3(oldScale.x + changespeed, oldScale.y + changespeed, oldScale.z + changespeed);

							entityRoot.transform.localScale = newScale;

							if (newScale.x >= 10f || newScale.y >= 10f || newScale.z >= 10f)
							{
								entityRoot.GetComponentInChildren<LiveMixin>()?.Kill(DamageType.Normal);
								entityRoot.GetComponentInChildren<BreakableResource>()?.HitResource();
								Drillable drillable = entityRoot?.GetComponent<Drillable>();

								if (drillable != null)
								{
									while (drillable.health.Sum() > 0)
									{
										drillable?.OnDrill(entityRoot.transform.position, null, out var _);
									}
								}
							}
						}
						else if (ScaleSmall)
						{
							par[6].gameObject.transform.Rotate(-Vector3.forward * 5f);

							float changespeed = Main.config.ScaleDownspeed;
							Vector3 oldScale = entityRoot.transform.localScale;
							Vector3 newScale = new Vector3(oldScale.x - changespeed, oldScale.y - changespeed, oldScale.z - changespeed);

							if (newScale.x > changespeed)
							{
								entityRoot.transform.localScale = newScale;
							}
							else
							{
								entityRoot.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
							}
						}
					}

					// Handles the Target display on the top of the gun.
					TechType techType = CraftData.GetTechType(entityRoot);
					string name = techType != TechType.None ? techType.AsString() : "";
					string translatedName = name != "" ? Language.main.GetOrFallback(name, name) + " size" : "";
					string scale = translatedName != "" ? System.Math.Round(entityRoot.transform.localScale.x, 2).ToString(): "";

					textname.text = translatedName;
					textblood.text = scale;
				}
				else
				{
					textname.text = "";
					textblood.text = "";
				}
			}
			catch
			{
				textname.text = "";
				textblood.text = "";
			}
		}

		/// <summary>
		/// Handles the Cannon and Laser functions of the Harm Mode.
		/// </summary>
		private void HarmMode()
		{
			try
			{
				// Handles the Target display on the top of the gun.
				if (Targeting.GetTarget(Player.main.gameObject, Main.config.TargetingRange, out GameObject gameObject4, out float num4) && gameObject4.TryGetComponent<LiveMixin>(out LiveMixin liveMixin))
				{
					TechType techType = CraftData.GetTechType(gameObject4);
					string name = techType != TechType.None ? techType.AsString() : "";
					string translatedName = Language.main.GetOrFallback(name, name);
					string health = liveMixin?.health.ToString() ?? "";

					if (health == "0")
					{
						health = "Dead";
					}
					textname.text = translatedName;
					textblood.text = health;
				}
				else
				{
					textname.text = "";
					textblood.text = "";
				}

				if (CannonCharging && energyMixin.ConsumeEnergy(0.1f))
				{
					if (time > 0f)
					{
						time -= 5f * Time.deltaTime;
					}
					else
					{
						par[2].Stop();
						if (time2 > 0f)
						{
							time2 -= 5f * Time.deltaTime;
						}
						else if (energyMixin.ConsumeEnergy(30f))
						{
							FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
							FMODUWE.PlayOneShot(stasisRifleFireSound, base.transform.position, 1f);
							par[1].Stop();
							par[1].Clear();
#if SN1
							par[3].transform.rotation = Player.main.camRoot.mainCamera.transform.rotation;
#elif BZ
						par[3].transform.rotation = Player.main.camRoot.mainCam.transform.rotation;
#endif
							par[3].Play();
							StartCannonCharging();
						}
					}
				}
				else if (LaserFiring)
				{
					energyMixin.ConsumeEnergy(0.2f);
					par[4].gameObject.transform.Rotate(Vector3.forward * 5f);
					if (Targeting.GetTarget(Player.main.gameObject, Main.config.TargetingRange, out GameObject gameObject, out float num))
					{
						var entityRoot = UWE.Utils.GetEntityRoot(gameObject);
						entityRoot.GetComponentInChildren<LiveMixin>()?.TakeDamage(Main.config.LaserDamage, gameObject.transform.position, DamageType.Heat, null);
						entityRoot.GetComponentInChildren<BreakableResource>()?.HitResource();
						entityRoot.GetComponentInChildren<Drillable>()?.OnDrill(entityRoot.transform.position, null, out var _);
					}
				}
			}
			catch
			{
				textname.text = "";
				textblood.text = "";
			}
		}

		public override bool OnRightHandUp()
		{
			switch (mode)
			{
				case 1:
					if (CannonCharging)
					{
						par[1].Stop();
						par[2].Stop();
						par[3].Stop();
						stasisRifleEvent.Stop(false);
						CannonCharging = false;
					}
					break;
				case 2:
					if (ScaleSmall)
					{
						par[6].Stop();
						Radical.FindChild(LaserParticles, "scale").GetComponent<ParticleSystem>().Stop();
						Line[3].SetPosition(0, new Vector3(0f, 0f, 0f));
						Line[3].SetPosition(1, new Vector3(0f, 0f, 0f));
						ScaleSmall = false;
					}
					break;
			}
			return true;
		}

		public override bool OnLeftHandUp()
		{
			switch (mode)
			{
				case 1:
					if (LaserFiring)
					{
						Line[1].SetPosition(0, new Vector3(0f, 0f, 0f));
						Line[1].SetPosition(1, new Vector3(0f, 0f, 0f));
						par[4].Stop();
						laserShootSound.Stop();
						LaserFiring = false;
						Radical.FindChild(LaserParticles, "Laserend").GetComponent<ParticleSystem>().Stop();
					}
					break;
				case 2:
					if (ScaleBig)
					{
						Radical.FindChild(LaserParticles, "scale").GetComponent<ParticleSystem>().Stop();
						Line[2].SetPosition(0, new Vector3(0f, 0f, 0f));
						Line[2].SetPosition(1, new Vector3(0f, 0f, 0f));
						par[5].Stop();
						ScaleBig = false;
					}
					break;
			}
			return true;
		}

		public override bool OnRightHandDown()
		{
			if (energyMixin.charge > 0f || !GameModeUtils.RequiresPower())
			{
				switch (mode)
				{
					case 1:
						if (!LaserFiring)
						{
							StartCannonCharging();
						}
						break;
					case 2:
						if (!ScaleBig)
						{
							Radical.FindChild(LaserParticles, "scale").GetComponent<ParticleSystem>().Play();
							par[6].Play();
							FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
							ScaleSmall = true;
						}
						break;
				}
			}
			return false;
		}

		private void StartCannonCharging()
		{
			CannonCharging = true;
			Console.WriteLine("1");
			par[1].Play();
			Console.WriteLine("2");
			par[2].Play();
			Console.WriteLine("3");
			time = 10f;
			time2 = 10f;
			stasisRifleEvent.StartEvent();
			Console.WriteLine("4");
		}

		public override bool OnLeftHandDown()
		{
			if (energyMixin.charge > 0f || !GameModeUtils.RequiresPower())
			{
				switch (mode)
				{
					case 1:
						if (!CannonCharging)
						{
							FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
							laserShootSound.Play();
							LaserFiring = true;
							par[4].Play();
							Radical.FindChild(LaserParticles, "Laserend").GetComponent<ParticleSystem>().Play();
						}
						break;
					case 2:
						if (!ScaleSmall)
						{
							FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
							par[5].Play();
							ScaleBig = true;
							Radical.FindChild(LaserParticles, "scale").GetComponent<ParticleSystem>().Play();
						}
						break;
				}
			}
			return false;
		}

		public void Reset()
		{
			if (LaserFiring || CannonCharging || ScaleBig || ScaleSmall)
			{
				LaserParticles.transform.Find("scale").GetComponent<ParticleSystem>().Stop();
				LaserParticles.transform.Find("Laserend").gameObject.GetComponent<ParticleSystem>().Stop();
				par[1].Stop();
				par[2].Stop();
				par[3].Stop();
				par[4].Stop();
				par[5].Stop();
				par[6].Stop();
				par[4].gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				laserShootSound.Stop();
				stasisRifleEvent.Stop(true);
				LaserFiring = false;
				CannonCharging = false;
				ScaleBig = false;
				ScaleSmall = false;
				Line[1].SetPosition(0, new Vector3(0f, 0f, 0f));
				Line[1].SetPosition(1, new Vector3(0f, 0f, 0f));
				Line[2].SetPosition(0, new Vector3(0f, 0f, 0f));
				Line[2].SetPosition(1, new Vector3(0f, 0f, 0f));
				Line[3].SetPosition(0, new Vector3(0f, 0f, 0f));
				Line[3].SetPosition(1, new Vector3(0f, 0f, 0f));
			}
			textname.text = "";
			textblood.text = "";
		}

		public void OnProtoSerialize(ProtobufSerializer serializer)
		{

			GameObject battery = energyMixin.GetBattery();
			if (battery != null)
			{
				TechType batteryTech = CraftData.GetTechType(battery);
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type", batteryTech.AsString());
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge", energyMixin.charge.ToString());
			}
			else
			{
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type", "None");
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge", "0");
			}
		}

		public void OnProtoDeserialize(ProtobufSerializer serializer)
		{
			if (energyMixin == null)
			{
				energyMixin = base.GetComponent<EnergyMixin>();
			}
			string typeFile = SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type";
			string chargeFile = SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge";

			bool flag2 = File.Exists(typeFile);
			if (flag2)
			{
				string a = File.ReadAllText(typeFile);
				float num = float.Parse(File.ReadAllText(chargeFile));
				if (a != "None" && TechTypeExtensions.FromString(a, out TechType batteryTech, true))
				{
#if SUBNAUTICA_EXP
					CoroutineHost.StartCoroutine(SetBattery(batteryTech, num));
				}
			}
		}

		private IEnumerator SetBattery(TechType techType, float num)
		{
			TaskResult<GameObject> result = new TaskResult<GameObject>();
			yield return CraftData.InstantiateFromPrefabAsync(techType, result);

			Battery battery = result.Get().GetComponent<Battery>();

			if(battery != null)
			{
				TaskResult<InventoryItem> task = new TaskResult<InventoryItem>();
				yield return energyMixin.SetBatteryAsync(techType, num / battery.capacity, task);
			}

			yield break;
		}
#else

					Battery battery = CraftData.InstantiateFromPrefab(batteryTech).GetComponent<Battery>();
					if (battery != null)
					{
						energyMixin.SetBattery(batteryTech, num / battery.capacity);
					}
				}
			}
		}
#endif
	}
}