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

namespace TakerylProject
{
	public class ProjectPlayer : ModPlayer
	{
		public bool canSpin = false;
		public bool drawBlink = false;
		public bool drawLeft, drawRight;
		public bool angel, demon, spacePirate;
		bool light = false;
		bool grow = true;
		int lightDust, lightDust2;
		int coolDown = 0;
		int radius = 48;
		float degrees = 0.017f, degrees2 = 3.077f;
		bool airStrikeUsed = false;
		bool canAirStrike = false, striking = false, midAir = false;
		int strikeDust, strikeCharge, strikeRange = 32, strikeCoolDown = 0, airStrike;
		float Depreciate = 40, Point;
		int ticks = 0;
		const int defaultStrikeRange = 32;
		float WaveTimer = 0f;
		int blasts;
		int charge = 0;
		bool active = false;
		const float Time = 40;
		const int TileSize = 16;
		const float radians = 0.017f;
		Vector2 center;
		Vector2 Start, End;
		Rectangle mouse;
		Rectangle projBox;
		public override void PreUpdate()
		{
			if(angel || demon)
				player.noFallDmg = true;

			int TileX = (int)player.position.X/16;
			int TileY = (int)player.position.Y/16;
			
			if(charge == 0 && Main.mouseRight)
			{
				charge = 180;
				active = !active;
			}
			if(charge > 0)
				charge--;
			if(active)
			{
				Vector2 updateCenter = player.position + new Vector2(0, player.height/2);
				Vector2 mousev = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y );
				
				if(ticks%60 == 0)
					// rocket ID: 134
					blasts = Projectile.NewProjectile(updateCenter, Vector2.Zero, 134, 32 + Main.rand.Next(-16, 8), 4f, player.whoAmI, 0f, 0f);
				
				ticks++;
				degrees += radians;
				radius = 4;
				
				float Angle = (float)Math.Atan2(Main.screenPosition.Y + Main.mouseY - updateCenter.Y,
												Main.screenPosition.X + Main.mouseX - updateCenter.X);
				float MouseAngle = (float)Math.Atan2(mousev.Y - Main.projectile[blasts].position.Y, 
													mousev.X - Main.projectile[blasts].position.X);
			
				Main.projectile[blasts].velocity.X += Distance(Main.projectile[blasts], MouseAngle, radius).X;
				Main.projectile[blasts].velocity.Y += Distance(Main.projectile[blasts], MouseAngle, radius).Y;
			}
			#region Air Strike
			if(strikeCoolDown == 0 && !canAirStrike && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
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
			else if(strikeCoolDown > 0 && !canAirStrike && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
			{
				ticks++;
				if(ticks%20 == 0)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/heathurt"), player.position);
			}
			if(canAirStrike && strikeRange >= 32 && !midAir && !striking)
			{
				strikeCoolDown = 3600;
				
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
			if(strikeCoolDown > 0)
				strikeCoolDown--;
			#endregion
			#region Light
			if(!light && coolDown == 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Q) < 0)
			{
				light = !light;
				coolDown = 300;
			}
			if(light)
			{
				degrees += 0.017f * 6; // 1 degree in radians multiplied by desired degree
				
				radius = 90;
				
				center = player.position + new Vector2(player.width/2, player.height/2);
				// dust 1
				lightDust = Dust.NewDust(player.Center, 8, 8, 20, 0f, 0f, 0, Color.White, 1f);
				Main.dust[lightDust].noGravity = true;
				Main.dust[lightDust].position.X = center.X + (float)(radius*Math.Cos(degrees));
				Main.dust[lightDust].position.Y = center.Y + (float)(radius*Math.Sin(degrees));
				// dust 2
				lightDust2 = Dust.NewDust(player.Center, 8, 8, 20, 0f, 0f, 0, Color.White, 1f);
				Main.dust[lightDust2].noGravity = true;
				Main.dust[lightDust2].position.X = center.X + (float)(radius*Math.Cos(degrees + (0.017f*180)));
				Main.dust[lightDust2].position.Y = center.Y + (float)(radius*Math.Sin(degrees + (0.017f*180)));
			}
			if(coolDown > 0)
				coolDown--;
			#endregion
		}
		public Vector2 Distance(Projectile projectile, float Angle, float Radius)
		{
		float VelocityX = (float)(Radius*Math.Cos(Angle));
		float VelocityY = (float)(Radius*Math.Sin(Angle));
			
			if(projectile.velocity.X > 6f) 
				projectile.velocity.X -= 2f;
			if(projectile.velocity.Y > 6f) 
				projectile.velocity.Y -= 2f;
			
			return new Vector2(VelocityX, VelocityY);
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
		int animate = 0, frameHeight = 52;
		float degrees3 = 0;
		Texture2D swordTexture;
		Texture2D blinkTexture;
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
			Vector2 origin = new Vector2((float)player.legFrame.Width * 0.5f, (float)player.legFrame.Height * 0.5f);
			Vector2 bodyPosition = new Vector2((float)((int)(player.position.X - Main.screenPosition.X - (float)(player.bodyFrame.Width / 2) + (float)(player.width / 2))), (float)((int)(player.position.Y - Main.screenPosition.Y + (float)player.height - (float)player.bodyFrame.Height + 4f)));
			Vector2 wingsPosition = new Vector2((float)((int)(player.position.X - Main.screenPosition.X + (float)(player.width / 2) - (float)(9 * player.direction))), (float)((int)(player.position.Y - Main.screenPosition.Y + (float)(player.height / 2) + 2f * player.gravDir)));
			float MoveX = origin.X + (float)(radius*Math.Cos(degrees3));
			float MoveY = origin.Y + (float)(radius*Math.Sin(degrees3));
			
			Item item = player.inventory[player.selectedItem];
			#region sword textures
			if(item.type == 46) // light's bane
				swordTexture = Main.itemTexture[46];
			if(item.type == 121) // fiery greatsword
				swordTexture = Main.itemTexture[121];
			if(item.type == 155) // muramasa
				swordTexture = Main.itemTexture[155];
			if(item.type == 190) // blade of grass
				swordTexture = Main.itemTexture[190];
			if(item.type == 273) // night's edge
				swordTexture = Main.itemTexture[273];
			if(item.type == 368) // excalibur
				swordTexture = Main.itemTexture[368];
			if(item.type == 484) // adamantite sword
				swordTexture = Main.itemTexture[484];
			if(item.type == 485) // cobalt sword
				swordTexture = Main.itemTexture[485];
			if(item.type == 486) // mythril sword
				swordTexture = Main.itemTexture[486];
			if(item.type == 675) // true night's edge
				swordTexture = Main.itemTexture[675];
			if(item.type == 723) // beam sword
				swordTexture = Main.itemTexture[723];
			if(item.type == 989) // enchanted sword
				swordTexture = Main.itemTexture[989];
			if(item.type == 1166) // bone sword
				swordTexture = Main.itemTexture[1166];
			if(item.type == 1185) // palladium sword
				swordTexture = Main.itemTexture[1185];
			if(item.type == 1192) // orichalcum sword
				swordTexture = Main.itemTexture[1192];
			if(item.type == 1199) // titanium sword
				swordTexture = Main.itemTexture[1199];
			#endregion
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
				if(player.direction == 1)
					Main.spriteBatch.Draw(Main.itemTexture[493], 
						wingsPosition, 
						new Rectangle?(new Rectangle(0, Main.itemTexture[493].Height / 4 * player.wingFrame, Main.itemTexture[493].Width, Main.itemTexture[493].Height / 4)), 
						Color.White, 0f, new Vector2((float)(Main.itemTexture[493].Width / 2), (float)(Main.itemTexture[493].Height / 8)), 1f, effects, 0f);
				else
					effects = SpriteEffects.FlipHorizontally;
					Main.spriteBatch.Draw(Main.itemTexture[493], 
						wingsPosition, 
						new Rectangle?(new Rectangle(0, Main.itemTexture[493].Height / 4 * player.wingFrame, Main.itemTexture[493].Width, Main.itemTexture[493].Height / 4)), 
						Color.White, 0f, new Vector2((float)(Main.itemTexture[493].Width / 2), (float)(Main.itemTexture[493].Height / 8)), 1f, effects, 0f);
			}
			else if(demon)
			{
				if(player.direction == 1)
					Main.spriteBatch.Draw(Main.itemTexture[492], 
						wingsPosition, 
						new Rectangle?(new Rectangle(0, Main.itemTexture[492].Height / 4 * player.wingFrame, Main.itemTexture[492].Width, Main.itemTexture[492].Height / 4)), 
						Color.White, 0f, new Vector2((float)(Main.itemTexture[492].Width / 2), (float)(Main.itemTexture[492].Height / 8)), 1f, effects, 0f);
				else
					effects = SpriteEffects.FlipHorizontally;
					Main.spriteBatch.Draw(Main.itemTexture[492], 
						wingsPosition, 
						new Rectangle?(new Rectangle(0, Main.itemTexture[492].Height / 4 * player.wingFrame, Main.itemTexture[492].Width, Main.itemTexture[492].Height / 4)), 
						Color.White, 0f, new Vector2((float)(Main.itemTexture[492].Width / 2), (float)(Main.itemTexture[492].Height / 8)), 1f, effects, 0f);
			}
		}
	}
}