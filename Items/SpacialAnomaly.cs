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
			Tooltip.SetDefault("Bends swords to your hand"
					+	"\nHold the 'Middle Mouse Button' to charge"
					+	"\nMust have a sword selected on your hotbar");
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
		
		int spinCharge = 0, spinDuration = 600;
		bool /*modPlayer.canSpin = false, */spinning = false, charging = false;
		int radius = 72, Radius; 
		float degrees = 0, degrees2 = 0, degrees3 = 0;
		int dmgTicks;
		int soundTicks = 0;
		Vector2 dustPos;
		Vector2 center;
		Texture2D swordTexture;
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var modPlayer = player.GetModPlayer<ProjectPlayer>(mod);
			
			Item item = player.inventory[player.selectedItem];
			if(!modPlayer.canSpin && !spinning && Main.mouseMiddle && item.type == 368)
			{
				spinCharge++;
				if(spinCharge >= 60)
				{
					modPlayer.canSpin = true;
					spinCharge = 0;
					spinDuration = 600;
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
				if(item.type == 368)
				{
					spinning = true;
					
					radius = 72;
					degrees2 += 0.06f;
					
					center = player.position + new Vector2(player.width/2, player.height/2);
					
					float PosX = center.X + (float)(radius*Math.Cos(degrees2));
					float PosY = center.Y + (float)(radius*Math.Sin(degrees2));	
					
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
							n.StrikeNPC((int)(item.damage + player.meleeDamage), item.knockBack*3, player.direction*(-1),false,false);
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
				if(spinDuration > 0) 
					spinDuration--;
				if(spinDuration == 0)
				{
					degrees2 = 0f;
					modPlayer.canSpin = false;
					spinning = false;
				}
			}
			if(dmgTicks > 0) 
				dmgTicks--;
		}
	}
}