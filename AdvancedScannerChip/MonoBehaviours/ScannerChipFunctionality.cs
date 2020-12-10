using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedScannerChip.MonoBehaviours
{
	public class ScannerChipFunctionality : MapRoomFunctionality
	{
		public static new void GetMapRoomsInRange(Vector3 position, float range, ICollection<MapRoomFunctionality> outlist)
		{
			float num = range * range;
			for (int i = 0; i < MapRoomFunctionality.mapRooms.Count; i++)
			{
				MapRoomFunctionality mapRoomFunctionality = MapRoomFunctionality.mapRooms[i];
				if ((mapRoomFunctionality.transform.position - position).sqrMagnitude <= num)
				{
					outlist.Add(mapRoomFunctionality);
				}
			}
		}

		public new TechType GetActiveTechType()
		{
			return this.typeToScan;
		}

		public new void Start()
		{
			this.typeToScan = TechType.Fragment;
			this.numNodesScanned = 50000;
			if (this.typeToScan != TechType.None)
			{
				this.StartScanning(this.typeToScan);
			}

			ResourceTracker.onResourceDiscovered += this.OnResourceDiscovered;
			ResourceTracker.onResourceRemoved += this.OnResourceRemoved;
			MapRoomFunctionality.mapRooms.Add(this);
		}

		public new void Update()
		{

		}

		public new void OnResourceDiscovered(ResourceTracker.ResourceInfo info)
		{
			if (this.typeToScan == info.techType)
			{
				this.resourceNodes.Add(info);
			}
		}

		public new void OnResourceRemoved(ResourceTracker.ResourceInfo info)
		{
			if (this.typeToScan == info.techType)
			{
				this.resourceNodes.Remove(info);
			}
		}

		private new void ObtainResourceNodes(TechType typeToScan)
		{
			this.resourceNodes.Clear();
			Vector3 scannerPos = Player.main.transform.position;
			Dictionary<string, ResourceTracker.ResourceInfo>.ValueCollection nodes = ResourceTracker.GetNodes(typeToScan);
			if (nodes != null)
			{
				float num = 150f * 150f;
				foreach (ResourceTracker.ResourceInfo resourceInfo in nodes)
				{
					if ((scannerPos - resourceInfo.position).sqrMagnitude <= num)
					{
						this.resourceNodes.Add(resourceInfo);
					}
				}
			}
			this.resourceNodes.Sort(delegate (ResourceTracker.ResourceInfo a, ResourceTracker.ResourceInfo b)
			{
				float sqrMagnitude = (a.position - scannerPos).sqrMagnitude;
				float sqrMagnitude2 = (b.position - scannerPos).sqrMagnitude;
				return sqrMagnitude.CompareTo(sqrMagnitude2);
			});
		}

		public new void StartScanning(TechType newTypeToScan)
		{
			this.typeToScan = newTypeToScan;
			this.ObtainResourceNodes(this.typeToScan);
			this.scanActive = this.typeToScan > TechType.None;
		}

		public new void OnDestroy()
		{
			ResourceTracker.onResourceDiscovered -= this.OnResourceDiscovered;
			ResourceTracker.onResourceRemoved -= this.OnResourceRemoved;
			MapRoomFunctionality.mapRooms.Remove(this);
		}

		public ScannerChipFunctionality()
		{

		}

		static ScannerChipFunctionality()
		{

		}
	}
}
