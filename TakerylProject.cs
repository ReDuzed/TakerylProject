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
}
