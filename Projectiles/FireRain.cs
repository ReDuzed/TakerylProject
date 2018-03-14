using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject.Projectiles
{
	public class FireRain : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteor");
		}
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 68;
			projectile.aiStyle = -1;
			projectile.timeLeft = 600;
			projectile.friendly = true;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
			projectile.scale = 1f;
			projectile.netUpdate = true;
		}		
		private Rectangle plrB, prjB;
		private static int ticks = 0;
		public override void AI()
		{
			ticks++;
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
			Lighting.AddLight((int)(projectile.position.X + projectile.width/2)/16, (int)(projectile.position.Y + projectile.height)/16, 0.7f, 0.2f, 0.1f);
			Color newColor = default(Color);
			int c = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + projectile.height - 4f), 10, 10, 6, 0f, 0f, 100, newColor, 1.5f);
			Main.dust[c].noGravity = true;
			if(ticks%30 == 0){
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/onfire"), projectile.Center);
			}
		/*	if(ticks%15 == 0 && (Main.dedServ || Main.netMode != 0)) 
			{	
				NetMessage.SendModData(ModWorld.modId,ticks,ModPlayer.fauxPID,-1,ModPlayer.fauxPID);
				NetMessage.SendData(27, -1, -1, "", projectile.whoAmI, 0f, 0f, 0f, 0);
			} */
			foreach(NPC N in Main.npc)
			{
				if(!N.active) continue;
				if(N.life <= 0) continue;
				if(N.friendly) continue;
				if(N.dontTakeDamage) continue;
				if(N.boss) continue;
				Rectangle MB = new Rectangle((int)projectile.position.X+(int)projectile.velocity.X,(int)projectile.position.Y+(int)projectile.velocity.Y,projectile.width,projectile.height);
				Rectangle NB = new Rectangle((int)N.position.X,(int)N.position.Y,N.width,N.height);
				if(MB.Intersects(NB))
				{
					N.AddBuff(24,600,false);
					N.StrikeNPC(45, 0f, 0, false, false, false);
				}
			}
		}
		public override bool PreKill(int timeleft)
		{
			Projectile P = projectile;
			for(int i = (int)(P.position.X - (6f*projectile.scale)); i < (int)(P.position.X + 6f + (6f*projectile.scale)); i++)
			for(int j = (int)(P.position.Y + P.height - (6f*projectile.scale)); j < (int)(P.position.Y + P.height + (6f*projectile.scale)); j++)
			{
				if(i%2 == 0 && j%2 == 0) 
				{
					Color newColor = default(Color);
					int a = Dust.NewDust(new Vector2(i, j), 10, 10, 6, 0f, 0f, 100, newColor, 2.0f);
					int b = Dust.NewDust(new Vector2(i, j), 20, 20, 55, 0f, 0f, 100, newColor, 1.0f);
					Main.dust[a].noGravity = true;
					Main.dust[b].noGravity = true;
				}
			}
			if(Main.rand.Next(4) == 1) Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/blast"), P.Center);
			return true;
		}
	}
}