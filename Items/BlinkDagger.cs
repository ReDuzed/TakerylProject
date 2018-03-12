using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject.Items
{
	public class BlinkDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blink Dagger");
			Tooltip.SetDefault("Blink through space"
					+	"\nHold 'Shift' and 'Move Left or Right' to charge");
		}
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.maxStack = 1;
			item.value = 50000;
			item.rare = 5;
			item.scale = 1f;
			item.accessory = true;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(9);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
		
		int blinkCharge = 0, blinkRange = 64, blinkDust, blinkCoolDown = 0;
		int radius, Radius; 
		float degrees = 0, degrees2 = 0, degrees3 = 0;
		bool canBlink = false, blinked;
		bool facingRight, facingLeft;
		float Depreciate = 30, Point;
		const float Time = 30;
		const int defaultBlinkRange = 64;
		const int TileSize = 16;
		Vector2 dustPos;
		Vector2 center;
		Vector2 OldPosition;
		Vector2 Start, End;
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			int TileX = (int)player.position.X/16;
			int TileY = (int)player.position.Y/16;
			if(/*!canSpin && */!canBlink && !blinked && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.LeftShift) < 0)
			{
				if((player.controlLeft || player.controlRight))
				{
					blinkCharge++;
					if(blinkCharge == 1 || blinkCharge == 60)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ShineSparkLoop"), player.position);
					}
				}
				else
				{
					if(blinkCharge > 0)
						blinkCharge--;
				}
				if(blinkCharge >= 117)
				{
					if(player.direction == 1)
					{
						for(int k = 0; k < defaultBlinkRange; k++)
						{
							if(CheckRight(TileX + blinkRange, TileY, player))
								blinkRange -= 1;
						}
						facingRight = true;
						facingLeft = false;
					}
					else if(player.direction == -1)
					{
						for(int k = 0; k < defaultBlinkRange; k++)
						{
							if(CheckLeft(TileX - blinkRange, TileY, player))
								blinkRange -= 1;
						}
						facingRight = false;
						facingLeft = true;
					}
					if(facingLeft && !CheckLeft(TileX - blinkRange, TileY, player) 
					|| facingRight && !CheckRight(TileX + blinkRange, TileY, player))
					{
						canBlink = true;
						blinkCharge = 0;
					}
					else 
					{
						blinkRange = defaultBlinkRange;
						blinkCharge = 0;
					}
				}
				// set start position
				Start.X = player.position.X;
				Start.Y = player.position.Y;
				
				// set end position
				End.X = Start.X + TileSize * blinkRange;
				End.Y = Start.Y;
			}
			if(canBlink)
			{
				player.velocity = Vector2.Zero;
				if(blinkCharge == 0)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ChargeStartup"), player.position);
				}
				blinkCharge++;
				degrees+=0.06f;
				
				radius = 64;
				double X = (radius*Math.Cos(radius*2/(180/Math.PI)));
				double Y = (radius*Math.Sin(radius*2/(180/Math.PI)));
				center = player.position + new Vector2(player.width/2, player.height/2);
				blinkDust = Dust.NewDust(center, 8, 8, 71, 0f, 0f, 0, Color.White, 1.0f);
				Main.dust[blinkDust].noGravity = true;
				Main.dust[blinkDust].position.X = center.X + (float)(radius*Math.Cos(degrees));
				Main.dust[blinkDust].position.Y = center.Y + (float)(radius*Math.Sin(degrees));
				#region points on circle
				if(blinkCharge >= 15)
				{
					int degree45 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree45].noGravity = false;
					Main.dust[degree45].position.X = center.X + (float)(radius*Math.Cos(45));
					Main.dust[degree45].position.Y = center.Y + (float)(radius*Math.Sin(45));
				}
				if(blinkCharge >= 30)
				{
					int degree90 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree90].noGravity = false;
					Main.dust[degree90].position.X = center.X + (float)(radius*Math.Cos(90));
					Main.dust[degree90].position.Y = center.Y + (float)(radius*Math.Sin(90));
				}
				if(blinkCharge >= 45)
				{
					int degree135 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree135].noGravity = false;
					Main.dust[degree135].position.X = center.X + (float)(radius*Math.Cos(135));
					Main.dust[degree135].position.Y = center.Y + (float)(radius*Math.Sin(135));
				}
				if(blinkCharge >= 60)
				{
					int degree180 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree180].noGravity = true;
					Main.dust[degree180].position.X = center.X + (float)(radius*Math.Cos(180));
					Main.dust[degree180].position.Y = center.Y + (float)(radius*Math.Sin(180));
				}
				if(blinkCharge >= 75)
				{
					int degree225 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree225].noGravity = true;
					Main.dust[degree225].position.X = center.X + (float)(radius*Math.Cos(225));
					Main.dust[degree225].position.Y = center.Y + (float)(radius*Math.Sin(225));
				}
				if(blinkCharge >= 90)
				{
					int degree270 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree270].noGravity = true;
					Main.dust[degree270].position.X = center.X + (float)(radius*Math.Cos(270));
					Main.dust[degree270].position.Y = center.Y + (float)(radius*Math.Sin(270));
				}
				if(blinkCharge >= 117)
				{
					int degree315 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree315].noGravity = true;
					Main.dust[degree315].position.X = center.X + (float)(radius*Math.Cos(315));
					Main.dust[degree315].position.Y = center.Y + (float)(radius*Math.Sin(315));
				}
				#endregion
				if(blinkCharge >= 117)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/teleport"), player.oldPosition);
					
					blinkRange = defaultBlinkRange;
					blinkCharge = 0;
					blinkCoolDown = 360;
					degrees = 0;
					canBlink = false;
					blinked = true;
				}
			}
			if(blinked)
			{
				if(facingRight && Depreciate > 0)
				{
					Depreciate--;
					Point = (Time - Depreciate) / Time;
					player.position = Vector2.Lerp(Start, End, Point);
				}
				else if(facingLeft && Depreciate > 0)
				{
					Depreciate--;
					Point = (Time - Depreciate) / Time;
					player.position = Vector2.Lerp(Start, new Vector2(Start.X - TileSize * blinkRange, Start.Y), Point);
				}
				if(Depreciate == 0)
				{
					for(int k = 0; k < 360/2; k++)
					{
						radius = 16;
						blinkDust = Dust.NewDust(player.Center, 8, 8, 71, 0f, 0f, 0, Color.White, 1.2f);
						Main.dust[blinkDust].noGravity = true;
						Main.dust[blinkDust].velocity.X = (float)(radius*Math.Cos(k));
						Main.dust[blinkDust].velocity.Y = (float)(radius*Math.Sin(k));
					}
					blinked = false;
					Depreciate = Time;
				}
			}
			if(blinkCoolDown > 0) 
				blinkCoolDown--;
			if(blinkCoolDown == 0) 
				blinked = false;
		}
		public bool CheckLeft(int i, int j, Player player)
		{
			bool Active = Main.tile[i-1, j].active() || Main.tile[i-1, j+1].active() || Main.tile[i-1, j +2].active();
			bool Solid = Main.tileSolid[Main.tile[i-1, j].type] == true || Main.tileSolid[Main.tile[i-1, j+1].type] == true || Main.tileSolid[Main.tile[i-1, j+2].type] == true;
			
			if(Active && Solid) return true;
			else return false;
		}
		public bool CheckRight(int i, int j, Player player)
		{
			bool Active = (Main.tile[i+1, j].active() || Main.tile[i+1, j+1].active() || Main.tile[i+1, j +2].active());
			bool Solid = ((Main.tileSolid[Main.tile[i+1, j].type] == true) || (Main.tileSolid[Main.tile[i+1, j+1].type] == true) || (Main.tileSolid[Main.tile[i+1, j+2].type] == true));
			
			if(Active && Solid) return true;
			else return false;
		}
	}
}