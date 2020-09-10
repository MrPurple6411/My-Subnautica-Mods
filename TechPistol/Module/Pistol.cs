using System;
using System.Collections;
using System.IO;
using SMLHelper.V2.Utility;
using UnityEngine;
using UWE;

namespace TechPistol.Module
{
	[RequireComponent(typeof(EnergyMixin))]
	class Pistol : PlayerTool, IProtoEventListener
	{
		public FMODAsset shoot1;
		public FMODAsset shoot2;
		public FMOD_StudioEventEmitter xulikai;
#if SN1
		public FMODASRPlayer laseroopS;
#elif BZ
		public FMOD_CustomEmitter laseroopS;
#endif
		public FMODAsset modechang;
		public ParticleSystem[] par = new ParticleSystem[10];
		public LineRenderer[] Line = new LineRenderer[10];
		public GameObject dis;
		public bool CannonStart = false;
		public bool LaserStart = false;
		public bool Scalebig = false;
		public bool Scalesamm = false;
		public float time;
		public float time2;
		public int mode;
		public TextMesh textname;
		public TextMesh textblood;
		public TextMesh textmode;


		public override string animToolName => "flashlight";

		private void TargetLaser(float range, LineRenderer lineder)
		{
			bool target = Targeting.GetTarget(Player.main.gameObject, range, out GameObject gameObject, out float num);
#if SN1
			Vector3 forward = Player.main.camRoot.mainCamera.transform.forward;
			Vector3 position = Player.main.camRoot.mainCamera.transform.position;
#elif BZ
			Vector3 forward = Player.main.camRoot.mainCam.transform.forward;
			Vector3 position = Player.main.camRoot.mainCam.transform.position;
#endif
			if (target)
			{
				lineder.SetPosition(0, Radical.FindChild(base.gameObject, "Point").transform.position);
				lineder.SetPosition(1, (forward * num) + position);
				this.dis.transform.position = (forward * num) + position;
			}
			else
			{
				lineder.SetPosition(0, Radical.FindChild(base.gameObject, "Point").transform.position);
				lineder.SetPosition(1, (forward * range) + position);
				this.dis.transform.position = (forward * range) + position;
			}
		}

		public override void OnHolster()
		{
			this.Reset();
		}


		private void Start()
		{
			GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("Dis.prefab");
			this.dis = GameObject.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
			this.par[0] = Radical.FindChild(base.gameObject, "modech").GetComponent<ParticleSystem>();
			GameObject gameObject2 = Radical.FindChild(base.gameObject, "Cannonmode");
			this.par[1] = Radical.FindChild(gameObject2, "Ball").GetComponent<ParticleSystem>();
			this.par[2] = Radical.FindChild(gameObject2, "Charge").GetComponent<ParticleSystem>();
			this.par[3] = Radical.FindChild(gameObject2, "shoot").GetComponent<ParticleSystem>();
			GameObject gameObject3 = Radical.FindChild(base.gameObject, "Lasermode");
			this.par[4] = Radical.FindChild(gameObject3, "Laser").GetComponent<ParticleSystem>();
			this.Line[1] = Radical.FindChild(gameObject3, "line").GetComponent<LineRenderer>();
			GameObject gameObject4 = Radical.FindChild(base.gameObject, "Scalemode");
			this.par[5] = Radical.FindChild(gameObject4, "Laser").GetComponent<ParticleSystem>();
			this.par[6] = Radical.FindChild(gameObject4, "Lasersamm").GetComponent<ParticleSystem>();
			this.Line[2] = Radical.FindChild(gameObject4, "linebig").GetComponent<LineRenderer>();
			this.Line[3] = Radical.FindChild(gameObject4, "linesamm").GetComponent<LineRenderer>();
			this.textname = base.gameObject.transform.Find("miazhun/name").gameObject.GetComponent<TextMesh>();
			this.textblood = base.gameObject.transform.Find("miazhun/blood").gameObject.GetComponent<TextMesh>();
			this.textmode = base.gameObject.transform.Find("modech/modehud").gameObject.GetComponent<TextMesh>();
		}

		public override bool OnAltDown()
		{
			bool flag = this.energyMixin.charge > 0f;
			if (flag)
			{
				this.par[0].Play();
				this.Reset();
				this.mode++;
				FMODUWE.PlayOneShot(this.modechang, base.transform.position, 1f);
				bool flag2 = this.mode == 1;
				if (flag2)
				{
					this.textmode.text = "Cannon";
					this.time = 10f;
					this.time2 = 10f;
				}
				bool flag3 = this.mode == 2;
				if (flag3)
				{
					this.textmode.text = "Laser";
				}
				bool flag4 = this.mode == 3;
				if (flag4)
				{
					this.textmode.text = "Scale";
				}
				bool flag5 = this.mode == 4;
				if (flag5)
				{
					this.textmode.text = "Standby";
					this.mode = 0;
				}
			}
			else
			{
				this.textmode.text = "No Power";
			}
			return true;
		}

