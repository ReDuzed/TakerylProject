using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace TakerylProject
{
	public class ProjectWorld : ModWorld
	{
		static Vector2 boxOrigin;
		static Vector2 boxSize;
		static Vector2 boxCenter;
		// Draw before players, npcs, or projectiles
		public override void PostDrawTiles()
		{
			boxOrigin = new Vector2(32, 160);
			boxSize = new Vector2(40, 40);
			boxCenter = new Vector2(96, 224);
			
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			
			if(!Main.playerInventory)
				DrawBorderedRect(Main.spriteBatch, Color.LightGreen * 0.1f, Color.LightGreen * 0.3f, boxOrigin, boxSize, 4);
		
			Main.spriteBatch.End();
		}
		
		public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth)
		{
		//	Box
		//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
		//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
		//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
		//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
		//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + (int)size.X, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
			Color newColor = default(Color);
			
			spriteBatch.DrawString(Main.fontMouseText, "Left-Shift", boxCenter, newColor, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(text), 1f, effects, 0f);
		}
	}
}