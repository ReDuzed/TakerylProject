using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject.Projectiles
{
    //  Possibility to thrown any held object
    public class ThrownSword : ModProjectile
    {
        private bool preAI;
        private short ai
        {
            get { return (short)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        private short graphic
        {
            get { return (short)projectile.ai[1]; }
        }
        private int ticks
        {
            get { return (int)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }
        private const float gravityMax = 9.17f;
        private Texture2D texture;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public const short
            AI_Default = 0,
            AI_Embed4Owner = 1,
            AI_Embed4All = 2,
            AI_Explode = 10;
        public override bool Autoload(ref string name)
        {
            return true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thrown Sword");
        }
        public override void SetDefaults()
        {
            projectile.aiStyle = -1;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override bool PreAI()
        {
            if (!preAI)
            {
                texture = Main.itemTexture[graphic];
                projectile.width = texture.Width;
                projectile.height = texture.Height;
                projectile.damage = 30;
                projectile.knockBack = 5f;
                //  set variations for initialze speed adjustments
                //  flying (default) = 0
                if (ai == AI_Default)
                {
                    projectile.velocity *= 2f;
                    projectile.tileCollide = true;
                }
                preAI = true;
            }
            return true;
        }
        public override void AI()
        {
            ticks++;
            Func<int, bool> fall = delegate(int max) {
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 4f;
                if (ticks > max) {
                    //  projectile.rotation += 0.017f * 5f;
                    if (projectile.velocity.Y < gravityMax) {
                        projectile.velocity.Y += gravityMax / 10f;
                    }
                    return true;
                }
                return false;
            };
            //  set variations based on when 'ticks' reaches a certain value for each ai
            //  Default = 0
            //  Sword explodes (no item) == 10
            if (ai == AI_Default)
            {
                fall(100);
            }
            if (ai == AI_Embed4Owner)
            {
                if (!TileCollide())
                    fall(100);
                else 
                {
                    projectile.velocity = Vector2.Zero;
                    //  can make check for if ANY player intersects the item
                    if (owner.Hitbox.Intersects(projectile.Hitbox))
                        projectile.Kill();
                }
            }
            if (ai == AI_Embed4All)
            {
                if (!TileCollide())
                    fall(100);
                else 
                {
                    projectile.velocity = Vector2.Zero;
                    //  can make check for if ANY player intersects the item
                    if (Main.player[Player.FindClosest(projectile.Center, projectile.width, projectile.height)].Hitbox.Intersects(projectile.Hitbox))
                        projectile.Kill();
                }
            }
            projectile.timeLeft = 3;
        }
        
        public override void Kill(int timeLeft)
        {
            //  need global explosion effect for one style
            for (int i = 0; i < 5; i++)
                Dust.NewDust(projectile.position, texture.Width, texture.Height, DustID.Stone);
            Item.NewItem(projectile.position, texture.Width, texture.Height, graphic);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (texture != null)
                spriteBatch.Draw(texture, projectile.position - Main.screenPosition, null, Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16), projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            return false;
        } 

        private bool TileCollide()
        {
            int x = (int)projectile.position.X / 16;
            int y = (int)projectile.position.Y / 16;
            int w = texture.Width / 16 / 2;
            int h = texture.Height / 16;
            for (int i = x; i < x + w; i++)
            for (int j = y; j < y + h; j++)
            {   
                Tile tile = Main.tile[i, j];
                if (tile.active() && Main.tileSolid[tile.type])
                    return true;
            }
            return false;
        }
    }
}