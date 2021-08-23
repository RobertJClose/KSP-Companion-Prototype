using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Angle : System.IEquatable<Angle>, System.IComparable<Angle>
{
    // This class is used to represent an angle in radians: 0 < angle < 2PI
    private readonly float angle; // The angle stored in radians

    public float RadValue
    {
        get => angle;
    }

    public float DegValue
    {
        get => angle * Mathf.Rad2Deg;
    }

    public float RadValueMinusPiToPiRange
    {
        get
        {
            return (angle < Mathf.PI) ? (angle) : (-2.0f * Mathf.PI + angle);
        }
    }

    public float DegValueMinus180To180Range
    {
        get
        {
            return RadValueMinusPiToPiRange * Mathf.Rad2Deg;
        }
    }

    // ********** Methods **********
    public bool IsBetween(Angle? lowerBound, Angle? upperBound)
    {
        // This function uses the 'lowerBound' and 'upperBound' parameters to define a region that is checked to see if this angle is in.
        // If either bound is null then false is returned.

        // The two cases to catch is that the lower bound may be either lesser or greater than the upper bound:
        // The normal hierarchy (lower bound < upper bound):
        // 0 rad ----- lower bound ***** upper bound ----- 2PI

        // Inverted hierarchy   (lower bound > upper bound):
        // 0 rad ***** upper bound ----- lower bound ***** 2PI

        // Between() returns true  : *****
        // Between() returns false : -----

        // Check if the region is non-existant
        if (lowerBound == upperBound)
            return angle == lowerBound;
        else if (lowerBound == null || upperBound == null)
            return false;

        if (lowerBound < upperBound)
        {
            // Normal hierarchy.
            if (!(lowerBound < angle) || !(angle < upperBound))
            {
                // The angle is not within the region.
                return false;
            }
        }

        if (lowerBound > upperBound)
        {
            // Inverted hierarchy.
            if (!(lowerBound < angle) && !(angle < upperBound))
            {
                // The angle is not within the region.
                return false;
            }
        }

        // The angle is in the forbidden region.
        return true;
    }

    public Angle Closer(Angle? angleOne, Angle? angleTwo)
    {
        if (angleOne == null && angleTwo != null)
            return angleTwo.Value;
        else if (angleOne != null && angleTwo == null)
            return angleOne.Value;
        else if (angleOne == null && angleTwo == null)
            return angle;
        else
        {
            float diffOne = Mathf.Abs(angle - angleOne.Value);
            float diffTwo = Mathf.Abs(angle - angleTwo.Value);
            return (diffOne < diffTwo) ? angleOne.Value : angleTwo.Value;
        }
    }

    // ********** Static Methods ********** 
    private static float EquivalentRad(float angle)
    {
        // Converts an angle in radians to the equivalent angle in the range 0 < output < 2PI 
        float output = angle % (2f * Mathf.PI);

        if (output < 0)
            output = 2f * Mathf.PI + output;

        return output;
    }

    private static float EquivalentDeg(float angle)
    {
        // Converts an angle in degrees to the equivalent angle in the range 0 < output < 360
        float output = angle % 360f;

        if (output < 0)
            output = 360f + output;

        return output;
    }

    public static Angle Expel(Angle angle, Angle forbiddenLowerBound, Angle forbiddenUpperBound)
    {
        // This method checks to see in an angle is within a forbidden region defined by the upper and lower bounds.
        // If the angle is not in this region then it is returned. If it is in this region then the closer of the 
        // two bounds is returned.

        // Check if the forbidden region is non-existant
        if (forbiddenLowerBound == forbiddenUpperBound)
        {
            return angle;
        }

        if (!angle.IsBetween(forbiddenLowerBound, forbiddenUpperBound))
            return angle;
        else
        {
            // The angle is in the forbidden region.
            return angle.Closer(forbiddenLowerBound, forbiddenUpperBound);
        }
    }

    public override bool Equals(object obj)
    {
        var item = (Angle)obj;

        if (item == null)
            return false;

        return Equals(item);
    }

    public override int GetHashCode()
    {
        return angle.GetHashCode();
    }

    public override string ToString()
    {
        return RadValue.ToString();
    }

    public bool Equals(Angle other)
    {
        if (other == null)
            return false;

        // As their values are stored as floats, we check for equality using the Mathf.Approximately() method. 
        // A special case arises however, because all angles are stored between the values of 0 <= value < 2PI.
        // Two angles may be close enough to be considered equal, but be split on either side of 2PI, and so have a difference of ~2PI. 
        // For example: angleOne = 0rad, angleTwo = (2PI - 0.000...0001)rad.
        // These two angles are approximately equal, but sit on either side of the 0/2PI divide, and so will return 
        // false from the Approximately() method.

        float difference = Mathf.Abs(angle - other.RadValue);

        if (Mathf.Approximately(difference, 2f * Mathf.PI)) // Special case checking
            return true;

        return Mathf.Approximately(difference, 0f);
    }

    public int CompareTo(Angle other)
    {
        if (other == null)
            return 1;

        return angle.CompareTo(other.RadValue);
    }

    // ********** Static Members **********
    public static Angle Zero => new(0f);
    public static Angle QuarterTurn => new(Mathf.PI / 2f);
    public static Angle HalfTurn => new(Mathf.PI);
    public static Angle ThreeQuartersTurn => new(3f * Mathf.PI / 2f);

    public static Angle MaxAngle => new(6.283185243f); // The largest value before becoming Angle(0f). Found through horrible trial and error.
    public static Angle Tiny => new(0.000001f);

    // ********** Constructors ************
    public Angle(float f, bool isDeg = false)
    {
        if (isDeg)
            angle = EquivalentDeg(f) * Mathf.Deg2Rad;
        else
            angle = EquivalentRad(f);
    }

    // ********** Operator Overloads
    public static bool operator==(Angle angleOne, Angle angleTwo)
    {
        // As their values are stored as floats, we check for equality using the Mathf.Approximately() method. 
        // A special case arises however, because all angles are stored between the values of 0 <= value < 2PI.
        // Two angles may be close enough to be considered equal, but be split on either side of 2PI, and so have a difference of ~2PI. 
        // For example: angleOne = 0rad, angleTwo = (2PI - 0.00000001)rad.
        // These two angles are approximately equal, but sit on either side of the 0/2PI divide, and so will return 
        // false from the Approximately() method.

        float difference = Mathf.Abs(angleOne.RadValue - angleTwo.RadValue);

        if (Mathf.Approximately(difference, 2f * Mathf.PI)) // Special case checking
            return true;

        return Mathf.Approximately(difference, 0f);
    }

    public static bool operator!=(Angle angleOne, Angle angleTwo)
    {
        // As their values are stored as floats, we check for equality using the Mathf.Approximately() method. 
        // A special case arises however, because all angles are stored between the values of 0 <= value < 2PI.
        // Two angles may be close enough to be considered equal, but be split on either side of 2PI, and so have a difference of ~2PI. 
        // For example: angleOne = 0rad, angleTwo = (2PI - 0.00000001)rad.
        // These two angles are approximately equal, but sit on either side of the 0/2PI divide, and so will return 
        // false from the Approximately() method.

        float difference = Mathf.Abs(angleOne.RadValue - angleTwo.RadValue);

        if (Mathf.Approximately(difference, 2f * Mathf.PI)) // Special case checking
            return false;

        return !Mathf.Approximately(difference, 0f);
    }

    public static bool operator<(Angle angleOne, Angle angleTwo)
    {
        return angleOne.RadValue < angleTwo.RadValue;
    }

    public static bool operator>(Angle angleOne, Angle angleTwo)
    {
        return angleOne.RadValue > angleTwo.RadValue;
    }

    public static Angle operator-(Angle angle)
    {
        return new Angle(2.0f * Mathf.PI - angle.angle);
    }

    // ********** Conversions ************
    public static implicit operator Angle(float f) => new(f);  // float to Angle conversion, where the float value is interpreted as an angle in radians.

    public static implicit operator float(Angle angle) => angle.RadValue; // Angle to float conversion, where the implied request is the angle in radians.
}
