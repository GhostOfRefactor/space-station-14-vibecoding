using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared.Hostage;

/// <summary>
///     Attached to an entity that is being held hostage.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HostageComponent : Component
{
    /// <summary>Whether this entity is currently a hostage.</summary>
    [DataField, AutoNetworkedField]
    public bool IsHostage = false;

    /// <summary>The threatening entity.</summary>
    [DataField, AutoNetworkedField]
    public EntityUid Threatener = EntityUid.Invalid;

    /// <summary>Maximum allowed distance to the threatener.</summary>
    [DataField, AutoNetworkedField]
    public float MaxDistance = 1.5f;

    /// <summary>If the threatening weapon is a gun.</summary>
    [DataField, AutoNetworkedField]
    public bool IsGun = false;

    /// <summary>Whether hard threat mode is enabled.</summary>
    [DataField, AutoNetworkedField]
    public bool IsHardThreatEnabled = false;
}
