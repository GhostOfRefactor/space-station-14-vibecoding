using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Timing;

namespace Content.Server.Hostage;

[AdminCommand(AdminFlags.Debug)]
public sealed class HostageDebugCommand : IConsoleCommand
{
    [Dependency] private readonly IEntitySystemManager _systems = default!;

    public string Command => "hostagedebug";
    public string Description => "Force hostage status";
    public string Help => "hostagedebug <threatener> <victim>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2
            || !NetEntity.TryParse(args[0], out var threat)
            || !NetEntity.TryParse(args[1], out var victim))
        {
            shell.WriteError($"Usage: {Help}");
            return;
        }

        var sys = _systems.GetEntitySystem<HostageSystem>();
        sys.DebugHostage(threat, victim);
    }
}
