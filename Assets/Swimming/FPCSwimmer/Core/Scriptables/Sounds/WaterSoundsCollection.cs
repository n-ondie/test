using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPCSwimmer
{
    [CreateAssetMenu(fileName = "WaterSoundsCollection", menuName = "FPCSwimmer/Water Sounds Collection", order = 10000)]
    public class WaterSoundsCollection : SoundsCollection
    {
        public List<AudioClip> waterSmallWaveSounds;
        public List<AudioClip> waterSplashSounds;
        public List<AudioClip> waterImmersionSounds;
        public List<AudioClip> waterEmersionSounds;
        public AudioClip underwaterSound;

        private int randomIndex = 0;
        private int maxRandomIndexValue = 1000;

        public AudioClip GetUnderwaterSound()
        {
            return underwaterSound;
        }

        public AudioClip GetRandomSmallWaveSound()
        {
            return GetRandomSound(waterSmallWaveSounds);
        }

        public AudioClip GetRandomSplashSound()
        {
            return GetRandomSound(waterSplashSounds);
        }

        public AudioClip GetRandomImmersionSound()
        {
            return GetRandomSound(waterImmersionSounds);
        }

        public AudioClip GetRandomEmersionSound()
        {
            return GetRandomSound(waterEmersionSounds);
        }

        private AudioClip GetRandomSound(List<AudioClip> source)
        {
            randomIndex = randomIndex > maxRandomIndexValue ? 0 : randomIndex + 1;
            if (source != null && source.Count > 0)
            {
                return source[randomIndex % source.Count];
            }
            return null;
        }
    }
}