﻿namespace FrogWorks
{
    public class VirtualButtonLeftAnalog : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public AnalogAxis Axis { get; private set; }

        public float DeadZone { get; private set; }

        public override bool IsDown => Input.GamePads[PlayerIndex].IsLeftAnalogDown(Axis, DeadZone);

        public override bool IsPressed => Input.GamePads[PlayerIndex].IsLeftAnalogPressed(Axis, DeadZone);

        public override bool IsReleased => Input.GamePads[PlayerIndex].IsLeftAnalogReleased(Axis, DeadZone);

        public VirtualButtonLeftAnalog(int playerIndex, AnalogAxis axis, float deadZone)
        {
            PlayerIndex = playerIndex;
            Axis = axis;
            DeadZone = deadZone;
        }
    }
}
