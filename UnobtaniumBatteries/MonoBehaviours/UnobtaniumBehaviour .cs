namespace UnobtaniumBatteries.MonoBehaviours;

using UnityEngine;

internal class UnobtaniumBehaviour : MonoBehaviour
{
	private Battery _battery;
	private Renderer _renderer;
	private EnergyMixin _energyMixin;
	private PlayerTool _playerTool;
	private Vehicle _vehicle;
	private PowerRelay _powerRelay;

	private int _currentStrength;
	private int _nextStrength = 2;
	private const float ChangeTime = 2f;
	private float _timer;

	public void Awake()
	{
		_renderer = gameObject.GetComponentInChildren<Renderer>();
		_battery = gameObject.GetComponent<Battery>();
		_playerTool = gameObject.GetComponentInParent<PlayerTool>();
		_vehicle = gameObject.GetComponentInParent<Vehicle>();
		_energyMixin = gameObject.GetComponentInParent<EnergyMixin>();
		_powerRelay = PowerSource.FindRelay(base.transform);
	}

	public void Update()
	{
		if (_battery != null)
			_battery.charge = _battery.capacity;

		_timer += Time.deltaTime;

		if (_timer > ChangeTime)
		{
			_currentStrength = _nextStrength;
			_nextStrength = _currentStrength == 2 ? 0 : 2;
			try
			{
				_energyMixin?.AddEnergy(_energyMixin.capacity - _energyMixin.charge);
				_powerRelay?.ModifyPower(_powerRelay.GetMaxPower(), out _);
			}
			catch{ }

			_timer = 0.0f;
		}

		if(_playerTool != null)
		{
			switch (_playerTool)
			{
				case StasisRifle stasisRifle when stasisRifle.isCharging:
					while (stasisRifle.isCharging)
						stasisRifle.Charge();
					break;
				case Welder welder when welder.usedThisFrame && welder.activeWeldTarget != null && welder.activeWeldTarget.health < welder.activeWeldTarget.maxHealth:
					while (welder.usedThisFrame && welder.activeWeldTarget != null && welder.activeWeldTarget.health < welder.activeWeldTarget.maxHealth)
						welder.Weld();
					break;
				case LaserCutter laserCutter when laserCutter.usedThisFrame && laserCutter.activeCuttingTarget != null && laserCutter.activeCuttingTarget.openedAmount < laserCutter.activeCuttingTarget.maxOpenedAmount:
					bool flag = true;
					if (laserCutter.activeCuttingTarget && laserCutter.activeCuttingTarget.requireOpenFromFront && !Utils.CheckObjectInFront(laserCutter.activeCuttingTarget.transform, Player.main.transform, 90f))
					{
						flag = false;
					}
					while (flag && laserCutter.usedThisFrame && laserCutter.activeCuttingTarget != null && laserCutter.activeCuttingTarget.openedAmount < laserCutter.activeCuttingTarget.maxOpenedAmount)
						laserCutter.LaserCut();
					break;
			}
			return;
		}

		if (_vehicle != null)
		{
			if(_vehicle.activeSlot < 0 || _vehicle.activeSlot >= _vehicle.slotIDs.Length) return;

			var slotID = _vehicle.slotIDs[_vehicle.activeSlot];
			TechType techType = _vehicle.modules.GetTechTypeInSlot(slotID);
			if (techType == TechType.None) return;

			var currentCharge = _vehicle.quickSlotCharge[_vehicle.activeSlot];
			var maxCharge = CraftData.GetQuickSlotMaxCharge(techType);

			if (currentCharge < maxCharge)
			{
				_vehicle.quickSlotCharge[_vehicle.activeSlot] = maxCharge;
			}

			var currentCooldown = _vehicle.quickSlotCooldown[_vehicle.activeSlot];
			if (currentCooldown > 1f)
			{
				_vehicle.quickSlotCooldown[_vehicle.activeSlot] = 1f;
			}

			return;
		}

		if (_renderer == null) return;

		_renderer.material.SetFloat(ShaderPropertyID._GlowStrength, Mathf.Lerp(_currentStrength, _nextStrength, _timer / ChangeTime));
		_renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, Mathf.Lerp(_currentStrength, _nextStrength, _timer / ChangeTime));

	}

	public void OnDestroy()
	{
		_renderer?.material?.SetFloat(ShaderPropertyID._GlowStrength, 1f);
		_renderer?.material?.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
	}
}
