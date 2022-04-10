using UnityEngine;

namespace GravTrapStorage.MonoBehaviours;

public class uGUI_GravtrapIcon: MonoBehaviour
{
	public uGUI_GravtrapIcon()
	{
	}

	public static uGUI_GravtrapIcon main;
	public Vector2 iconSize = new(108f, 108f);
	public float timeIn = 1f;
	public float timeOut = 0.5f;
	public float oscReduction = 100f;
	public float oscFrequency = 5f;
	public float oscScale = 2f;
	public float oscDuration = 2f;
	private uGUI_ItemIcon icon;
	private Sequence sequence = new();
	private bool show;
	private float oscSeed;
	private float oscTime;
	
	private void Awake()
	{
		if (main == null)
		{
			main = this;
			GameObject go = new("GravtrapIcon");
			go.transform.SetParent(gameObject.transform);
			icon = go.AddComponent<uGUI_ItemIcon>();
			icon.Init(null, transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
			icon.SetForegroundSprite(SpriteManager.Get(SpriteManager.Group.Item, TechType.Gravsphere.AsString()));
			icon.SetBackgroundSprite(SpriteManager.GetBackground(CraftData.BackgroundType.PlantAirSeed));
			icon.SetSize(iconSize);
			icon.SetBackgroundRadius(Mathf.Min(iconSize.x, iconSize.y) * 0.5f);
			Color color = new Color(1f, 0.6f, 0f, 2f);
			icon.SetBackgroundColors(color, color, color);
			SetAlpha(0f);
			sequence.ForceState(false);
			return;
		}
		Destroy(this);
	}

	private void LateUpdate()
	{
		if (sequence.target != show)
		{
			if (show && sequence.t == 0f)
			{
				oscTime = Time.time;
				oscSeed = Random.value;
			}
			sequence.Set(show ? timeIn : timeOut, show);
		}
		if (sequence.active)
		{
			if (sequence.t > 0f)
			{
				float num = 0f;
				float num2 = 0f;
				float t = Mathf.Clamp01((Time.time - oscTime) / oscDuration);
				MathExtensions.Oscillation(oscReduction, oscFrequency, oscSeed, t, out num, out num2);
				icon.rectTransform.localScale = new Vector3(1f + num * oscScale, 1f + num2 * oscScale, 1f);
			}
			sequence.Update();
		}
		SetAlpha(sequence.target ? 1f : sequence.t);
		show = false;
	}

	private void SetAlpha(float alpha)
	{
		icon.SetAlpha(alpha, alpha, alpha);
	}

	public void Show()
	{
		show = true;
	}
}