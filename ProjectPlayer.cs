using System;
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

namespace TakerylProject
{
	public class ProjectPlayer : ModPlayer
	{
		int blinkCharge = 0, blinkRange = 32, blinkDust, blinkCoolDown = 0;
		int radius, degrees = 360;
		bool canBlink = false, blinked;
		bool facingRight, facingLeft;
		const int defaultBlinkRange = 32;
		const int TileSize = 16;
		Vector2 dustPos;
		public override void PreUpdate()
		{
			int TileX = (int)player.position.X/16;
			int TileY = (int)player.position.Y/16;
			Main.NewText("Blink Charge: " +blinkCharge,200,150,100);
			if(!canBlink && !blinked && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.LeftShift) < 0)
			{
				if((player.controlLeft || player.controlRight))
				{
					blinkCharge++;
				}
				else
				{
					if(blinkCharge > 0)
						blinkCharge--;
				}
				if(blinkCharge == 1 || blinkCharge == 60)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ShineSparkLoop"), player.position);
				}
				if(blinkCharge >= 117)
				{
					if(player.direction == 1)
					{
						for(int k = 0; k < blinkRange; k++)
						{
							if(CheckRight(TileX + blinkRange, TileY, player))
								blinkRange -= 1;
						}
						facingRight = true;
						facingLeft = false;
					}
					else if(player.direction == -1)
					{
						for(int k = 0; k < blinkRange; k++)
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
			}
			if(canBlink)
			{
				player.velocity = Vector2.Zero;
				if(blinkCharge == 0)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ChargeStartup"), player.position);
				}
				blinkCharge++;
				
				radius = 8;
				double X = (radius*Math.Cos(radius*2/(180/Math.PI)));
				double Y = (radius*Math.Sin(radius*2/(180/Math.PI)));
				blinkDust = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 1.2f);
				Main.dust[blinkDust].noGravity = true;
				Main.dust[blinkDust].velocity.X = (float)(radius*Math.Cos(blinkCharge));
				Main.dust[blinkDust].velocity.Y = (float)(radius*Math.Sin(blinkCharge));
				
				if(blinkCharge >= 117)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/teleport"), player.oldPosition);
					if(facingRight)
					{
						player.position.X += TileSize*blinkRange;
						facingRight = false;
					}
					else if(facingLeft)
					{
						player.position.X -= TileSize*blinkRange;
						facingLeft = false;
					}
					for(int k = 0; k < degrees/2; k++)
					{
						blinkDust = Dust.NewDust(player.Center, 8, 8, 71, 0f, 0f, 0, Color.White, 1.2f);
						Main.dust[blinkDust].noGravity = true;
						Main.dust[blinkDust].velocity.X = (float)(radius*Math.Cos(k));
						Main.dust[blinkDust].velocity.Y = (float)(radius*Math.Sin(k));
					}
					blinkRange = defaultBlinkRange;
					blinkCharge = 0;
					blinkCoolDown = 360;
					canBlink = false;
					blinked = true;
				}
			}
			if(blinkCoolDown > 0) blinkCoolDown--;
			if(blinkCoolDown == 0) blinked = false;
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