		public void Update()
		{
			bool flag = this.energyMixin.charge > 0f && base.isDrawn;
			if (flag)
			{
				bool laserStart = this.LaserStart;
				if (laserStart)
				{
					this.TargetLaser(Main.config.LaserRange, this.Line[1]);
				}
				bool flag2 = this.mode == 3;
				if (flag2)
				{
					bool scalebig = this.Scalebig;
					if (scalebig)
					{
						this.TargetLaser(Main.config.ScaleRange, this.Line[2]);
					}
					bool scalesamm = this.Scalesamm;
					if (scalesamm)
					{
						this.TargetLaser(Main.config.ScaleRange, this.Line[3]);
					}
					bool flag3 = Input.GetKeyDown(KeyCode.Q) && this.energyMixin.charge > 0f;
					if (flag3)
					{
						bool flag4 = !this.Scalebig;
						if (flag4)
						{
							Radical.FindChild(this.dis, "scale").GetComponent<ParticleSystem>().Play();
							this.par[6].Play();
							FMODUWE.PlayOneShot(this.shoot1, base.transform.position, 1f);
							this.Scalesamm = true;
						}
					}
					bool keyUp = Input.GetKeyUp(KeyCode.Q);
					if (keyUp)
					{
						this.par[6].Stop();
						Radical.FindChild(this.dis, "scale").GetComponent<ParticleSystem>().Stop();
						this.Line[3].SetPosition(0, new Vector3(0f, 0f, 0f));
						this.Line[3].SetPosition(1, new Vector3(0f, 0f, 0f));
						this.Scalesamm = false;
					}
				}
			}
		}

		public void LateUpdate()
		{
			bool isDrawn = base.isDrawn;
			if (isDrawn)
			{
				bool flag = this.energyMixin.charge > 0f;
				if (flag)
				{
					bool flag2 = this.mode == 1 && this.CannonStart;
					if (flag2)
					{
						this.energyMixin.ConsumeEnergy(0.05f);
						bool flag3 = this.time > 0f;
						if (flag3)
						{
							this.time -= 5f * Time.deltaTime;
						}
						else
						{
							this.par[2].Stop();
							bool flag4 = this.time2 > 0f;
							if (flag4)
							{
								this.time2 -= 5f * Time.deltaTime;
							}
							else
							{
								this.energyMixin.ConsumeEnergy(30f);
								FMODUWE.PlayOneShot(this.shoot1, base.transform.position, 1f);
								FMODUWE.PlayOneShot(this.shoot2, base.transform.position, 1f);
								this.par[1].Stop();
								this.par[1].Clear();
#if SN1
								this.par[3].transform.rotation = Player.main.camRoot.mainCamera.transform.rotation;
#elif BZ
								this.par[3].transform.rotation = Player.main.camRoot.mainCam.transform.rotation;
#endif
								this.par[3].Play();
								this.time = 10f;
								this.time2 = 10f;
								this.CannonStart = false;
							}
						}
					}
					bool flag5 = this.mode == 2 && this.LaserStart;
					if (flag5)
					{
						this.energyMixin.ConsumeEnergy(0.2f);
						this.par[4].gameObject.transform.Rotate(Vector3.forward * 5f);
						bool flag6 = Targeting.GetTarget(Player.main.gameObject, Main.config.LaserRange, out GameObject gameObject, out float num) && gameObject.GetComponentInChildren<LiveMixin>();
						if (flag6)
						{
							UWE.Utils.GetEntityRoot(gameObject).GetComponentInChildren<LiveMixin>().TakeDamage(Main.config.LaserDamage, gameObject.transform.position, DamageType.Explosive, null);
						}
						else
						{
							bool flag7 = gameObject;
							if (flag7)
							{
								DamageSystem.RadiusDamage(Main.config.LaserDamage, gameObject.transform.position, 1f, DamageType.Explosive, UWE.Utils.GetEntityRoot(gameObject));
							}
						}
					}
					bool flag8 = this.mode == 3;
					if (flag8)
					{
						bool scalebig = this.Scalebig;
						if (scalebig)
						{
							this.energyMixin.ConsumeEnergy(0.1f);
							this.par[5].gameObject.transform.Rotate(Vector3.forward * 5f);
							bool flag9 = Targeting.GetTarget(Player.main.gameObject, Main.config.ScaleRange, out GameObject gameObject2, out float num2) && gameObject2.GetComponentInChildren<Creature>();
							if (flag9)
							{
								float x = UWE.Utils.GetEntityRoot(gameObject2).transform.localScale.x;
								UWE.Utils.GetEntityRoot(gameObject2).GetComponentInChildren<Creature>().SetScale(x + Main.config.ScaleUpspeed);
							}
						}
						bool scalesamm = this.Scalesamm;
						if (scalesamm)
						{
							this.energyMixin.ConsumeEnergy(0.1f);
							this.par[6].gameObject.transform.Rotate(-Vector3.forward * 5f);
							bool flag10 = Targeting.GetTarget(Player.main.gameObject, Main.config.ScaleRange, out GameObject gameObject3, out float num3) && gameObject3.GetComponentInChildren<Creature>();
							if (flag10)
							{
								float x2 = UWE.Utils.GetEntityRoot(gameObject3).transform.localScale.x;
								UWE.Utils.GetEntityRoot(gameObject3).GetComponentInChildren<Creature>().SetScale(x2 - Main.config.ScaleDownspeed);
							}
						}
					}
				}
				else
				{
					this.mode = 0;
					this.Reset();
				}
				bool flag11 = Targeting.GetTarget(Player.main.gameObject, Main.config.HealthDetectionRange, out GameObject gameObject4, out float num4) && gameObject4.GetComponentInChildren<LiveMixin>();
				if (flag11)
				{
					string text = gameObject4.GetComponentInChildren<LiveMixin>().health.ToString();
					string text2 = gameObject4.GetComponentInChildren<LiveMixin>().name;
					text2 = text2.Replace("(Clone)", "");
					text2 = text2.Replace("Leviathan", "");
					bool flag12 = text == "0";
					if (flag12)
					{
						text = "0-death";
					}
					this.textname.text = text2;
					this.textblood.text = text;
				}
				else
				{
					this.textname.text = "No target";
					this.textblood.text = "";
				}
			}
		}

