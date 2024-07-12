namespace Marius.Engine
{
    public class Animator : Component
    {
        public Animation Animation { get; private set; }
        public float Time { get; set; }
        public bool IsPlaying { get; private set; }

        public void Play(Animation animation)
        {
            if (animation != Animation)
            {
                Animation = animation;
                Time = 0;
                IsPlaying = animation != null;
            }
        }

        public void Pause()
        {
            IsPlaying = false;
        }
    }
}
