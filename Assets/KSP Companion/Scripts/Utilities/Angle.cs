using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an angle.
/// </summary>
/// <remarks>
/// This struct can be used to make working with floating-point values representing angles much simpler. Each instance of 
/// this struct stores a float value that represents an angle in radians between 0 and 2PI. Instances of this struct may be
/// implicitly converted to and from floats, allowing for easy use of Angle type objects in expressions expecting float data.
/// There are methods to assist in common tasks whose execution is made more challenging by the periodic nature of angular values,
/// such as working out if an angle is between two other angles (see <see cref="IsBetween(Angle?, Angle?)"/>).
/// </remarks>
public readonly struct Angle : System.IEquatable<Angle>, System.IComparable<Angle>
{
    #region Fields

    // The angle stored in radians. This value is always between 0 and 2.0f * Mathf.PI.
    private readonly float angle; 

    #endregion

    #region Constructors

    /// <summary>
    /// Initialises a new Angle object with a value either in radians (default) or degrees.
    /// </summary>
    /// <param name="f">The value of the angle.</param>
    /// <param name="isDeg">Indicates whether <paramref name="f"/> is a value in radians or degrees.</param>
    public Angle(float f, bool isDeg = false)
    {
        if (isDeg)
            angle = EquivalentDeg(f) * Mathf.Deg2Rad;
        else
            angle = EquivalentRad(f);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the smallest Angle that is greater than zero.
    /// </summary>
    public static Angle Epsilon => new Angle(float.Epsilon);

    /// <summary>
    /// Gets an angle of one half turn.
    /// </summary>
    public static Angle HalfTurn => new Angle(Mathf.PI);

    /// <summary>
    /// Gets an angle with the largest possible value for an angle before wrapping around 2PI.
    /// </summary>
    public static Angle MaxAngle => new Angle(6.283185243f);
    
    /// <summary>
    /// Gets an angle of one quarter turn.
    /// </summary>
    public static Angle QuarterTurn => new Angle(Mathf.PI / 2f);
    
    /// <summary>
    /// Gets an angle of three quarters of a full turn.
    /// </summary>
    public static Angle ThreeQuartersTurn => new Angle(3f * Mathf.PI / 2f);

    /// <summary>
    /// Gets an angle of no turn.
    /// </summary>
    public static Angle Zero => new Angle(0f);
    
    /// <summary>
    /// Gets the value of the angle in degrees, in the range of 0 to 360.
    /// </summary>
    public float DegValue
    {
        get => angle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Gets the value of the angle in degrees, in the range of -180 to 180.
    /// </summary>
    public float DegValueMinus180To180Range
    {
        get
        {
            return RadValueMinusPiToPiRange * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Gets the value of the angle in radians, in the range of 0 to 2PI.
    /// </summary>
    public float RadValue
    {
        get => angle;
    }

    /// <summary>
    /// Gets the value of the angle in radians, in the range of -PI to PI.
    /// </summary>
    public float RadValueMinusPiToPiRange
    {
        get
        {
            return (angle < Mathf.PI) ? (angle) : (-2.0f * Mathf.PI + angle);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Implicitly converts a float value to an Angle, where the float value is interpreted as an angle in radians.
    /// </summary>
    /// <param name="f">The value of the angle in radians.</param>
    public static implicit operator Angle(float f) => new Angle(f);

    /// <summary>
    /// Checks for approximate equality between two Angle instances.
    /// </summary>
    /// <param name="angleOne">The first Angle to be compared.</param>
    /// <param name="angleTwo">The second Angle to be compared.</param>
    /// <returns>
    /// True if the values represented by <paramref name="angleOne"/> and <paramref name="angleTwo"/> are 
    /// approximately equal according to the UnityEngine.Mathf.Approximately() method. False if the angles are not 
    /// approximately equal, or if either argument is null.
    /// </returns>
    public static bool Approximately(Angle? angleOne, Angle? angleTwo)
    {
        if (angleOne == null || angleTwo == null)
            return false;

        // A special case arises for approximate equality checks because all angles are stored between the values of 0 <= value < 2PI.
        // Two angles may be close enough to be considered equal, but be split on either side of 2PI, and so have a difference of ~2PI. 
        // For example: angleOne = 0rad, angleTwo = (2PI - 0.000...0001)rad.
        // These two angles are approximately equal, but sit on either side of the 0/2PI divide, and so will incorrectly return 
        // false from the Approximately() method.

        float difference = Mathf.Abs(angleOne.Value - angleTwo.Value);

        if (Mathf.Approximately(difference, 2f * Mathf.PI)) // Special case checking
            return true;

        return Mathf.Approximately(difference, 0f);
    }

    /// <summary>
    /// Expels an angle from a region of forbidden values.
    /// </summary>
    /// <param name="angle">An angle.</param>
    /// <param name="forbiddenStartValue">
    /// The starting value for the forbidden region.
    /// </param>
    /// <param name="forbiddenEndValue">
    /// The final value of the forbidden region. This may be a smaller numerical value than the starting value, with the 
    /// interpretation that the forbiden region wraps around 2PI to end at the final bound.
    /// </param>
    /// <returns>If <paramref name="angle"/> is not in the forbidden region, then it is returned. If it is in the 
    /// forbidden region, the closer of the two boundary values is returned.</returns>
    public static Angle Expel(Angle angle, Angle forbiddenStartValue, Angle forbiddenEndValue)
    {
        // Check if the forbidden region is non-existant
        if (forbiddenStartValue == forbiddenEndValue)
        {
            return angle;
        }

        if (!angle.IsBetween(forbiddenStartValue, forbiddenEndValue))
            return angle;
        else
        {
            return angle.Closer(forbiddenStartValue, forbiddenEndValue);
        }
    }

    /// <summary>
    /// Implicitly converts an Angle in radians to a float value.
    /// </summary>
    /// <param name="angle">The angle to be converted.</param>
    public static implicit operator float(Angle angle) => angle.RadValue;

    /// <summary>
    /// Checks for equality between two angles.
    /// </summary>
    /// <param name="angleOne">The first angle.</param>
    /// <param name="angleTwo">The second angle.</param>
    /// <returns>
    /// True the two angles are equal. False otherwise. 
    /// <para>
    /// Two angles may be close enough that their values are considered equal, but the angles are split on either
    /// side of the 0/2PI wrap around. This operator checks for that case and will return true if the angles are close enough
    /// even if they are split in such a manner.
    /// </para>
    /// </returns>
    public static bool operator ==(Angle angleOne, Angle angleTwo)
    {
        return angleOne.angle == angleTwo.angle;
    }

    /// <summary>
    /// Checks for inequality between two angles.
    /// </summary>
    /// <param name="angleOne">The first angle to compare.</param>
    /// <param name="angleTwo">The second angle to compare.</param>
    /// <returns>True if the angles are not equal. False if the angles are equal.</returns>
    public static bool operator !=(Angle angleOne, Angle angleTwo)
    {
        return angleOne.angle != angleTwo.angle;
    }

    /// <summary>
    /// Checks to see if a specified angle is smaller than another specified angle.
    /// </summary>
    /// <param name="angleOne">The first angle to compare.</param>
    /// <param name="angleTwo">The second angle to compare.</param>
    /// <returns>True if <paramref name="angleOne"/> is smaller than <paramref name="angleTwo"/>. False otherwise.</returns>
    public static bool operator <(Angle angleOne, Angle angleTwo)
    {
        return angleOne.RadValue < angleTwo.RadValue;
    }

    /// <summary>
    /// Checks to see if a specified angle is greater than another specified angle.
    /// </summary>
    /// <param name="angleOne">The first angle to compare.</param>
    /// <param name="angleTwo">The second angle to compare.</param>
    /// <returns>True if <paramref name="angleOne"/> is greater than <paramref name="angleTwo"/>. False otherwise.</returns>
    public static bool operator >(Angle angleOne, Angle angleTwo)
    {
        return angleOne.RadValue > angleTwo.RadValue;
    }

    /// <summary>
    /// Flips an angle around to be a turn in the opposite direction.
    /// </summary>
    /// <param name="angle">The angle to be flipped.</param>
    /// <returns>2PI minus <paramref name="angle"/>. This is the same as rotating by the same amount as <paramref name="angle"/>,
    /// but in the opposite direction.</returns>
    public static Angle operator -(Angle angle)
    {
        return new Angle(2.0f * Mathf.PI - angle.angle);
    }

    /// <summary>
    /// Checks for equality with another object.
    /// </summary>
    /// <param name="obj">The object to be checked.</param>
    /// <returns>True if <paramref name="obj"/> is an instance of Angle and equals the value of this Angle. False otherwise.</returns>
    public override bool Equals(object obj)
    {
        var item = (Angle)obj;

        if (item == null || !GetType().Equals(obj.GetType()))
            return false;

        return Equals(item);
    }

    /// <summary>
    /// Checks for equality with another Angle object.
    /// </summary>
    /// <param name="other">The object to be compared.</param>
    /// <returns>True if <paramref name="other"/> is equal to this instance. False otherwise.</returns>
    public bool Equals(Angle other)
    {
        return angle.Equals(other.angle);
    }

    /// <summary>
    /// Finds which of two angles is closer to this instance.
    /// </summary>
    /// <param name="angleOne">The first Angle.</param>
    /// <param name="angleTwo">The second Angle.</param>
    /// <returns>The closer of <paramref name="angleOne"/> and <paramref name="angleTwo"/> to this instance's value.</returns>
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

    /// <summary>
    /// Compares this instance to a specified Angle and returns an integer indicating whether the value of this instance
    /// is less than, equal to, or greater than the value of the specified Angle.
    /// </summary>
    /// <param name="other">An Angle to be compared.</param>
    /// <returns>Returns zero if the values are equal. Returns an integer less than zero if this instance's value is 
    /// less than <paramref name="other"/>'s value. Returns an integer greater than zero if this instance's value is 
    /// greater than <paramref name="other"/>'s value.</returns>
    public int CompareTo(Angle other)
    {
        if (other == null)
            return 1;

        return angle.CompareTo(other.RadValue);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return angle.GetHashCode();
    }

    /// <summary>
    /// Indicates whether this Angle is in the region between two other angles. This region may wrap around 2PI.
    /// </summary>
    /// <param name="lowerBound">
    /// The starting value for the region. This may be a larger value than <paramref name="upperBound"/>, in which case the 
    /// region between the two angles is wrapped around at 2PI.
    /// </param>
    /// <param name="upperBound">
    /// The ending value for the region. This may be a smaller value than <paramref name="lowerBound"/>, in which case the region
    /// between the two angles is wrapped around at 2PI.
    /// </param>
    /// <returns>True if this angle instance is in the region between the two bounds. False otherwise.</returns>
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

    /// <summary>
    /// Converts the numeric value of this instance to a string representation.
    /// </summary>
    /// <returns>The string representation of this instance.</returns>
    public override string ToString()
    {
        return RadValue.ToString();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Finds an equivalent angle in the range 0 to 360 degrees.
    /// </summary>
    /// <param name="angle">The value of an angle in degrees.</param>
    /// <returns>A value in the range of 0 to 360 degrees that is the same angle as <paramref name="angle"/>.</returns>
    private static float EquivalentDeg(float angle)
    {
        // Converts an angle in degrees to the equivalent angle in the range 0 < output < 360
        float output = angle % 360f;

        if (output < 0)
            output = 360f + output;

        return output;
    }

    /// <summary>
    /// Finds an equivalent angle in the range 0 to 2PI.
    /// </summary>
    /// <param name="angle">The value of an angle in radians.</param>
    /// <returns>A value in the range 0 to 2PI radians that is the same angle as <paramref name="angle"/>.</returns>
    private static float EquivalentRad(float angle)
    {
        // Converts an angle in radians to the equivalent angle in the range 0 < output < 2PI 
        float output = angle % (2f * Mathf.PI);

        if (output < 0)
            output = 2f * Mathf.PI + output;

        return output;
    }

    #endregion
}