		public override bool OnRightHandDown()
		{
			bool flag = this.energyMixin.charge > 0f;
			if (flag)
			{
				bool flag2 = this.mode == 1;
				if (flag2)
				{
					this.CannonStart = true;
					this.par[1].Play();
					this.par[2].Play();
					this.time = 10f;
					this.time2 = 10f;
					this.xulikai.StartEvent();
				}
				bool flag3 = this.mode == 2;
				if (flag3)
				{
					FMODUWE.PlayOneShot(this.shoot1, base.transform.position, 1f);
					this.laseroopS.Play();
					this.LaserStart = true;
					this.par[4].Play();
					Radical.FindChild(this.dis, "Laserend").GetComponent<ParticleSystem>().Play();
				}
				bool flag4 = this.mode == 3 && !this.Scalesamm;
				if (flag4)
				{
					FMODUWE.PlayOneShot(this.shoot1, base.transform.position, 1f);
					this.par[5].Play();
					this.Scalebig = true;
					Radical.FindChild(this.dis, "scale").GetComponent<ParticleSystem>().Play();
				}
			}
			return true;
		}

		public override bool OnRightHandUp()
		{
			bool flag = this.mode == 1;
			if (flag)
			{
				this.par[1].Stop();
				this.par[2].Stop();
				this.par[3].Stop();
				this.xulikai.Stop(false);
				this.CannonStart = false;
			}
			bool flag2 = this.mode == 2;
			if (flag2)
			{
				this.Line[1].SetPosition(0, new Vector3(0f, 0f, 0f));
				this.Line[1].SetPosition(1, new Vector3(0f, 0f, 0f));
				this.par[4].Stop();
				this.laseroopS.Stop();
				this.LaserStart = false;
				Radical.FindChild(this.dis, "Laserend").GetComponent<ParticleSystem>().Stop();
			}
			bool flag3 = this.mode == 3;
			if (flag3)
			{
				Radical.FindChild(this.dis, "scale").GetComponent<ParticleSystem>().Stop();
				this.Line[2].SetPosition(0, new Vector3(0f, 0f, 0f));
				this.Line[2].SetPosition(1, new Vector3(0f, 0f, 0f));
				this.par[5].Stop();
				this.Scalebig = false;
			}
			return true;
		}

