using System;
using Core.Data;

namespace View.Data
{
    public struct RotateRequest
    {
        public readonly Direction Direction;
        public readonly Action OnComplete;

        public RotateRequest(Direction dir, Action action)
        {
            Direction = dir;
            OnComplete = action;
        }
    }
}
