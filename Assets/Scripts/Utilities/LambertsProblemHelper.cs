using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LambertsProblemHelper
{
    // Reference paper:
    // Revisiting Lambert's Problem - Dario Izzo (2014)

    public static Orbit Solver(GravitationalBody body, Vector3d positionOne, double timeOne, Vector3d positionTwo, double timeTwo)
    {
        double chordLength = (positionTwo - positionOne).magnitude;
        double semiPerimeter = 0.5 * (positionOne.magnitude + positionTwo.magnitude + chordLength);
        double lambdaSquared = (semiPerimeter - chordLength) / semiPerimeter;
        double lambda;
        if (positionOne.x * positionTwo.y - positionOne.y * positionTwo.x < 0)
            lambda = -Math.Sqrt(lambdaSquared);
        else
            lambda = Math.Sqrt(lambdaSquared);

        double TStar = Math.Sqrt(2.0 * body.GravParameter / (semiPerimeter * semiPerimeter * semiPerimeter)) * (timeTwo - timeOne);

        double xFinal = FindXThatProducesTStar(lambda, TStar);
        double yFinal = X2Y(xFinal, lambda);

        // Compute the velocity vector at t = timeOne on the transfer orbit from (xFinal, yFinal)
        // First compute the necessary unit vectors
        Vector3d orbitalPlaneNormalVector = Vector3d.Cross(positionOne, positionTwo).normalized;
        Vector3d tangentialUnitVector = Vector3d.Cross(orbitalPlaneNormalVector, positionOne);
        if (lambda < 0)
            tangentialUnitVector = -tangentialUnitVector;

        Debug.Log("Position one: " + positionOne);
        Debug.Log("Position two: " + positionTwo);
        Debug.Log("h normalised vector:\n" + orbitalPlaneNormalVector);
        Debug.Log("tUnit Vector:\n" + tangentialUnitVector);

        double gamma = Math.Sqrt(body.GravParameter * semiPerimeter / 2.0);
        double rho = (positionOne.magnitude - positionTwo.magnitude) / chordLength;
        double sigma = Math.Sqrt(1.0 - rho * rho);

        double componentOne = gamma / positionOne.magnitude * (lambda * yFinal - xFinal - rho * (lambda * yFinal + xFinal));
        double componentTwo = gamma * sigma / positionOne.magnitude * (yFinal + lambda * xFinal);

        Vector3d velocityAtPositionOne = componentOne * positionOne.normalized + componentTwo * tangentialUnitVector;

        return Orbit.StateVectors2Orbit(body, positionOne, velocityAtPositionOne, timeOne);

        //double posOneMag = positionOne.magnitude;
        //double posTwoMag = positionTwo.magnitude;
        //double planeNormalX = 1.0 / (posOneMag * posTwoMag) * (positionOne.y * positionTwo.z - positionOne.z * positionTwo.y);
        //double planeNormalY = 1.0 / (posOneMag * posTwoMag) * (positionOne.z * positionTwo.x - positionOne.x * positionTwo.z);
        //double planeNormalZ = 1.0 / (posOneMag * posTwoMag) * (positionOne.x * positionTwo.y - positionOne.y * positionTwo.x);
        //double tUnitVectorX = 1.0 / posOneMag * (planeNormalY * positionOne.z - planeNormalZ * positionOne.y);
        //double tUnitVectorY = 1.0 / posOneMag * (planeNormalZ * positionOne.x - planeNormalX * positionOne.z);
        //double tUnitVectorZ = 1.0 / posOneMag * (planeNormalX * positionOne.y - planeNormalY * positionOne.x);
        //if (lambda < 0)
        //{
        //    tUnitVectorX *= -1.0;
        //    tUnitVectorY *= -1.0;
        //    tUnitVectorZ *= -1.0;
        //}

        //Debug.Log("Position one: " + positionOne);
        //Debug.Log("Position two: " + positionTwo);
        //Debug.Log("h normalised vector:\n" + planeNormalX + '\n' + planeNormalY + '\n' + planeNormalZ);
        //Debug.Log("tUnit Vector:\n" + tUnitVectorX + '\n' + tUnitVectorY + '\n' + tUnitVectorZ);

        //double gamma = Math.Sqrt(body.GravParameter * semiPerimeter / 2.0);
        //double rho = (positionOne.magnitude - positionTwo.magnitude) / chordLength;
        //double sigma = Math.Sqrt(1.0 - rho * rho);

        //double componentOne = gamma / positionOne.magnitude * (lambda * yFinal - xFinal - rho * (lambda * yFinal + xFinal));
        //double componentTwo = gamma * sigma / positionOne.magnitude * (yFinal + lambda * xFinal);

        //Vector3 velocityAtPositionOne;
        //velocityAtPositionOne.x = (float)(componentOne * positionOne.x / positionOne.magnitude) + (float)(componentTwo * tUnitVectorX);
        //velocityAtPositionOne.y = (float)(componentOne * positionOne.y / positionOne.magnitude) + (float)(componentTwo * tUnitVectorY);
        //velocityAtPositionOne.z = (float)(componentOne * positionOne.z / positionOne.magnitude) + (float)(componentTwo * tUnitVectorZ);

        //return Orbit.StateVectors2Orbit(body, positionOne, velocityAtPositionOne, timeOne);
    }

    private static double FindXThatProducesTStar(double lambda, double TStar)
    {
        // Calculate T for the minimum energy ellipse and the parabolic case
        double T0 = Math.Acos(lambda) + lambda * Math.Sqrt(1.0 - lambda * lambda);
        double T1 = 2.0 / 3.0 * (1 - lambda * lambda * lambda);

        // Make an initial guess for x based on TStar, T0, and T1.
        double x;
        if (TStar < T1)
        {
            x = 5.0 / 2.0 * (T1 * (T1 - TStar)) / (TStar * (1.0 - Math.Pow(lambda, 5))) + 1;
        }
        else if (Mathf.Approximately((float)TStar, (float)T1))
        {
            x = 1.0;
            return x;
        }
        else if (T1 < TStar && TStar < T0)
        {
            x = Math.Pow(T0 / TStar, Math.Log(T1 / T0, 2)) - 1;
        }
        else if (Mathf.Approximately((float)TStar, (float)T0))
        {
            x = 0.0;
            return x;
        }
        else
        {
            // TStar > T0
            x = Math.Pow(T0 / TStar, 2.0 / 3.0) - 1;
        }

        // Do 3rd order Householder iteration to find a value for x that solves T(x) - TStar = 0.
        double y;
        double T;       // The nondimensional time of flight T.
        double Td;      // First derivative.
        double Tdd;     // Second derivative.
        double Tddd;    // Third derivative.
        for (int i = 0; i < Constants.LambertSolverMaxIterations; i++)
        {
            y = X2Y(x, lambda);
            T = TOfX(x, y, lambda);
            Td = TFirstDerivative(x, y, lambda, T);
            Tdd = TSecondDerivative(x, y, lambda, T, Td);
            Tddd = TThirdDerivative(x, y, lambda, T, Td, Tdd);

            // Iterate the next guess for x
            x = x - (T - TStar) * (Td * Td - 0.5 * (T - TStar) * Tdd) / (Td * (Td * Td - (T - TStar) * Tdd) + 1.0 / 6.0 * (T - TStar) * (T - TStar) * Tddd);
        }

        Debug.Log("Lambert Solver Householder iteration output: \nx = " + x + "\nT(x) = " + TOfX(x, X2Y(x, lambda), lambda) + "\nTStar = " + TStar);
        return x;
    }

    private static double X2Y(double x, double lambda)
    {
        return Math.Sqrt(1 - lambda * lambda * (1.0 - x * x));
    }

    private static double TOfX(double x, double y, double lambda, int numOfRevolutions = 0)
    {
        return 1.0 / (1.0 - x * x) * ((Psi(x, y, lambda) + numOfRevolutions * Math.PI) / Math.Sqrt(Math.Abs(1.0 - x * x)) - x + lambda * y);
    }

    private static double TFirstDerivative(double x, double y, double lambda, double T)
    {
        return 1.0 / (1.0 - x * x) * (3.0 * x * T - 2.0 + 2.0 * lambda * lambda * lambda * x / y);
    }

    private static double TSecondDerivative(double x, double y, double lambda, double T, double TfirstDeriv)
    {
        return 1.0 / (1.0 - x * x) * (3.0 * T + 5.0 * x * TfirstDeriv + 2.0 * (1 - lambda * lambda) * lambda * lambda * lambda / y / y / y);
    }

    private static double TThirdDerivative(double x, double y, double lambda, double T, double TFirstDeriv, double TSecondDeriv)
    {
        return 1.0 / (1.0 - x * x) * (7.0 * x * TSecondDeriv + 8.0 * TFirstDeriv - 6.0 * (1.0 - lambda * lambda) * Math.Pow(lambda, 5.0) * x / Math.Pow(y, 5.0));
    }

    private static double Psi(double x, double y, double lambda)
    {
        if (x < 1.0)
        {
            return Math.Acos(x * y + lambda * (1.0 - x * x));
        }
        else
            return ACosh(x * y - lambda * (1.0 - x * x));
    }

    // Inverse hyperbolic cosine function
    private static double ACosh(double x)
    {
        return Math.Log(x + Math.Sqrt(x * x - 1.0));
    }
}
