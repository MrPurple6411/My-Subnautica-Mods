using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UWE;

namespace CustomCommands.MonoBehaviours
{
	// Token: 0x02000002 RID: 2
	public class Commands : MonoBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void Awake()
		{
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.Command);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002065 File Offset: 0x00000265
		public void OnDestroy()
		{
			Placeholder.Awake();
			SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.Command);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
		public void Command(Scene off, LoadSceneMode on)
		{
			DevConsole.RegisterConsoleCommand(this, "spse", false, false);
			DevConsole.RegisterConsoleCommand(this, "pickup", false, false);
			DevConsole.RegisterConsoleCommand(this, "friend", false, false);
			DevConsole.RegisterConsoleCommand(this, "playse", false, false);
			DevConsole.RegisterConsoleCommand(this, "size", false, false);
			DevConsole.RegisterConsoleCommand(this, "suse", false, false);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E4 File Offset: 0x000002E4
		private bool Gettarget(out GameObject targeta)
		{
			RaycastHit raycastHit;
			Physics.Raycast(new Ray(MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 0.15f, MainCamera.camera.transform.forward), out raycastHit, 100f);
			bool flag = raycastHit.collider != null;
			if (flag)
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				bool flag2 = gameObject.GetComponentInParent<PrefabIdentifier>() != null;
				if (flag2)
				{
					targeta = gameObject.GetComponentInParent<PrefabIdentifier>().gameObject;
					return targeta;
				}
			}
			targeta = null;
			return false;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002198 File Offset: 0x00000398
		public void OnConsoleCommand_pickup()
		{
			GameObject gameObject;
			bool flag = this.Gettarget(out gameObject);
			if (flag)
			{
				bool flag2 = gameObject.GetComponent<Pickupable>() == null;
				if (flag2)
				{
					gameObject.AddComponent<Pickupable>();
					ErrorMessage.AddDebug(gameObject.name + " add pick up");
				}
				else
				{
                    UnityEngine.Object.Destroy(gameObject.GetComponent<Pickupable>());
					ErrorMessage.AddDebug(gameObject.name + " remove pick up");
				}
			}
			else
			{
				ErrorMessage.AddDebug("Target not found");
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002218 File Offset: 0x00000418
		public void OnConsoleCommand_size(NotificationCenter.Notification n)
		{
			this.x = 1f;
			this.y = 1f;
			this.z = 1f;
			bool flag = n.data.Count == 1;
			if (flag)
			{
				this.x = float.Parse((string)n.data[0]);
				this.y = float.Parse((string)n.data[0]);
				this.z = float.Parse((string)n.data[0]);
			}
			bool flag2 = n.data.Count == 2;
			if (flag2)
			{
				this.x = float.Parse((string)n.data[0]);
				this.y = float.Parse((string)n.data[1]);
				this.z = float.Parse((string)n.data[1]);
			}
			bool flag3 = n.data.Count == 3;
			if (flag3)
			{
				this.x = float.Parse((string)n.data[0]);
				this.y = float.Parse((string)n.data[1]);
				this.z = float.Parse((string)n.data[2]);
			}
			GameObject gameObject;
			bool flag4 = this.Gettarget(out gameObject);
			if (flag4)
			{
				gameObject.transform.localScale = new Vector3(this.x, this.y, this.z);
				ErrorMessage.AddDebug(gameObject.name + " Size changed complete");
			}
			else
			{
				ErrorMessage.AddDebug("Target not found");
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002408 File Offset: 0x00000608
		public void OnConsoleCommand_playse(NotificationCenter.Notification n)
		{
			float num = float.Parse((string)n.data[0]);
			Player.mainObject.transform.localScale = new Vector3(num, num, num);
			ErrorMessage.AddDebug("Warning!!!! This feature may cause you to be unable to move and other issues");
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002458 File Offset: 0x00000658
		public void OnConsoleCommand_friend()
		{

			GameObject gameObject;
			bool flag = this.Gettarget(out gameObject) && gameObject.GetComponent<Creature>();
			if (flag)
			{
#if SN1
				bool flag2 = gameObject.GetComponent<Creature>().friend != Player.main.gameObject;
				if (flag2)
				{
					gameObject.GetComponent<Creature>().Friendliness.Add(1f);
					gameObject.GetComponent<Creature>().friend = Player.main.gameObject;
					ErrorMessage.AddDebug(gameObject.name + " set friend");
				}
				else
				{
					gameObject.GetComponent<Creature>().Friendliness.Value = 0f;
					gameObject.GetComponent<Creature>().friend = null;
					ErrorMessage.AddDebug(gameObject.name  + " remove friend");
				}
#elif BZ
				bool flag2 = gameObject.GetComponent<Creature>().GetFriend() != Player.main.gameObject;
				if (flag2)
				{
					gameObject.GetComponent<Creature>().Friendliness.Add(1f);
					gameObject.GetComponent<Creature>().SetFriend(Player.main.gameObject);
					ErrorMessage.AddDebug(gameObject.name + " set friend");
				}
				else
				{
					gameObject.GetComponent<Creature>().Friendliness.Value = 0f;
					gameObject.GetComponent<Creature>().SetFriend(null);
					ErrorMessage.AddDebug(gameObject.name + " remove friend");
				}
#endif
			}
			else
			{
				ErrorMessage.AddDebug("Target not found or Not a creature");
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002534 File Offset: 0x00000734
		public void OnConsoleCommand_spse(NotificationCenter.Notification n)
		{
			bool flag = n != null && n.data != null && n.data.Count > 0;
			if (flag)
			{
				string text = (string)n.data[0];
				TechType techType;
				bool flag2 = UWE.Utils.TryParseEnum<TechType>(text, out techType);
				if (flag2)
				{
					bool flag3 = CraftData.IsAllowed(techType);
					if (flag3)
					{
						CoroutineHost.StartCoroutine(SpawnTechType(n,techType));
					}
				}
				else
				{
					ErrorMessage.AddDebug("Unable to read " + text + " as TechType");
				}
			}
		}

		private IEnumerator SpawnTechType(NotificationCenter.Notification n, TechType techType)
		{
			CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType, false);
			yield return task;
			GameObject prefabForTechType = task.GetResult();
			if (prefabForTechType != null)
			{
				this.x = 1f;
				this.y = 1f;
				this.z = 1f;
				int num = 6;
				int num2 = 1;

				if (n.data.Count > 1)
				{
					num2 = int.Parse((string)n.data[1]);
				}
				if (n.data.Count > 2)
				{
					this.x = float.Parse((string)n.data[2]);
				}
				if (n.data.Count > 3)
				{
					this.y = float.Parse((string)n.data[3]);
				}
				if (n.data.Count > 4)
				{
					this.z = float.Parse((string)n.data[4]);
				}
				if (n.data.Count > 5)
				{
					num = int.Parse((string)n.data[5]);
				}
				if (n.data.Count == 3)
				{
					this.y = float.Parse((string)n.data[2]);
					this.z = float.Parse((string)n.data[2]);
				}
				if (n.data.Count == 4)
				{
					this.z = float.Parse((string)n.data[3]);
				}

				for (int i = 0; i < num2; i++)
				{
					GameObject gameObject = global::Utils.CreatePrefab(prefabForTechType, (float)num, i > 0);
					UWE.Utils.GetEntityRoot(gameObject).transform.localScale = new Vector3(this.x, this.y, this.z);
					bool flag12 = gameObject.GetComponent<Creature>() != null;
					if (flag12)
					{
						gameObject.AddComponent<mono>().Setsize(this.x, this.y, this.z);
					}
					LargeWorldEntity.Register(gameObject);
					CrafterLogic.NotifyCraftEnd(gameObject, techType);
					gameObject.SendMessage("StartConstruction", 1);
				}
			}
			else
			{
				ErrorMessage.AddDebug("Prefab not found for " + techType);
			}
			yield break;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000285C File Offset: 0x00000A5C
		public void OnConsoleCommand_suse(NotificationCenter.Notification n)
		{
			string text = (string)n.data[0];
			bool flag = text != null && text != string.Empty;
			if (flag)
			{
				this.x = 1f;
				this.y = 1f;
				this.z = 1f;
				int num = 6;
				bool flag2 = n.data.Count > 1;
				if (flag2)
				{
					this.x = float.Parse((string)n.data[1]);
				}
				bool flag3 = n.data.Count > 2;
				if (flag3)
				{
					this.y = float.Parse((string)n.data[2]);
				}
				bool flag4 = n.data.Count > 3;
				if (flag4)
				{
					this.z = float.Parse((string)n.data[3]);
				}
				bool flag5 = n.data.Count > 4;
				if (flag5)
				{
					num = int.Parse((string)n.data[4]);
				}
				bool flag6 = n.data.Count == 2;
				if (flag6)
				{
					this.y = float.Parse((string)n.data[1]);
					this.z = float.Parse((string)n.data[1]);
				}
				bool flag7 = n.data.Count == 3;
				if (flag7)
				{
					this.z = float.Parse((string)n.data[2]);
				}
				Transform transform = MainCamera.camera.transform;
				this.spawnPosition = transform.position + (float)num * transform.forward;
				this.spawnRotation = Quaternion.LookRotation(MainCamera.camera.transform.right);
				LightmappedPrefabs.main.RequestScenePrefab(text, new LightmappedPrefabs.OnPrefabLoaded(this.OnSubPrefabLoaded));
			}
			else
			{
				ErrorMessage.AddDebug("Must specify name (cyclops beetle aurora)");
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002A8C File Offset: 0x00000C8C
		private void OnSubPrefabLoaded(GameObject prefab)
		{
			GameObject gameObject = global::Utils.SpawnPrefabAt(prefab, null, this.spawnPosition);
			gameObject.transform.localScale = new Vector3(this.x, this.y, this.z);
			gameObject.transform.rotation = this.spawnRotation;
			gameObject.SetActive(true);
			gameObject.SendMessage("StartConstruction", 1);
			LargeWorldEntity.Register(gameObject);
			CrafterLogic.NotifyCraftEnd(gameObject, CraftData.GetTechType(gameObject));
		}

		// Token: 0x04000001 RID: 1
		private Quaternion spawnRotation;

		// Token: 0x04000002 RID: 2
		private Vector3 spawnPosition;

		// Token: 0x04000003 RID: 3
		private float x;

		// Token: 0x04000004 RID: 4
		private float y;

		// Token: 0x04000005 RID: 5
		private float z;

		// Token: 0x04000006 RID: 6
		public bool friend = false;

		// Token: 0x04000007 RID: 7
		public bool pickup = false;
	}
}
