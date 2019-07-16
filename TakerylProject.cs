using System;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace TakerylProject
{
	public class TakerylProject : Mod
	{
		public void SetModInfo(out string name, ref ModProperties properties)
		{
			name = "Takeryl Project";
			properties.Autoload = true;
			properties.AutoloadGores = true;
			properties.AutoloadSounds = true;
		}
	}
	public class WingID
	{
		public static int Selected = None;
		public const int 
			None = 0,
			Demon = 1,
			Angel = 2;
	}
	public class SwordID
	{
		public static int Selected = None;
		public static int[] swordTypes = new int[] 
		{
			46, 121, 155, 190, 273, 368, 484, 485, 486, 675, 723, 989, 1166, 1185, 1192, 1199, ItemID.CopperShortsword
		};
		public const int
			None = 0,
			CopperShortSword = ItemID.CopperShortsword,
			LightsBane = 46,
			FieryGreatsword = 121,
			Muramasa = 155,
			BladeOfGrass = 190,
			NightsEdge = 273,
			Excalibur = 368,
			Adamantite = 484,
			Cobalt = 485,
			Mythril = 486,
			TrueNightsEdge = 675,
			BeamWword = 723,
			EnchantedSword = 989,
			BoneSword = 1166,
			Palladium = 1185,
			Orichalcum = 1192,
			Titanium = 1199;
	}
}
