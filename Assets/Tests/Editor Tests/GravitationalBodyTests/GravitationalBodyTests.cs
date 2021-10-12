using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GravitationalBodyTests
{
    #region (In)equality operator tests

    [Test]
    public void EqualityOperator_SameGravitationalBody_ReturnsTrue()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Kerbin;

        // Act
        bool areBodiesEqual = bodyOne == bodyTwo;

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(true));
    }

    [Test]
    public void EqualityOperator_DifferentGravitationalBodies_ReturnsFalse()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Mun;

        // Act
        bool areBodiesEqual = bodyOne == bodyTwo;

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(false));
    }

    [Test]
    public void EqualityOperator_OneBodyIsNull_ReturnsFalse()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = null;

        // Act
        bool areBodiesEqual = bodyOne == bodyTwo;

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(false));
    }

    [Test]
    public void EqualityOperator_BothBodiesAreNull_ReturnTrue()
    {
        // Arrange
        GravitationalBody bodyOne = null;
        GravitationalBody bodyTwo = null;

        // Act
        bool areBodiesEqual = bodyOne == bodyTwo;

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(true));
    }

    [Test]
    public void InequalityOperator_SameGravitationalBody_ReturnsFalse()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Kerbin;

        // Act
        bool areBodiesDifferent = bodyOne != bodyTwo;

        // Assert
        Assert.That(areBodiesDifferent, Is.EqualTo(false));
    }

    [Test]
    public void InequalityOperator_DifferentGravitationalBodies_ReturnsTrue()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Mun;

        // Act
        bool areBodiesDifferent = bodyOne != bodyTwo;

        // Assert
        Assert.That(areBodiesDifferent, Is.EqualTo(true));
    }

    [Test]
    public void InequalityOperator_OneBodyIsNull_ReturnsTrue()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = null;

        // Act
        bool areBodiesDifferent = bodyOne != bodyTwo;

        // Assert
        Assert.That(areBodiesDifferent, Is.EqualTo(true));
    }

    [Test]
    public void InequalityOperator_BothBodiesAreNull_ReturnFalse()
    {
        // Arrange
        GravitationalBody bodyOne = null;
        GravitationalBody bodyTwo = null;

        // Act
        bool areBodiesDifferent = bodyOne != bodyTwo;

        // Assert
        Assert.That(areBodiesDifferent, Is.EqualTo(false));
    }

    #endregion

    #region Equals() tests

    [Test]
    public void Equals_SameGravitationalBody_ReturnsTrue()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Kerbin;

        // Act
        bool areBodiesEqual = bodyOne.Equals(bodyTwo);

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(true));
    }

    [Test]
    public void Equals_DifferentGravitationalBodies_ReturnsFalse()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = GravitationalBody.Mun;

        // Act
        bool areBodiesEqual = bodyOne.Equals(bodyTwo);

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(false));
    }

    [Test]
    public void Equals_OneBodyIsNull_ReturnsFalse()
    {
        // Arrange
        GravitationalBody bodyOne = GravitationalBody.Kerbin;
        GravitationalBody bodyTwo = null;

        // Act
        bool areBodiesEqual = bodyOne.Equals(bodyTwo);

        // Assert
        Assert.That(areBodiesEqual, Is.EqualTo(false));
    }

    #endregion
}
