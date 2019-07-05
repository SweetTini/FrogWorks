namespace FrogWorks
{
    public struct Animation
    {
        public int[] Frames { get; internal set; }

        public float Duration { get; internal set; }

        public bool Loop { get; internal set; }

        public int LoopCount { get; internal set; }

        public int LoopFrom { get; internal set; }
    }
}
