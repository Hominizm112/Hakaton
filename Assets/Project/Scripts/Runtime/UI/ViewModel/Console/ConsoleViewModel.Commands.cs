using Zenject;

public partial class ConsoleViewModel
{
    [Inject] private CurrencyPresenter _currencyPresenter;
    [Inject] private LazyInject<AppController> _appController;
    [Inject] private SaveManager _saveService;
    [Inject] private InventoryService _inventoryService;


    [ConsoleCommand("addcurrency", "Adds currency to player", "addcurrency <amount>")]
    private CommandResult AddCurrencyCommand(CommandContext context)
    {
        if (context.ArgumentCount == 0)
            return CommandResult.Error("Usage: addcurrency <amount>");

        int amount = context.GetInt(0, 0);
        _currencyPresenter?.AddCurrency(amount);
        return CommandResult.Ok($"Added {amount} currency");
    }

    [ConsoleCommand("loadscene", "Loads a scene", "loadscene <sceneName>")]
    private CommandResult LoadSceneCommand(CommandContext context)
    {
        if (context.ArgumentCount == 0)
            return CommandResult.Error("Usage: loadscene <sceneName>");

        string sceneName = context.GetString(0);
        // _mediator.LoadScene(sceneName, Game.State.Gameplay);
        return CommandResult.Ok($"Loading scene: {sceneName}");
    }

    [ConsoleCommand("unlockemail", "Unlock an email", "unlockemail <npcName> <friendLevel>")]
    private CommandResult UnlockEmailCommand(CommandContext context)
    {
        if (context.ArgumentCount < 2)
            return CommandResult.Error("Usage: unlockemail <npcName> <friendLevel>");

        if (_appController.Value != null)
        {
            // _appController.Value.GetApp<EmailApp>()?.UnlockEmailFromConsole(context.GetString(0, ""), context.GetInt(1, 0));
        }
        return CommandResult.Ok($"Email unlocked.");
    }


    [ConsoleCommand("save", "Save game data", "save")]
    private CommandResult SaveCommand(CommandContext context)
    {
        _ = _saveService.SaveDataAsync();
        return CommandResult.Ok("Game data saved.");
    }



    [ConsoleCommand("load", "Load game data", "load")]
    private CommandResult LoadCommand(CommandContext context)
    {
        _ = _saveService.LoadDataAsync();
        return CommandResult.Ok("Game data saved.");
    }

    [ConsoleCommand("additem", "Adds an item to player's inventory", "additem <itemId> <quantity>")]
    private CommandResult AddItemCommand(CommandContext context)
    {
        if (_inventoryService.AddItem(context.GetString(0, ""), context.GetInt(1, 1)))
        {
            return CommandResult.Ok("Item Added.");
        }
        else
        {
            return CommandResult.Error("Item with provided itemId not found.");
        }
    }

    [ConsoleCommand("removeitem", "Removes an item from player's inventory", "removeitem <itemId> <quantity>")]
    private CommandResult RemoveItemCommand(CommandContext context)
    {
        if (_inventoryService.RemoveItem(context.GetString(0, ""), context.GetInt(1, 1)))
        {
            return CommandResult.Ok("Item Remove.");
        }
        else
        {
            return CommandResult.Error("Item with provided itemId not found, or you are trying to remove more than you have.");
        }
    }

}
