using Robust.Shared.GameStates;

namespace Content.Shared.Hostage;

/// <summary>
///     Marks a weapon that can be used to take hostages.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ThreatWeaponComponent : Component
{
    /// <summary>Modifier for the do-after time.</summary>
    [DataField, AutoNetworkedField]
    public float ThreatModifier = 1f;

    /// <summary>Whether this item can threaten at all.</summary>
    [DataField, AutoNetworkedField]
    public bool CanThreaten = true;

    /// <summary>Whether this is considered a gun.</summary>
    [DataField, AutoNetworkedField]
    public bool IsGun = false;
}
