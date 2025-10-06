using GenericModConfigMenu;
using Instant_Keg;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace YourProjectName
{
    internal sealed class ModEntry : Mod
    {

        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Display.WindowResized += this.resizeCustomMenu;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                getValue: () => this.Config.InstantKegKeybind,
                setValue: value => this.Config.InstantKegKeybind = value,
                name: () => "Keybind (click to change):\n"
            );
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if(this.Config.InstantKegKeybind.JustPressed())
            {
                Farmer player = Game1.player;
                if (Game1.activeClickableMenu is null && Context.IsPlayerFree)
                {
                    Game1.activeClickableMenu = new ConfirmationMenu(
                        GetKegs().Count,
                        () => FillAllKegs(GetKegs()),
                        () => { Game1.exitActiveMenu(); }
                        );
                }
            }
        }

        private List<StardewValley.Object> GetKegs()
        {
            GameLocation location = Game1.player.currentLocation;
            List<StardewValley.Object> kegs = new();
            foreach (var pair in location.Objects.Pairs)
            {
                var obj = pair.Value;
                if (obj?.bigCraftable.Value == true && obj.QualifiedItemId == "(BC)12" && obj.heldObject.Value == null)
                    kegs.Add(obj);
            }
            return kegs;
        }

        private int FillAllKegs(List<StardewValley.Object> kegs)
        {
            Farmer player = Game1.player;
            StardewValley.Object? heldItem = player.ActiveItem as StardewValley.Object;
            if (heldItem is null)
                return kegs.Count;
            int beforeCount = 0;
            foreach (StardewValley.Object keg in kegs)
            {
                heldItem = player.ActiveItem as StardewValley.Object;
                if (heldItem is null)
                    break;
                beforeCount = heldItem.Stack;
                bool isValidItem = keg.performObjectDropInAction(heldItem, true, player);
                if (!isValidItem)
                    break;
                bool tryUseKeg = keg.performObjectDropInAction(heldItem, false, player);
                if (beforeCount == heldItem.Stack)
                    break;
            }

            int emptyKegs = 0;
            foreach (StardewValley.Object keg in kegs)
            {
                if (keg.heldObject.Value == null)
                    emptyKegs++;
            }
            return emptyKegs;

        }
        private void resizeCustomMenu(object? sender, WindowResizedEventArgs e)
        {
            if (Game1.activeClickableMenu is ConfirmationMenu)
            {
                Game1.activeClickableMenu = new ConfirmationMenu(
                        GetKegs().Count,
                        () => FillAllKegs(GetKegs()),
                        () => { Game1.exitActiveMenu(); }
                        );
            }
        }
    }
}