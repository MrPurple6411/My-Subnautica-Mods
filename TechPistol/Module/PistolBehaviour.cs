﻿using System;
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
		public TextMesh textName;
		public TextMesh textHealth;
		public TextMesh textMode;
		private bool PowerCheck => energyMixin.charge > 0f || !GameModeUtils.RequiresPower();

		public const string GunMain = "HandGun/GunMain";
		public const string Point = GunMain + "/GunCylinder/Point";
		public const string CannonMode = Point + "/CannonMode";
		public const string LaserMode = Point + "/LaserMode";
		public const string ScaleMode = Point + "/ScaleMode";

		public Vector3 currentMuzzlePosition => base.gameObject.transform.Find(Point).position;

		public override string animToolName => TechType.Scanner.AsString(true);

		private void TargetLaser(float range, LineRenderer lineRenderer)
		{
			Vector3 forward = Player.main.camRoot.mainCam.transform.forward;
			Vector3 position = Player.main.camRoot.mainCam.transform.position;

			if (Targeting.GetTarget(Player.main.gameObject, range, out GameObject gameObject, out float num))
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, currentMuzzlePosition);
				lineRenderer.SetPosition(1, (forward * num) + position);
				LaserParticles.transform.position = (forward * num) + position;
			}
			else
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, currentMuzzlePosition);
				lineRenderer.SetPosition(1, (forward * range) + position);
				LaserParticles.transform.position = (forward * range) + position;
			}
		}

		public override void OnHolster()
		{
			Reset();
		}

		private void Start()
		{
			par[0] = base.gameObject.transform.Find(GunMain + "/ModeChange").gameObject.GetComponent<ParticleSystem>();
			textMode = base.gameObject.transform.Find(GunMain + "/ModeChange/ModeHud").gameObject.GetComponent<TextMesh>();
			textName = base.gameObject.transform.Find(GunMain + "/TargetDisplay/Name").gameObject.GetComponent<TextMesh>();
			textHealth = base.gameObject.transform.Find(GunMain + "/TargetDisplay/Health").gameObject.GetComponent<TextMesh>();

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


			VFXFabricating vfxfabricating = base.gameObject.transform.Find("HandGun").gameObject.EnsureComponent<VFXFabricating>();
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
			if (PowerCheck)
			{
				par[0].Play();
				Reset();
				mode++;
				FMODUWE.PlayOneShot(modeChangeSound, base.transform.position, 0.5f);

				switch (mode)
				{
					case 1:
						textMode.text = "Laser";
						break;
					case 2:
						textMode.text = "Cannon";
						time = 10f;
						time2 = 10f;
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

		private void Update()
		{
			if (base.isDrawn && PowerCheck)
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
				if (PowerCheck)
				{
					switch (mode)
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
				if(Targeting.GetTarget(Player.main.gameObject, Main.config.TargetingRange, out GameObject gameObject, out float num2))
				{
					GameObject entityRoot = UWE.Utils.GetEntityRoot(gameObject) ?? gameObject;

					// Handles the Target display on the top of the gun.
					TechType techType = CraftData.GetTechType(entityRoot);
					string name = techType != TechType.None ? techType.AsString() : "";
					string translatedName = name != "" ? Language.main.GetOrFallback(name, name) + " size" : "";
					string scale = translatedName != "" ? System.Math.Round(entityRoot.transform.localScale.x, 2).ToString() : "";

					textName.text = translatedName;
					textHealth.text = scale;

					if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
					{
						entityRoot.transform.localScale = Vector3.one;
						return;
					}
					else if (GameInput.GetButtonHeld(GameInput.Button.RightHand) && ScaleBig && (energyMixin.ConsumeEnergy(0.1f) || !GameModeUtils.RequiresPower()))
					{
						par[5].gameObject.transform.Rotate(Vector3.forward * 5f);
						float changespeed = Main.config.ScaleUpspeed;
						Vector3 oldScale = entityRoot.transform.localScale;
						Vector3 newScale = new Vector3(oldScale.x + changespeed, oldScale.y + changespeed, oldScale.z + changespeed);

						entityRoot.transform.localScale = newScale;

						if (Main.config.LethalResizing && (newScale.x >= Main.config.ScaleKillSize || newScale.y >= Main.config.ScaleKillSize || newScale.z >= Main.config.ScaleKillSize))
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
					else if (GameInput.GetButtonHeld(GameInput.Button.RightHand) && ScaleSmall && (energyMixin.ConsumeEnergy(0.1f) || !GameModeUtils.RequiresPower()))
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
							entityRoot.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
						}
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
			try
			{
				// Handles the Target display on the top of the gun.
				if (Targeting.GetTarget(Player.main.gameObject, Main.config.TargetingRange, out GameObject gameObject4, out float num4) && UWE.Utils.GetEntityRoot(gameObject4).TryGetComponent<LiveMixin>(out LiveMixin liveMixin))
				{
					string name = CraftData.GetTechType(liveMixin.gameObject).AsString();
					string translatedName = Language.main.GetOrFallback(name, name);

					if (translatedName.ToLower().Contains("school") && liveMixin.health == 0)
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

				if (GameInput.GetButtonHeld(GameInput.Button.RightHand) && CannonCharging && (energyMixin.ConsumeEnergy(0.1f) || !GameModeUtils.RequiresPower()))
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
						else if (energyMixin.ConsumeEnergy(30f) || !GameModeUtils.RequiresPower())
						{
							FMODUWE.PlayOneShot(repulsionCannonFireSound, base.transform.position, 1f);
							FMODUWE.PlayOneShot(stasisRifleFireSound, base.transform.position, 1f);
							par[1].Stop();
							par[1].Clear();
							par[3].transform.rotation = Player.main.camRoot.mainCam.transform.rotation;
							par[3].Play();
							CannonCharging = false;
						}
						else
						{
							Reset();
						}
					}
				}
				else if (GameInput.GetButtonHeld(GameInput.Button.RightHand) && LaserFiring && (energyMixin.ConsumeEnergy(0.2f) || !GameModeUtils.RequiresPower()))
				{
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
				textName.text = "";
				textHealth.text = "";
			}
		}

		public override bool OnRightHandUp()
		{
			Reset();
			return true;
		}

		public override bool OnRightHandDown()
		{
			if (PowerCheck)
			{
				switch (mode)
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
						time = 10f;
						time2 = 10f;
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
				Line[1].enabled = false;
				Line[2].SetPosition(0, new Vector3(0f, 0f, 0f));
				Line[2].SetPosition(1, new Vector3(0f, 0f, 0f));
				Line[2].enabled = false;
				Line[3].SetPosition(0, new Vector3(0f, 0f, 0f));
				Line[3].SetPosition(1, new Vector3(0f, 0f, 0f));
				Line[3].enabled = false;
			}
			textName.text = "";
			textHealth.text = "";
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