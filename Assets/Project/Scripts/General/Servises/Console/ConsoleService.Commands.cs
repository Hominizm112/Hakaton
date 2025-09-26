using UnityEngine;

public partial class ConsoleService
{
    #region Game-specific Commands

    [ConsoleCommand("god", "Toggle god mode", "god [on/off]")]
    private CommandResult GodCommand(CommandContext context)
    {
        if (!enableCheats)
            return CommandResult.Error("Cheats are disabled");

        // Implement your god mode logic here
        return CommandResult.Ok("God mode toggled");
    }

    [ConsoleCommand("addcurrency", "Adds currency to player", "addcurrency <amount>")]
    private CommandResult AddCurrencyCommand(CommandContext context)
    {
        if (!enableCheats)
            return CommandResult.Error("Cheats are disabled");

        if (context.ArgumentCount == 0)
            return CommandResult.Error("Usage: addcurrency <amount>");

        int amount = context.GetInt(0, 0);
        _mediator.GetService<CurrencyPresenter>()?.AddCurrency(amount);
        return CommandResult.Ok($"Added {amount} currency");
    }

    [ConsoleCommand("loadscene", "Loads a scene", "loadscene <sceneName>")]
    private CommandResult LoadSceneCommand(CommandContext context)
    {
        if (context.ArgumentCount == 0)
            return CommandResult.Error("Usage: loadscene <sceneName>");

        string sceneName = context.GetString(0);
        _mediator.LoadScene(sceneName, Game.State.Gameplay);
        return CommandResult.Ok($"Loading scene: {sceneName}");
    }

    [ConsoleCommand("unlockemail", "Unlock an email", "unlockemail <npcName> <friendLevel>")]
    private CommandResult UnlockEmailCommand(CommandContext context)
    {
        if (context.ArgumentCount < 2)
            return CommandResult.Error("Usage: unlockemail <npcName> <friendLevel>");

        _mediator.GetService<AppController>()?.GetApp<EmailApp>()?.UnlockEmailFromConsole(context.GetString(0, ""), context.GetInt(1, 0));
        return CommandResult.Ok($"Email unlocked.");
    }


    #endregion
}