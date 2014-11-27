using System;
using System.Collections.Generic;
using System.Linq;

namespace wwtlib
{
    public class BlendState
    {
        bool state;

        public bool State
        {
            get
            {
                if (targetState != state)
                {
                    int ts = Date.Now - switchedTime;
                    if (ts > delayTime)
                    {
                        state = targetState;
                    }
                    
                    return true;
                }

                return state;
            }
            set
            {
                switchedTime = new Date(1990, 0, 0, 0, 0, 0, 0);
                state = value;
                targetState = state;
            }
        }

        bool targetState;

        public bool TargetState
        {
            get
            {
                return targetState;
            }
            set
            {
                if (targetState != value)
                {
                    switchedTime = Date.Now;
                    targetState = value;
                }
            }
        }

        public float Opacity
        {
            get
            {
                if (targetState != state)
                {
                    int ts = Date.Now - switchedTime;
                    if (ts > delayTime)
                    {
                        state = targetState;

                    }
                    else
                    {
                        float opacity = (float)(ts / delayTime);

                        return targetState ? opacity : 1f - opacity;
                    }
                }
                return state ? 1f : 0f;
            }
        }
        Date switchedTime;
        double delayTime = 0;

        public double DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }

        public BlendState()
        {
            switchedTime = new Date(1990, 0, 0, 0, 0, 0, 0);
            state = false;
            targetState = state;
            this.delayTime = 1000;
        }

        public static BlendState Create(bool initialState, double delayTime)
        {
            BlendState temp = new BlendState();
            
            temp.state = initialState;
            temp.targetState = initialState;
            temp.delayTime = delayTime;
            return temp;
        }
    }
}
