using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject.Items
{
	public class SpacialAnomaly : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spacial Anomaly");
			Tooltip.SetDefault("Bends special swords to your hand"
					+	"\nHold the 'Middle Mouse Button' to charge"
					+	"\nMust have a 'unique' sword selected on your hotbar");
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
			// recipe 1
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(46);	// recipe for night's edge
			recipe.AddIngredient(121);
			recipe.AddIngredient(155);
			recipe.AddIngredient(190);
			recipe.AddTile(26);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
			// recipe 2
			ModRecipe recipe2 = new ModRecipe(mod);
			recipe2.AddIngredient(675);	// night's edge
			recipe2.AddTile(TileID.CrystalBall);
			recipe2.SetResult(this, 1);
			recipe2.AddRecipe();
		}
		bool light = false;
		int lightDust, lightDust2;
		int coolDown = 0;
			
		int spinCharge = 0, spinDuration = 600;
		bool /*canSpin = false, */spinning = false, charging = false;
		int radius = 72, Radius; 
		float degrees = 0.017f, degrees2 = 3.077f, degrees3 = 0;
		int dmgTicks;
		int soundTicks = 0;
		int ticks = 0;
		
		int blasts;
		float charge = 450f;
		bool active = false;
		
		const float defaultCharge = 450f;
		
		Vector2 dustPos;
		Vector2 center;
		Texture2D swordTexture;
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var modPlayer = player.GetModPlayer<ProjectPlayer>(mod);
			
			ticks++;
			
			#region Missile Attack
			if(ticks >= 90 && Main.mouseRight && !Main.playerInventory)
			{
				ticks = 0;
				active = !active;
			}
			if(charge <= 0)
			{
				modPlayer.missileCoolDown = 900;
				charge = defaultCharge;
			}
			if(!active)
				charge += 0.5f;
			if(active && charge > 0f && modPlayer.missileCoolDown == 0)
			{
				charge--;
				
				Vector2 updateCenter = player.position + new Vector2(0, player.height/2);
				Vector2 mousev = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y );
				
				if(ticks%60 == 0)
				{
					// rocket ID: 134
					blasts = Projectile.NewProjectile(updateCenter, Vector2.Zero, 134, 32 + Main.rand.Next(-16, 8), 4f, player.whoAmI, 0f, 0f);
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Missile"), player.position);
				}
				radius = 4;
				
				float Angle = (float)Math.Atan2(Main.screenPosition.Y + Main.mouseY - updateCenter.Y,
												Main.screenPosition.X + Main.mouseX - updateCenter.X);
				float MouseAngle = (float)Math.Atan2(mousev.Y - Main.projectile[blasts].position.Y, 
													mousev.X - Main.projectile[blasts].position.X);
			
				Main.projectile[blasts].velocity.X += Distance(Main.projectile[blasts], MouseAngle, radius).X;
				Main.projectile[blasts].velocity.Y += Distance(Main.projectile[blasts], MouseAngle, radius).Y;
			}
			#endregion
			
			#region Sword Spin
			Item item = player.inventory[player.selectedItem];
			if(modPlayer.spinCoolDown == 0 && !modPlayer.canSpin && !spinning && Main.mouseMiddle && (item.type == 46 || item.type == 121 || item.type == 155 || item.type == 190 || item.type == 273 ||item.type == 368 || item.type == 484 || item.type == 485 || item.type == 486 || item.type == 675 || item.type == 723 || item.type == 989 || item.type == 1166 || item.type == 1185 || item.type == 1192 || item.type == 1199))
			{
				spinCharge++;
				if(spinCharge >= 60)
				{
					modPlayer.canSpin = true;
					spinCharge = 0;
					spinDuration = 600;
					modPlayer.spinCoolDown = 1200;
				}
				if(spinCharge == 30)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/conjure"), player.position);
			}
			else
			{
				if(spinCharge > 0) 
					spinCharge--;
			}
			if(modPlayer.canSpin)
			{
			//	blinkCharge = 0;
				if(item.type == 46 || item.type == 121 || item.type == 155 || item.type == 190 || item.type == 273 ||item.type == 368 || item.type == 484 || item.type == 485 || item.type == 486 || item.type == 675 || item.type == 723 || item.type == 989 || item.type == 1166 || item.type == 1185 || item.type == 1192 || item.type == 1199)
				{
					spinning = true;
					
					radius = 72;
					degrees3 += 0.06f;
					
					center = player.position + new Vector2(player.width/2, player.height/2);
					
					float PosX = center.X + (float)(radius*Math.Cos(degrees3));
					float PosY = center.Y + (float)(radius*Math.Sin(degrees3));	
					
					Vector2 swordPos = player.bodyPosition + new Vector2(PosX, PosY);
					
					Rectangle swordHitBox = new Rectangle((int)swordPos.X, (int)swordPos.Y, item.width, item.height);
					NPC[] npc = Main.npc;
					for(int k = 0; k < npc.Length-1; k++)
					{
						NPC n = npc[k];
						Vector2 npcv = new Vector2(n.position.X, n.position.Y);
						Rectangle npcBox = new Rectangle((int)npcv.X, (int)npcv.Y, n.width, n.height);
						if(n.active && !n.friendly && !n.dontTakeDamage 
						&& dmgTicks == 0 && swordHitBox.Intersects(npcBox))
						{
							n.StrikeNPC((int)(item.damage + player.meleeDamage), item.knockBack*3, player.direction,false,false);
							dmgTicks = 10;
						}
					}
					Projectile[] projectile = Main.projectile;
					for(int l = 0; l < projectile.Length-1; l++)
					{
						Projectile n = projectile[l];
						Vector2 projv = new Vector2(n.position.X, n.position.Y);
						Rectangle projBox = new Rectangle((int)projv.X, (int)projv.Y, n.width, n.height);
						if(n.active && swordHitBox.Intersects(projBox))
						{
							n.timeLeft = 0;
						}
					}
					
					soundTicks++;
					if(soundTicks%16 == 0)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SpinQuiet"), player.bodyPosition + new Vector2(PosX, PosY));
				}
				else 
				{
					spinning = false;
				}
				if(spinDuration > 0) 
					spinDuration--;
				if(spinDuration == 0)
				{
					degrees3 = 0f;
					modPlayer.canSpin = false;
					spinning = false;
				}
			}
			if(dmgTicks > 0) 
				dmgTicks--;
			#endregion
			
			#region Light
			int type = 0;
			if(coolDown == 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Q) < 0)
			{
				modPlayer.lightState = !modPlayer.lightState;
				
				light = !light;
				coolDown = 60;
			}
			if(light)
			{
				degrees += 0.017f * 6; // 1 degree in radians multiplied by desired degree
				
				radius = 90;
				
				if(modPlayer.angel)
					type = 20;
				else if(modPlayer.demon)
					type = 35;
				else type = 6;
				
				center = player.position + new Vector2(player.width/2, player.height/2);
				// dust 1
				lightDust = Dust.NewDust(player.Center, 8, 8, type, 0f, 0f, 0, Color.White, 1f);
				Main.dust[lightDust].noGravity = true;
				Main.dust[lightDust].position.X = center.X + (float)(radius*Math.Cos(degrees));
				Main.dust[lightDust].position.Y = center.Y + (float)(radius*Math.Sin(degrees));
				// dust 2
				lightDust2 = Dust.NewDust(player.Center, 8, 8, type, 0f, 0f, 0, Color.White, 1f);
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
	}
}