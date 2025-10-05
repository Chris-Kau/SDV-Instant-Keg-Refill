using Instant_Keg;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace YourProjectName
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.WindowResized += this.resizeCustomMenu;
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.Button == SButton.V)
            {
                Farmer player = Game1.player;
                if (Game1.activeClickableMenu is null && Context.IsPlayerFree)
                {
                    Game1.activeClickableMenu = new ConfirmationMenu(
                        GetKegs().Count,
                        () => { FillAllKegs(GetKegs()); },
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

        private void FillAllKegs(List<StardewValley.Object> kegs)
        {
            Farmer player = Game1.player;
            StardewValley.Object? heldItem = player.ActiveItem as StardewValley.Object;
            if (heldItem is null)
                return;
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
        }
        private void resizeCustomMenu(object? sender, WindowResizedEventArgs e)
        {
            if (Game1.activeClickableMenu is ConfirmationMenu)
            {
                Game1.activeClickableMenu = new ConfirmationMenu(
                        GetKegs().Count,
                        () => { FillAllKegs(GetKegs()); },
                        () => { Game1.exitActiveMenu(); }
                        );
            }
        }
    }
}