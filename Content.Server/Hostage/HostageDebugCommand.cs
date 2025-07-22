using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Timing;
using Content.Server.Administration;

namespace Content.Server.Hostage;

[AdminCommand(AdminFlags.Debug)]
public sealed class HostageDebugCommand : IConsoleCommand
{
    [Dependency] private readonly IEntitySystemManager _systems = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;

    public string Command => "hostagedebug";
    public string Description => "Force hostage status";
    public string Help => "hostagedebug <threatener> <victim>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2
            || !NetEntity.TryParse(args[0], out var threatNet)
            || !NetEntity.TryParse(args[1], out var victimNet)
            || !_entMan.TryGetEntity(threatNet, out var threat)
            || !_entMan.TryGetEntity(victimNet, out var victim))
        {
            shell.WriteError($"Usage: {Help}");
            return;
        }

        var sys = _systems.GetEntitySystem<HostageSystem>();
        sys.DebugHostage(threat.Value, victim.Value);
    }
}