		public void Reset()
		{
			Radical.FindChild(this.dis, "scale").GetComponent<ParticleSystem>().Stop();
			Radical.FindChild(this.dis, "Laserend").GetComponent<ParticleSystem>().Stop();
			this.par[1].Stop();
			this.par[2].Stop();
			this.par[3].Stop();
			this.par[4].Stop();
			this.par[5].Stop();
			this.par[6].Stop();
			this.par[4].gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			this.laseroopS.Stop();
			this.xulikai.Stop(true);
			this.LaserStart = false;
			this.CannonStart = false;
			this.Scalebig = false;
			this.Scalesamm = false;
			this.Line[1].SetPosition(0, new Vector3(0f, 0f, 0f));
			this.Line[1].SetPosition(1, new Vector3(0f, 0f, 0f));
			this.Line[2].SetPosition(0, new Vector3(0f, 0f, 0f));
			this.Line[2].SetPosition(1, new Vector3(0f, 0f, 0f));
			this.Line[3].SetPosition(0, new Vector3(0f, 0f, 0f));
			this.Line[3].SetPosition(1, new Vector3(0f, 0f, 0f));
		}

		public void OnProtoSerialize(ProtobufSerializer serializer)
		{

			string contents = null;
			GameObject battery = this.energyMixin.GetBattery();
			bool flag = battery;
			if (flag)
			{
				CraftData.GetTechType(battery);
				bool flag2 = CraftData.GetTechType(battery) == TechType.PrecursorIonBattery;
				if (flag2)
				{
					contents = "PrecursorIonBattery";
				}
				bool flag3 = CraftData.GetTechType(battery) == TechType.Battery;
				if (flag3)
				{
					contents = "Battery";
				}
				bool flag4 = CraftData.GetTechType(battery) == TechType.PowerCell;
				if (flag4)
				{
					contents = "PowerCell";
				}
				bool flag5 = CraftData.GetTechType(battery) == TechType.PrecursorIonPowerCell;
				if (flag5)
				{
					contents = "PrecursorIonPowerCell";
				}
				bool flag6 = CraftData.GetTechType(battery) != TechType.None && this.energyMixin.HasItem();
				if (flag6)
				{
					File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type", contents);
					File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge", this.energyMixin.charge.ToString());
				}
			}
			else
			{
				contents = "None";
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type", contents);
				File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge", "0");
			}
		}

		public void OnProtoDeserialize(ProtobufSerializer serializer)
		{
			bool flag = this.energyMixin == null;
			if (flag)
			{
				this.energyMixin = base.GetComponent<EnergyMixin>();
			}
			bool flag2 = File.Exists(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type");
			if (flag2)
			{
				string a = File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".type");
				float num = float.Parse(File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + base.GetComponent<PrefabIdentifier>().Id + ".charge"));
				bool flag3 = a != "None";
				if (flag3)
				{
					bool flag4 = a == "PrecursorIonBattery";
#if SUBNAUTICA_EXP
					if (flag4)
					{

						CoroutineHost.StartCoroutine(SetBattery(TechType.PrecursorIonBattery, num, 500f));
					}
					bool flag5 = a == "Battery";
					if (flag5)
					{
						CoroutineHost.StartCoroutine(SetBattery(TechType.Battery, num, 100f));
					}
					bool flag6 = a == "PowerCell";
					if (flag6)
					{
						CoroutineHost.StartCoroutine(SetBattery(TechType.PowerCell, num, 200f));
					}
					bool flag7 = a == "PrecursorIonPowerCell";
					if (flag7)
					{
						CoroutineHost.StartCoroutine(SetBattery(TechType.PrecursorIonPowerCell, num, 1000f));
					}
				}
			}
		}

		private IEnumerator SetBattery(TechType techType, float num, float num2)
		{
			TaskResult<InventoryItem> task = new TaskResult<InventoryItem>();
			yield return this.energyMixin.SetBatteryAsync(techType, num / num2, task);
		}
#else
					if (flag4)
					{
						this.energyMixin.SetBattery(TechType.PrecursorIonBattery, num / 500f);
					}
					bool flag5 = a == "Battery";
					if (flag5)
					{
						this.energyMixin.SetBattery(TechType.Battery, num / 100f);
					}
					bool flag6 = a == "PowerCell";
					if (flag6)
					{
						this.energyMixin.SetBattery(TechType.PowerCell, num / 200f);
					}
					bool flag7 = a == "PrecursorIonPowerCell";
					if (flag7)
					{
						this.energyMixin.SetBattery(TechType.PrecursorIonPowerCell, num / 1000f);
					}
				}
			}
		}
#endif
	}
}