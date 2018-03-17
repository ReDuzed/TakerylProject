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
		bool Spacial = false, Blink = false;
		public void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth)
		{
			var modPlayer = Main.LocalPlayer.GetModPlayer<ProjectPlayer>(mod);
			
			SpriteEffects effects = SpriteEffects.None;
			
			Color colorSpecial = default(Color);
			Color newColor = default(Color);
			Color newColor2 = default(Color);
			Color newColor3 = default(Color);
			Color newColor4 = default(Color);
			
			string textSize = "Right Mouse";
			
			int leftOrient = (int)boxCenter.X - 76;
			int verticalOrient = (int)boxCenter.Y;
			
			Player player = Main.LocalPlayer;
			
			for(int i = 0; i < player.armor.Length -1; i++)
			{
				if(player.armor[i].type == mod.ItemType("SpacialAnomaly"))
				{
					Spacial = true;
					break;
				}
				else Spacial = false;
			}
			for(int i = 0; i < player.armor.Length -1; i++)
			{
				if(player.armor[i].type == mod.ItemType("BlinkDagger"))
				{
					Blink = true;
					break;
				}
				else Blink = false;
			}
			
			// Light
			string textSpecial = "Light (Q)";
			if(modPlayer.lightState)
				colorSpecial = Color.LightBlue;
			else if(!modPlayer.lightState)
				colorSpecial = Color.Black;
			spriteBatch.DrawString(Main.fontMouseText, textSpecial, boxCenter + new Vector2(32, 64), colorSpecial, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(textSize), 1f, effects, 0f);
			
			if(Spacial)
			{
				// Single Missile
				if(modPlayer.missileCoolDown > 0)
					newColor = Color.Red;
				else if(modPlayer.missileCoolDown == 0)
					newColor = Color.LightGreen;
				
				string text = "Right Mouse";
				spriteBatch.DrawString(Main.fontMouseText, text, boxCenter + new Vector2(32, -64), newColor, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(textSize), 1f, effects, 0f);
					
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, verticalOrient - 76, (int)100, 4), Color.Black);
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, verticalOrient - 76, (int)modPlayer.missileCoolDown/9, 4), Color.Lerp(Color.Green, Color.Red, (float)modPlayer.missileCoolDown/900));
				
				// Blade Spin
				if(modPlayer.spinCoolDown > 0)
					newColor2 = Color.Red;
				else if(modPlayer.spinCoolDown == 0)
					newColor2 = Color.LightGreen;
				
				string text2 = "Middle Mouse";
				spriteBatch.DrawString(Main.fontMouseText, text2, boxCenter + new Vector2(32, -32), newColor2, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(textSize), 1f, effects, 0f);
				
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y - 44, (int)100, 4), Color.Black);
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y - 44, (int)modPlayer.spinCoolDown/12, 4), Color.Lerp(Color.Green, Color.Red, (float)modPlayer.spinCoolDown/1200));
			}
			if(Blink)
			{
				// Air Strike
				if(modPlayer.strikeCoolDown > 0)
					newColor3 = Color.Red;
				else if(modPlayer.strikeCoolDown == 0)
					newColor3 = Color.LightGreen;
				
				string text3 = "'X' Key";
				spriteBatch.DrawString(Main.fontMouseText, text3, boxCenter + new Vector2(32, 0), newColor3, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(textSize), 1f, effects, 0f);

				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y - 12, (int)100, 4), Color.Black);
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y - 12, (int)modPlayer.strikeCoolDown/27, 4), Color.Lerp(Color.Green, Color.Red, (float)modPlayer.strikeCoolDown/2700));
				
				// Blink
				if(modPlayer.blinkCoolDown > 0)
					newColor4 = Color.Red;
				else if(modPlayer.blinkCoolDown == 0)
					newColor4 = Color.LightGreen;
				
				string text4 = "Left-Shift";
				spriteBatch.DrawString(Main.fontMouseText, text4, boxCenter + new Vector2(32, 32), newColor4, 0f, new Vector2(8,8) + Main.fontMouseText.MeasureString(textSize), 1f, effects, 0f);
				
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y + 20, (int)100, 4), Color.Black);
				spriteBatch.Draw(Main.magicPixel, new Rectangle(leftOrient, (int)boxCenter.Y + 20, (int)modPlayer.blinkCoolDown/9, 4), Color.Lerp(Color.Green, Color.Red, (float)modPlayer.blinkCoolDown/900));
			}
		}
	}
}