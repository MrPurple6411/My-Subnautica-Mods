namespace NoCrosshair
{
    using System.Collections.Generic;

    public static class NoCrosshair
    {
        internal static bool check = true;
        internal static uGUI_HandReticleIcon icon;
        internal static Dictionary<HandReticle.IconType, uGUI_HandReticleIcon> icons;
        internal static bool mapCheck;

        internal static void ChangeCrosshair(bool show)
        {
            if(icons != null)
            {
                if(!show)
                {
                    if(icon == null)
                    {
                        icon = icons[HandReticle.IconType.Default];
                    }
                    if(icons.ContainsKey(HandReticle.IconType.Default))
                        icons.Remove(HandReticle.IconType.Default);
                }
                else if(icon)
                {
                    icons[HandReticle.IconType.Default] = icon;
                }
                icon?.SetActive(show, 0.1f);
            }
        }
    }
}