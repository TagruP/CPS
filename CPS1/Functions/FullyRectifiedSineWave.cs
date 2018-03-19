﻿namespace CPS1.Functions
{
    using System;

    public class FullyRectifiedSineWave : Function
    {
        public FullyRectifiedSineWave()
        {
            this.SignalFunction = (A, T, t, t1, p) =>
                {
                    var ret = A * Math.Sin(Math.PI * 2 * (t - t1) / T);
                    if (ret.CompareTo(0) < 0)
                    {
                        ret *= -1;
                    }

                    return ret;
                };
        }

        public static Required RequiredAttributes { get; } = new Required(true, true, true, true, false, true, true, true);
    }
}