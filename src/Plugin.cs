using BepInEx;
using MonoMod.Cil;
using System;
using System.Security.Permissions;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace MonkTempFix;

[BepInPlugin("alduris.monktempfix", "Monk Temp Fix", "0.0.0")]
sealed class Plugin : BaseUnityPlugin
{
    public void OnEnable()
    {
        IL.HUD.Map.ctor += Map_ctor;
    }

    public void OnDisable()
    {
        IL.HUD.Map.ctor -= Map_ctor;
    }

    private void Map_ctor(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchLdstr("gate condition on map, karma"));
            c.GotoNext(MoveType.After, x => x.MatchLdfld<HUD.Map.MapData.GateData>("karma"));
            c.EmitDelegate((RegionGate.GateRequirement req) => req ?? RegionGate.GateRequirement.OneKarma);
        }
        catch(Exception e)
        {
            Logger.LogError("Failed to apply!");
            Logger.LogError(e);
        }
    }
}
