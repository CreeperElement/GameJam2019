using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Camera
{
    /// <summary>
    /// Builds upon the line to add the ability to do any number of above/below/equal tests
    /// </summary>
    class Boundary : Line
    {
        private BoundaryCondition _boundaryCondition;
        public BoundaryCondition boundary { get { return _boundaryCondition; } }

        public Boundary(Vector2 a, Vector2 b, BoundaryCondition boundaryCondition): base(a, b)
        {
            _boundaryCondition = boundaryCondition;
        }

        public Boundary(Line line, BoundaryCondition boundaryCondition):
            // Calls the base constructor with the intercept point and one point 1 unit down the axis
            base(new Vector2(0, line.Intercept), new Vector2(1, line.Intercept + line.Slope))
        {
            _boundaryCondition = boundaryCondition;
        }

        /// <summary>
        /// Uses the Line component and the given point to determine which boundary condition
        /// this point would satisfy. (Skips less than or equal and greather than or equal)
        /// </summary>
        /// <param name="point">The point we are using to find the boundary condition</param>
        /// <returns>A boundary condition which would satisfy a boundary test
        /// using the line object and the point supplied.</returns>
        public BoundaryCondition getBoundaryPosition(Vector2 point)
        {
            // TODO: After some testing, perhaps see if some variability is needed
            // In base calls
            if (pointIsAbove(point))
                return BoundaryCondition.Above;
            else if (pointIsBelow(point))
                return BoundaryCondition.Below;
            else
                return BoundaryCondition.Equal;
        }

        /// <summary>
        /// Uses the current object's boundary condition to determine if the supplied point
        /// is within the boundaries of the object. (Usually above or below)
        /// </summary>
        /// <param name="point">The point which we are testing.</param>
        /// <returns>A true/false statement based on the BoundaryCondition</returns>
        public bool inBounds(Vector2 point)
        {
            switch(_boundaryCondition)
            {
                case BoundaryCondition.Above:
                    return pointIsAbove(point);
                case BoundaryCondition.AboveOrEqual:
                    return pointIsAbove(point) || !pointIsBelow(point);
                case BoundaryCondition.Below:
                    return pointIsBelow(point);
                case BoundaryCondition.BelowOrEqual:
                    return pointIsBelow(point) || !pointIsAbove(point);
                case BoundaryCondition.Equal:
                    return !pointIsAbove(point) && !pointIsBelow(point);
                default: // We should never get here. It's an enum...
                    return false;
            }
        }
    }
}
