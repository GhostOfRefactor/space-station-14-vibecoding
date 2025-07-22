using Content.Shared.DoAfter;
using Content.Shared.Hostage;
using Content.Shared.CombatMode;
using Content.Shared.Popups;
using Content.Shared.Damage;
using Content.Shared.Chat;
using Content.Shared.Interaction;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using System.Numerics;

namespace Content.Server.Hostage;

public sealed class HostageSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedCombatModeSystem _combat = default!;
    [Dependency] private readonly SharedSuicideSystem _suicide = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ThreatWeaponComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<HostageDoAfterEvent>(OnDoAfter);
    }

    private const float BaseTime = 2f;

    private void OnAfterInteract(EntityUid uid, ThreatWeaponComponent comp, AfterInteractEvent args)
    {
        if (args.Handled || args.Target == null || !args.CanReach)
            return;

        if (_combat.IsInCombatMode(args.User))
            return;

        if (!comp.CanThreaten)
            return;

        var time = BaseTime * comp.ThreatModifier;

        var ev = new HostageDoAfterEvent();
        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager, args.User, time, ev, args.Target.Value, target: args.Target.Value, used: uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = true,
        });

        args.Handled = true;
    }

    private void OnDoAfter(HostageDoAfterEvent ev)
    {
        if (ev.Cancelled || ev.Target == null || ev.Used == null)
            return;

        if (!TryComp<ThreatWeaponComponent>(ev.Used.Value, out var threat))
            return;

        var hostage = EnsureComp<HostageComponent>(ev.Target.Value);
        hostage.IsHostage = true;
        hostage.Threatener = ev.User;
        hostage.IsGun = threat.IsGun;
        hostage.MaxDistance = threat.IsGun ? 1.5f : 0.5f;
        Dirty(ev.Target.Value, hostage);

        _popup.PopupEntity(Loc.GetString("hostage-threat", ("name", EntityManager.GetComponent<MetaDataComponent>(ev.User).EntityName)), ev.Target.Value, Filter.Entities(ev.Target.Value), true, PopupType.LargeCaution);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<HostageComponent>();
        while (query.MoveNext(out var uid, out var hostage))
        {
            if (!hostage.IsHostage || hostage.Threatener == EntityUid.Invalid)
                continue;

            if (!Exists(hostage.Threatener))
            {
                RemCompDeferred<HostageComponent>(uid);
                continue;
            }

            var dist = Vector2.Distance(_transform.GetMapCoordinates(uid).Position,
                _transform.GetMapCoordinates(hostage.Threatener).Position);
            if (dist > hostage.MaxDistance && hostage.IsHardThreatEnabled && TryComp(hostage.Threatener, out HardThreatComponent? hard))
            {
                if (TryComp<DamageableComponent>(uid, out var damage))
                    _suicide.ApplyLethalDamage((uid, damage), hard.LethalHit);
                RemCompDeferred<HostageComponent>(uid);
            }
        }
    }

    public void DebugHostage(EntityUid threatener, EntityUid victim)
    {
        var hostage = EnsureComp<HostageComponent>(victim);
        hostage.IsHostage = true;
        hostage.Threatener = threatener;
        hostage.IsGun = false;
        hostage.MaxDistance = 1.5f;
        Dirty(victim, hostage);
    }
}

[Serializable, NetSerializable]
public sealed partial class HostageDoAfterEvent : SimpleDoAfterEvent { }
