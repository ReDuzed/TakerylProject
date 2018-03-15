using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace SuperMetroid.Sounds.Custom
{
	public class heathurt : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			if (soundInstance.State == SoundState.Playing)
				return null;
			soundInstance.Volume = volume;
			soundInstance.Pan = pan;
			soundInstance.Pitch = (float)Main.rand.Next(-5, 6) * .05f;
			return soundInstance;
		}
	}
}