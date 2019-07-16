using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;

using TakerylProject.Projectiles;

namespace TakerylProject
{
	public class ProjectPlayer : ModPlayer
	{
		public bool canSpin = false;
		public bool drawBlink = false;
		public bool drawLeft, drawRight;
		public bool angel, demon, spacePirate;
		public bool lightState = false;
		public bool disabled = true;
		public int 
			blinkCoolDown = 0, spinCoolDown = 0, 
			strikeCoolDown = 0, missileCoolDown = 0;
		
	/*	public override TagCompound Save()
		{
			return new TagCompound {
				{"allegiance", angel} ,
				{"_allegiance", demon}
			};
		}

		public override void Load(TagCompound tag)
		{
			angel = tag.GetBool("allegiance");
			demon = tag.GetBool("_allegiance");
		}	*/
		private bool init = true;
		public override void PreUpdate()
		{
			#region debug
			if (!init && player.active)
			{
				player.inventory[6].SetDefaults(mod.ItemType<Items.AngelicGrace>());
				player.inventory[7].SetDefaults(mod.ItemType<Items.DemonicDeal>());
				player.inventory[8].SetDefaults(mod.ItemType<Items.BlinkDagger>());
				player.inventory[9].SetDefaults(mod.ItemType<Items.SpacialAnomaly>());
				init = true;
			}
			#endregion
			if (KeyPress(Keys.F))
			{
				for (int i = 0; i < SwordID.swordTypes.Length; i++)
				{	
					if (player.HeldItem.type == SwordID.swordTypes[i])
					{	
						float speed = 8f;
						Projectile.NewProjectile(player.Center, new Vector2(speed * (float)Math.Cos(player.AngleTo(Main.MouseWorld)), speed * (float)Math.Sin(player.AngleTo(Main.MouseWorld))), mod.ProjectileType<Projectiles.ThrownSword>(), 30, 5f, player.whoAmI, ThrownSword.AI_Embed4All, player.HeldItem.type);
						player.HeldItem.type = ItemID.None;
						break;
					}
				}
			}
			if(angel || demon)
				player.noFallDmg = true;
			#region actions
			if(missileCoolDown > 0)
				missileCoolDown--;
			if(spinCoolDown > 0)
				spinCoolDown--;
			if(blinkCoolDown > 0)
				blinkCoolDown--;
			if(strikeCoolDown > 0)
				strikeCoolDown--;
			#endregion
		}
		
		int animate = 0, frameHeight = 52;
		float degrees3 = 0;
		Texture2D swordTexture;
		Texture2D blinkTexture;
		Texture2D GemTexture;
		Texture2D WingsTex;
		public override void DrawEffects(PlayerDrawInfo drawinfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			PlayerHeadDrawInfo drawInfo = new PlayerHeadDrawInfo();
			drawInfo.spriteBatch = Main.spriteBatch;
			drawInfo.drawPlayer = player;
			
			int radius = 72;
			if(/*thrown || */canSpin)
				degrees3 += 0.06f; // 0.017f = 1 degree displayed in radians
			else degrees3 = 0;
			
		//	CurrentPoint = (Time - Depreciating) / Time;
			
			SpriteEffects effects = SpriteEffects.None;
			Color color = player.GetImmuneAlpha(Lighting.GetColor((int)((double)player.position.X + (double)player.width * 0.5) / 16, (int)(((double)player.position.Y + (double)player.height * 0.25) / 16.0), Color.White), 0f);
			Vector2 Position = player.position;
			Vector2 origin = new Vector2((float)player.legFrame.Width * 0.5f, (float)player.legFrame.Height * 0.5f);
			Vector2 bodyPosition = new Vector2((float)((int)(player.position.X - Main.screenPosition.X - (float)(player.bodyFrame.Width / 2) + (float)(player.width / 2))), (float)((int)(player.position.Y - Main.screenPosition.Y + (float)player.height - (float)player.bodyFrame.Height + 4f)));
			Vector2 wingsPosition = new Vector2((float)((int)(Position.X - Main.screenPosition.X + (float)(player.width / 2) - (float)(9 * player.direction)) + 0 * player.direction), (float)((int)(Position.Y - Main.screenPosition.Y + (float)(player.height / 2) + 2f * player.gravDir + (float)24 * player.gravDir)));

      		float MoveX = origin.X + (float)(radius*Math.Cos(degrees3));
			float MoveY = origin.Y + (float)(radius*Math.Sin(degrees3));
			
			Item item = player.inventory[player.selectedItem];
			for (int i = 0; i < SwordID.swordTypes.Length; i++) 
			{
				if (item.type == SwordID.swordTypes[i])
				{
					swordTexture = Main.itemTexture[item.type];
					break;
				}
			}
			if(canSpin)
			{
				Main.spriteBatch.Draw(swordTexture, 
					bodyPosition + player.bodyPosition + new Vector2(MoveX, MoveY), 
					null, Color.White, (degrees3*3)*(-1) + 0.48f, origin, 1f, effects, 0f);
			}
			if(drawBlink)
			{
				animate++;
				
				if(animate == 9)
					animate = 0;
				if(animate == 0)
					frameHeight = 1;
				else
					frameHeight = 52;
				blinkTexture = mod.GetTexture("Gores/ShineSpark");
				if(drawRight)
					Main.spriteBatch.Draw(blinkTexture, 
						bodyPosition + player.bodyPosition - new Vector2((player.width)*(-1), (player.height/2 + 4)*(-1)),
						new Rectangle(0, animate * frameHeight, 50, 52), Color.White, 1.57f, origin, 1.2f, effects, 0f);
				else if(drawLeft)
				{
					effects = SpriteEffects.FlipVertically;
					Main.spriteBatch.Draw(blinkTexture, 
						bodyPosition + player.bodyPosition - new Vector2(-8, (player.height/2 + 4)*(-1)),
						new Rectangle(0, animate * frameHeight, 50, 52), Color.White, 1.57f, origin, 1.2f, effects, 0f);
				}
			}
			if(angel)
			{
				GemTexture = Main.gemTexture[1];
				if(GemTexture != null)
				Main.spriteBatch.Draw(GemTexture, 
					bodyPosition + player.bodyPosition + new Vector2(player.width + 2, -16), 
					null, color, 0f, origin, 1f, effects, 0f);
				player.wings = WingID.Angel;
			}
			else if(demon)
			{
				GemTexture = Main.gemTexture[4];
				if(GemTexture != null)
				Main.spriteBatch.Draw(GemTexture, 
					bodyPosition + player.bodyPosition + new Vector2(player.width + 4, -16), 
					null, color, 0f, origin, 1f, effects, 0f);
				player.wings = WingID.Demon;
			}
			#region debug
			if (!disabled)
			{
				try
				{
					if (KeyPress(Keys.Up) && WingID.Selected > 0)
						WingID.Selected--;
					if (KeyPress(Keys.Down))
						WingID.Selected++;
					if (KeyPress(Keys.F1) && !disabled)
					{
						int[] wings = new int[] { ItemID.AngelWings, ItemID.DemonWings };
						for (int i = 0; i < 2; i++)
							Item.NewItem(player.Center, Vector2.Zero, wings[i]);
					}
					if (KeyPress(Keys.F2))
						Main.NewText(WingID.Selected);
					player.wings = WingID.Selected;
				}
				catch 
				{
					return;
				}
			}
			#endregion
		}
		public static bool KeyPress(Keys key)
		{
			return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
		}
	}
}