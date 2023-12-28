namespace BulletTime;

using BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
	public void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
	}

	public static bool BulletTimeEnabled { get; set; } = false;
	/// <summary>
	/// Reset timescale to 1 when b is pressed and raise and lower timescale when n and m are pressed.
	/// </summary>
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			BulletTimeEnabled = !BulletTimeEnabled;
			Time.timeScale = BulletTimeEnabled ? 0.25f : 1f;
		}
	}
}