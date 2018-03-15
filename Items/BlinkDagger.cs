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
			recipe.AddIngredient(57, 5);	// demonite bar
			recipe.AddIngredient(86, 10);	// shadow scale
			recipe.AddIngredient(2309, 5);	// specular fish
			recipe.AddTile(26);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
		
		int blinkCharge = 0, blinkRange = 64, blinkDust, blinkCoolDown = 0;
		bool canBlink = false, blinked;
		bool facingRight, facingLeft;
		
		int radius, Radius; 
		float degrees = 0, degrees2 = 0, degrees3 = 0;
		float Depreciate = 30, Point;
		
		bool airStrikeUsed = false;
		bool canAirStrike = false, striking = false, midAir = false;
		int strikeDust, strikeCharge, strikeRange = 32, StrikeCoolDown = 0, airStrike;
		float WaveTimer = 0f;
		int ticks = 0;
		
		const float Time = 30;
		const float radians = 0.017f;
		const int defaultBlinkRange = 64;
		const int defaultStrikeRange = 32;
		const int TileSize = 16;
		
		Vector2 dustPos;
		Vector2 center;
		Vector2 OldPosition;
		Vector2 Start, End;
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var modPlayer = player.GetModPlayer<ProjectPlayer>(mod);
			
			int TileX = (int)player.position.X/16;
			int TileY = (int)player.position.Y/16;
			
			#region Blink
			if(modPlayer.blinkCoolDown == 0 && /*!canSpin && */!canBlink && !blinked && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.LeftShift) < 0)
			{
				if(player.controlLeft || player.controlRight)
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
						modPlayer.drawRight = true;
						modPlayer.drawLeft = false;
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
						modPlayer.drawRight = false;
						modPlayer.drawLeft = true;
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
				modPlayer.blinkCoolDown = 900;
				
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
					modPlayer.drawBlink = true;
				}
				else if(facingLeft && Depreciate > 0)
				{
					Depreciate--;
					Point = (Time - Depreciate) / Time;
					player.position = Vector2.Lerp(Start, new Vector2(Start.X - TileSize * blinkRange, Start.Y), Point);
					modPlayer.drawBlink = true;
				}
				if(Depreciate == 0)
				{
					for(int k = 0; k < 360/2; k++)
					{
						radius = 16;
						blinkDust = Dust.NewDust(player.Center, 8, 8, 71, 0f, 0f, 0, Color.White, 1.2f);
						Main.dust[blinkDust].noGravity = true;
						degrees += 0.017f;
						float radCircle = degrees*(k*2);
						Main.dust[blinkDust].velocity.X = (float)(radius*Math.Cos(radCircle));
						Main.dust[blinkDust].velocity.Y = (float)(radius*Math.Sin(radCircle));
					}
					blinked = false;
					modPlayer.drawBlink = false;
					Depreciate = Time;
				}
			}
			if(blinkCoolDown > 0) 
				blinkCoolDown--;
			if(blinkCoolDown == 0) 
				blinked = false;
			#endregion
			
			#region Air Strike
			if(modPlayer.strikeCoolDown == 0 && StrikeCoolDown == 0 && !canAirStrike && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
			{
				if(!player.controlLeft && !player.controlRight)
				{
					strikeCharge++;
				}
				if(strikeCharge == 10 && !striking && !midAir )
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ShineSparkLoop"), player.position);
				for(int k = 0; k < defaultStrikeRange; k++)
				{
					if(CheckUp(TileX, TileY - strikeRange, player))
						strikeRange--;
				}
				strikeCharge = 0;
				canAirStrike = true;
				
				// set positions
				Start = player.position;
				End.X = Start.X;
				End.Y = Start.Y - TileSize * strikeRange;
			}
			else if(StrikeCoolDown > 0 && !canAirStrike && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
			{
				ticks++;
				if(ticks%20 == 0)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/heathurt"), player.position);
			}
			if(canAirStrike && strikeRange >= 32 && !midAir && !striking)
			{
				StrikeCoolDown = 3600;
				
				player.velocity = Vector2.Zero;
				if(strikeCharge == 10 && !striking && !midAir)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ChargeStartup"), player.position);
				if(strikeCharge < 180)
					strikeCharge++;
				if(strikeCharge >= 117)
				{
					striking = true;
					strikeCharge = 0;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/teleport"), Start);
					canAirStrike = false;
				}
				degrees+=0.06f;
				
				radius = 64;
				double X = (radius*Math.Cos(radius*2/(180/Math.PI)));
				double Y = (radius*Math.Sin(radius*2/(180/Math.PI)));
				center = player.position + new Vector2(player.width/2, player.height/2);
				strikeDust = Dust.NewDust(center, 8, 8, 71, 0f, 0f, 0, Color.White, 1.0f);
				Main.dust[strikeDust].noGravity = true;
				Main.dust[strikeDust].position.X = center.X + (float)(radius*Math.Cos(degrees));
				Main.dust[strikeDust].position.Y = center.Y + (float)(radius*Math.Sin(degrees));
				#region points on circle
				if(strikeCharge >= 15)
				{
					int degree45 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree45].noGravity = false;
					Main.dust[degree45].position.X = center.X + (float)(radius*Math.Cos(45));
					Main.dust[degree45].position.Y = center.Y + (float)(radius*Math.Sin(45));
				}
				if(strikeCharge >= 30)
				{
					int degree90 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree90].noGravity = false;
					Main.dust[degree90].position.X = center.X + (float)(radius*Math.Cos(90));
					Main.dust[degree90].position.Y = center.Y + (float)(radius*Math.Sin(90));
				}
				if(strikeCharge >= 45)
				{
					int degree135 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree135].noGravity = false;
					Main.dust[degree135].position.X = center.X + (float)(radius*Math.Cos(135));
					Main.dust[degree135].position.Y = center.Y + (float)(radius*Math.Sin(135));
				}
				if(strikeCharge >= 60)
				{
					int degree180 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree180].noGravity = true;
					Main.dust[degree180].position.X = center.X + (float)(radius*Math.Cos(180));
					Main.dust[degree180].position.Y = center.Y + (float)(radius*Math.Sin(180));
				}
				if(strikeCharge >= 75)
				{
					int degree225 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree225].noGravity = true;
					Main.dust[degree225].position.X = center.X + (float)(radius*Math.Cos(225));
					Main.dust[degree225].position.Y = center.Y + (float)(radius*Math.Sin(225));
				}
				if(strikeCharge >= 90)
				{
					int degree270 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree270].noGravity = true;
					Main.dust[degree270].position.X = center.X + (float)(radius*Math.Cos(270));
					Main.dust[degree270].position.Y = center.Y + (float)(radius*Math.Sin(270));
				}
				if(strikeCharge >= 117)
				{
					int degree315 = Dust.NewDust(player.position + new Vector2((float)X, (float)Y), 8, 8, 71, 0f, 0f, 0, Color.White, 0.5f);
					Main.dust[degree315].noGravity = true;
					Main.dust[degree315].position.X = center.X + (float)(radius*Math.Cos(315));
					Main.dust[degree315].position.Y = center.Y + (float)(radius*Math.Sin(315));
				}
				#endregion
			}
			else if(strikeRange < 32)
			{
				strikeRange = defaultStrikeRange;
				strikeCharge = 0;
				canAirStrike = false;
			}
			if(striking)
			{
				modPlayer.strikeCoolDown = 2700;
				if(Depreciate > 0)
				{
					Depreciate--;
					Point = (Time - Depreciate) / Time;
					player.position = Vector2.Lerp(Start, End, Point);
				}
				if(Depreciate == 0)
				{
					center = player.position + new Vector2(player.width/2, player.height/2);
					
					// need to update dust type
					float radCircle = 0.017f;
					for(int k = 0; k < 360/2; k++)
					{
						int r = 48;
						strikeDust = Dust.NewDust(player.position + new Vector2(player.width/2, player.height/2), 8, 8, 20, 0f, 0f, 0, Color.White, 1f);
						Main.dust[strikeDust].noGravity = true;
						radCircle = 0.017f*(k*2);
						Main.dust[strikeDust].velocity.X = (float)(r*Math.Cos(radCircle));
						Main.dust[strikeDust].velocity.Y = (float)(r*Math.Sin(radCircle));
					}
					for(int l = 90; l < 270; l++)
					{
						int r = 48;
						strikeDust = Dust.NewDust(player.position + new Vector2(player.width/2, player.height/2), 8, 8, 20, 0f, 0f, 0, Color.White, 1f);
						Main.dust[strikeDust].noGravity = true;
						radCircle = 0.017f*(l*2);
						Main.dust[strikeDust].velocity.X = (float)(r*Math.Cos(radCircle));
						Main.dust[strikeDust].velocity.Y = (float)(r*Math.Sin(radCircle));
					}
					radius = 16;
					degrees = radians*45;
					striking = false;
					midAir = true;
					Depreciate = Time;
				}
			}
			if(midAir)
			{
				strikeCharge = 0;
				striking = false;
				canAirStrike = false;
				
				player.velocity = Vector2.Zero;
				
				if(ticks == 0)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/IceBeamChargeShot"), player.position);
				ticks++;
				
				radius = 8;
				degrees += radians/2;
				
				Vector2 updateCenter = player.position + new Vector2(0, player.height/2);
				
				#region
				// the amplitude of the wave
				float Offset = 25.0f;
		
				// 360 degrees in radians
				float Revolution = 6.28308f;

				// how many waves are completed per second
				float WavesPerSecond = 2.0f;

				// get time between updates
				float Time = 1.0f / Main.frameRate;
				
				// increase wave timer
				WaveTimer += Time * Revolution * WavesPerSecond;
				
				float WaveOffset = (float)Math.Sin(WaveTimer) * Offset;
				
				float Angle = (float)Math.Atan2(Main.screenPosition.Y + Main.mouseY - updateCenter.Y,
												Main.screenPosition.X + Main.mouseX - updateCenter.X);
				#endregion
				
				if(ticks%6 == 0)
					airStrike = Projectile.NewProjectile(updateCenter, Vector2.Zero, 134, 64, Main.projectile[134].knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[airStrike].velocity.X = (float)(radius*Math.Cos(Angle));
				Main.projectile[airStrike].velocity.Y = (float)(radius*Math.Sin(Angle));
				Main.projectile[airStrike].position += new Vector2(WaveOffset/2, 0);
				
				if(ticks > 180)
				{
					midAir = false;
					ticks = 0;
				}
			}	
			if(StrikeCoolDown > 0)
				StrikeCoolDown--;
			#endregion
		}
		
		public bool CheckUp(int i, int j, Player player)
		{
			bool Active = Main.tile[i, j-1].active() || Main.tile[i+1, j-1].active();
			bool Solid = Main.tileSolid[Main.tile[i, j-1].type] == true || Main.tileSolid[Main.tile[i+1, j-1].type] == true;
			
			if(Active && Solid) return true;
			else return false;
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