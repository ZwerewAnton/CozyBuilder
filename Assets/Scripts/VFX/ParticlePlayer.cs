using UnityEngine;

namespace VFX
{
    public class ParticlePlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] clips;

        public void Play()
        {
            foreach (var clip in clips) clip.Play();
        }
    }
}