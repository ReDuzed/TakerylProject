using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject.Items
{
	public class AngelicGrace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Angelic Grace");
			Tooltip.SetDefault("An agreement to the heavens");
		}
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 32;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = 2;
			item.maxStack = 1;
			item.value = 0;
			item.rare = 3;
			item.consumable = true;
			item.autoReuse = false;
			item.useTurn = false;
			item.noMelee = true;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(9);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
		public override bool UseItem(Player player)
		{
			var modPlayer = player.GetModPlayer<ProjectPlayer>(mod);
			modPlayer.angel = true;
			
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/conjure"), player.Center);
			
			for(int i = 0; i < player.inventory.Length-1; i++)
			{
				if(player.inventory[i].type == mod.ItemType("AngelicGrace"))
					player.inventory[i].type = 0;
			}
			return true;
		}
	}
}