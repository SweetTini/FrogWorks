namespace FrogWorks
{
    public abstract class VirtualInput
    {
        protected VirtualInput()
        {
            if (!Input.VirtualInputs.Contains(this))
                Input.VirtualInputs.Add(this);
        }

        public void Deregister()
        {
            if (Input.VirtualInputs.Contains(this))
                Input.VirtualInputs.Remove(this);
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
