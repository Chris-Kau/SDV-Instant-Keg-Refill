using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Instant_Keg
{
    public class ConfirmationMenu : IClickableMenu
    {
        const int UIWidth = 550;
        const int UIHeight = 325;
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
        static string baseDescription;
        static int totalKegs;
        ClickableComponent description;
        static ClickableTextureComponent? confirmBTN;
        static ClickableTextureComponent? cancelBTN;
        Rectangle confirmBTNRect;
        Rectangle cancelBTNRect;
        Func<int> onConfirm;
        Action onCancel;

        public ConfirmationMenu(int tk, Func<int> onconfirm, Action oncancel)
        {
            totalKegs = tk;
            onConfirm = onconfirm;
            onCancel = oncancel;
            xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
            yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
            baseDescription = $"Attempt to fill {totalKegs} EMPTY\nkegs in this location?";
            description = new ClickableComponent(new Rectangle(xPos + (UIWidth / 2) - (UIWidth - 400 - 10), yPos + 108, UIWidth - 400, 64), baseDescription);


            Texture2D tex = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            int btnSize = 64;
            int marginX = 150;
            int baseY = yPos + UIHeight - 105;

            confirmBTN = new ClickableTextureComponent(
                new Rectangle(xPos + marginX, baseY, btnSize, btnSize),
                tex,
                new Rectangle(128, 256, 64, 64),
                1f
            );

            cancelBTN = new ClickableTextureComponent(
                new Rectangle(xPos + UIWidth - btnSize - marginX, baseY, btnSize, btnSize),
                tex,
                new Rectangle(192, 256, 64, 64),
                1f
            );
            confirmBTNRect = new Rectangle(confirmBTN.bounds.X, confirmBTN.bounds.Y, confirmBTN.bounds.Width, confirmBTN.bounds.Height);
            cancelBTNRect = new Rectangle(cancelBTN.bounds.X, cancelBTN.bounds.Y, cancelBTN.bounds.Width, cancelBTN.bounds.Height);
        }

        private void scaleTransition(ClickableTextureComponent icon, float scaleResult, float delta)
        {
            //if delta > 0, that means we want to scale up, otherwise scale down
            if (delta > 0)
            {
                if (icon.scale < scaleResult)
                    icon.scale += delta;
                else
                    icon.scale = scaleResult;
            }
            else
            {
                if (icon.scale > scaleResult)
                    icon.scale += delta;
                else
                    icon.scale = scaleResult;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (confirmBTN is null || cancelBTN is null)
                return;
            if (confirmBTNRect.Contains(x, y))
                scaleTransition(confirmBTN, 1.05f, 0.08f);
            else
                scaleTransition(confirmBTN, 1f, 0.08f);

            if (cancelBTNRect.Contains(x, y))
                scaleTransition(cancelBTN, 1.05f, 0.08f);
            else
                scaleTransition(cancelBTN, 1f, 0.08f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if(confirmBTN is null || cancelBTN is null || onConfirm is null || onCancel is null) return;
            if (confirmBTNRect.Contains(x, y))
            {
                Game1.playSound("select");
                int remaining = onConfirm.Invoke();
                if (remaining > 0)
                {
                    baseDescription = $"Attempt to fill {remaining} EMPTY\nkegs in this location?";
                }
                description.name = $"{baseDescription}\nFilled {totalKegs - remaining} of {totalKegs} keg(s).";

            }
            if (cancelBTNRect.Contains(x,y))
            {
                Game1.playSound("select");
                onCancel.Invoke(); 
            }

        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);

            string[] lines = description.name.Split('\n');
            float y = description.bounds.Y;
            foreach (string line in lines)
            {
                Vector2 size = Game1.smallFont.MeasureString(line);

                float xCentered = xPos + (UIWidth - size.X) / 2;

                Utility.drawTextWithShadow(b, line, Game1.smallFont, new Vector2(xCentered, y), Color.Black);

                y += size.Y + 3f;
            }
            confirmBTN?.draw(b);
            cancelBTN?.draw(b);
            drawMouse(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {

            xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
            yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
        }
    }
}
