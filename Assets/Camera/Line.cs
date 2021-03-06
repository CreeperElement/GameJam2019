﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Camera
{
    class Line
    {
        private float _slope;
        public float Slope{ get { return _slope; } }

        private float _intercept;
        public float Intercept { get { return _intercept; } }

        public Line(Vector2 a, Vector2 b)
        {
            _slope = (b.y - a.y) / (b.x - a.x);

            // y = mx + b
            // b = y - mx
            _intercept = a.y - _slope * a.x;
        }

        public float y(float x)
        {
            return Slope*x + Intercept;
        }

        public bool pointIsAbove(Vector2 point)
        {
            // TODO: After some testing, perhaps see if some variability is needed
            var lineHeight = y(point.x);
            return point.y > lineHeight;
        }

        public bool pointIsBelow(Vector2 point)
        {
            // TODO: After some testing, perhaps see if some variability is needed
            var lineHeight = y(point.x);
            return point.y < lineHeight;
        }

    }
}
