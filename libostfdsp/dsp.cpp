#pragma once
#include "pch.h"
#include <utility>
#include "dsp.h"  
#include <iostream>

void multiplyArrayBy(float* values, int size, float factor) {

        for (int i = 0; i < size; i++) {
            values[i] *= factor;
        }
}

void addTwoFloatArrays(float* array1, float* array2, int size) {

    for (int i = 0; i < size; i++) {
            array1[i] += array2[i];
        }

}

void fft_complex(double* real, double* imag, int size, int direction = 1, bool reorder = true, bool scaleForwardTransform = true) {

        // This is a modified C++ translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

        int ExponentSign ;
        if (direction == 1) {
            ExponentSign = -1;
        }
        else {
            ExponentSign = 1;
        };

        if (reorder == true) {
            double TempX;
            double TempY;

            int j = 0;
            for (int i = 0; i < size - 1; ++i) {
                if (i < j) {
                    TempX = real[i];
                    real[i] = real[j];
                    real[j] = TempX;

                    TempY = imag[i];
                    imag[i] = imag[j];
                    imag[j] = TempY;
                }

                int m = size;
                do {
                    m >>= 1;
                    j ^= m;
                } while ((j & m) == 0);
            }
        }


        // Defining some temporary variables to avoid definition inside the loop
        double aiX;
        double aiY;
        double Real1;
        double Imaginary1;
        double Real2;
        double Imaginary2;
        double TempReal1;

        const double pi = 3.14159265358979323846;

        int LevelSize = 1;
        while (LevelSize < size) {
            for (int k = 0; k < LevelSize; ++k) {
                int e_k = ExponentSign * k;
                double exponent = e_k * pi / LevelSize;
                //double exponent = (ExponentSign * k) * pi / LevelSize;
                double wX = cos(exponent);  // N.B. this step of the algorithm suffers from the inexact floating point numbers returned from the trigonometric functions cos and sin
                double wY = sin(exponent);

                int StepSize = LevelSize << 1;
                int i = k;
                while (i < size - 1) {
                    aiX = real[i];
                    aiY = imag[i];

                    Real1 = wX;
                    Imaginary1 = wY;
                    Real2 = real[i + LevelSize];
                    Imaginary2 = imag[i + LevelSize];

                    // Complex multiplication
                    TempReal1 = Real1;
                    Real1 = TempReal1 * Real2 - Imaginary1 * Imaginary2;
                    Imaginary1 = TempReal1 * Imaginary2 + Imaginary1 * Real2;

                    real[i] = aiX + Real1;
                    imag[i] = aiY + Imaginary1;

                    real[i + LevelSize] = aiX - Real1;
                    imag[i + LevelSize] = aiY - Imaginary1;

                    i += StepSize;
                }
            }
            LevelSize *= 2;
        }


        // Scaling
        if (direction == 1 && scaleForwardTransform) {
            double scalingFactor = 1.0 / size;
            for (int i = 0; i < size; ++i) {
                real[i] *= scalingFactor;
                imag[i] *= scalingFactor;
            }
        }


}
