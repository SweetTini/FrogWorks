namespace FrogWorks
{
    public abstract class VirtualInput
    {
        protected VirtualInput()
        {
            if (!Input._virtualInputs.Contains(this))
                Input._virtualInputs.Add(this);
        }

        public void Deregister()
        {
            if (Input._virtualInputs.Contains(this))
                Input._virtualInputs.Remove(this);
        }

        public abstract void Update(float deltaTime);
    }

    public abstract class VirtualInputNode
    {
        public virtual void Update(float deltaTime)
        {
        }
    }

    public enum OverlapMode
    {
        Cancel,
        TakeLatest,
        TakeOldest
    }

    public enum ThresholdMode
    {
        GreaterThan,
        LessThan,
        EqualTo
    }
}
