//#pragma once
#include "pch.h"
#include "dsp.h" 
#include <stdio.h>
#include <math.h>
#include <cmath>
#include <utility>
#include <iostream>
#include <algorithm>

using namespace std;

extern "C" void copyToDouble(float* sourceArray, int size, double* targetArray) {

    for (int i = 0; i < size; i++) {
        targetArray[i] = sourceArray[i];
    }
}

extern "C" void copyToFloat(double* sourceArray, int size, float* targetArray) {

    double min = -FLT_MAX;
    double max = FLT_MAX;

    double limitedValue;
    for (int i = 0; i < size; i++) {
        // limiting the new value within the boundaries of float
        limitedValue = std::clamp(sourceArray[i], min, max);
        targetArray[i] = limitedValue;
    }
}


extern "C" void createInterleavedArray(float* concatenatedArrays, int channelCount, int channelLength, float* interleavedArray, bool applyGain, float* channelGainFactor) {

    // Takes a flattened matrix in which each channel is put after each other, and interleaves the channels values
    int targetIndex = 0;
    if (applyGain)
    {
        for (int s = 0; s < channelLength; s++) {
            for (int c = 0; c < channelCount; c++) {
                interleavedArray[targetIndex] = channelGainFactor[c] * concatenatedArrays[c * channelLength + s];
                targetIndex += 1;
            }
        }
    }
    else {
        for (int s = 0; s < channelLength; s++) {
            for (int c = 0; c < channelCount; c++) {
                interleavedArray[targetIndex] = concatenatedArrays[c * channelLength + s];
                targetIndex += 1;
            }
        }
   }
}

extern "C" void deinterleaveArray(float* interleavedArray, int channelCount, int channelLength, float* concatenatedArrays) {

    // Takes an interleaved sound sample array and deinterleaves it to a flattened matrix in which each channel is put after each other
    int targetIndex = 0;
    for (int s = 0; s < channelLength; s++) {
        for (int c = 0; c < channelCount; c++) {
            concatenatedArrays[c * channelLength + s] = interleavedArray[targetIndex];
            targetIndex += 1;
        }
    }
}


extern "C" int multiplyDoubleArray(double* values, int size, double factor) {
    return multiplyDoubleArraySection(values, size, factor, 0, size);
}

extern "C" int multiplyDoubleArraySection(double* values, int arraySize, double factor, int startIndex, int sectionLength) {

    int clippedCount = 0;
    double newValue = 0;
    double limitedValue = 0;
    double min = -DBL_MAX;
    double max = DBL_MAX;

    // Returning if arraySize was zero or below
    if (arraySize < 1)
    {
        return clippedCount;
    }

    // Limiting the start index to positive values and the length of the array
    startIndex = std::clamp(startIndex, 0, arraySize - 1);

    // Limiting sectionLength to positive values and the length of the array
    sectionLength = std::clamp(sectionLength, 0, arraySize - startIndex);

    for (int i = startIndex; i < startIndex + sectionLength; i++) {
        // calculating the new value
        newValue = values[i] * factor;

        // limiting the new value within the boundaries of float
        limitedValue = std::clamp(newValue, min, max);

        // Noting if clipping occurred
        if (newValue != limitedValue) {
            clippedCount++;
        }

        // Storing the limited value
        values[i] = limitedValue;

    }

    // Returns the number of clipped samples
    return clippedCount;
}


extern "C" int multiplyFloatArray(float* values, int size, float factor) {
    return multiplyFloatArraySection(values, size, factor, 0, size);
}

extern "C" int multiplyFloatArraySection(float* values, int arraySize, float factor, int startIndex, int sectionLength) {

    int clippedCount = 0;
    double newValue = 0;
    double limitedValue = 0;
    double min = -FLT_MAX;
    double max = FLT_MAX;

    // Returning if arraySize was zero or below
    if (arraySize < 1)
    {
        return clippedCount;
    }

    // Limiting the start index to positive values and the length of the array
    startIndex = std::clamp(startIndex, 0, arraySize - 1);

    // Limiting sectionLength to positive values and the length of the array
    sectionLength = std::clamp(sectionLength, 0, arraySize - startIndex);

    for (int i = startIndex; i < startIndex + sectionLength; i++) {
        // calculating the new value
        newValue = values[i] * factor;

        // limiting the new value within the boundaries of float
        limitedValue = std::clamp(newValue, min, max);

        // Noting if clipping occurred
        if (newValue != limitedValue) {
            clippedCount++;
        }

        // Storing the limited value
        values[i] = limitedValue;

    }

    // Returns the number of clipped samples
    return clippedCount;
}


extern "C" double calculateFloatSumOfSquare(float* values, int arraySize, int startIndex, int sectionLength) {

    // Limiting the start index to positive values and the length of the array
    startIndex = std::clamp(startIndex, 0, arraySize - 1);

    // Limiting sectionLength to positive values and the length of the array
    sectionLength = std::clamp(sectionLength, 0, arraySize - startIndex);

    double SumOfSquare = 0;
    float power = 2;

    for (int i = startIndex; i < startIndex + sectionLength; i++) {
        SumOfSquare += values[i] * values[i];
    }

    // Returning the sum of squares
    return SumOfSquare;

}


extern "C" void addTwoFloatArrays(float* array1, float* array2, int size) {

    for (int i = 0; i < size; i++) {
        array1[i] += array2[i];
    }

}


extern "C" void fft_complex(double* real, double* imag, int size, double* pccos, double* pcsin, int direction = 1, bool reorder = true, bool scaleForwardTransform = true) {

    // This is a modified C++ translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

    int ExponentSign;
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
        int m;

        for (int i = 0; i < size - 1; ++i) {
            if (i < j) {
                TempX = real[i];
                real[i] = real[j];
                real[j] = TempX;

                TempY = imag[i];
                imag[i] = imag[j];
                imag[j] = TempY;
            }

            m = size;
            do {
                m >>= 1;
                j ^= m;
            } while ((j & m) == 0);
        }
    }

    int halfSize = size / 2;

    // Defining some temporary variables to avoid definition inside the loop
    double aiX;
    double aiY;
    double Real1;
    double Imaginary1;
    double Real2;
    double Imaginary2;
    double TempReal1;
    int lookupIndex;
    double wX;
    double wY;
    int StepSize;
    int i;
    int LevelSize = 1;
    
    while (LevelSize < size) {

        StepSize = LevelSize << 1;

        for (int k = 0; k < LevelSize; ++k) {
            lookupIndex = (double)halfSize * ((double)k / (double)LevelSize);

            wX = pccos[lookupIndex];
            wY = pcsin[lookupIndex];

            i = k;
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
        //LevelSize = LevelSize << 1;

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

extern "C" void complexMultiplication(double* real1, double* imag1, double* real2, double* imag2, int size) {

    double tempValue;
    for (int i = 0; i < size; i++) {
        tempValue = real1[i]; //stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
        real1[i] = tempValue * real2[i] - imag1[i] * imag2[i];
        imag1[i] = tempValue * imag2[i] + imag1[i] * real2[i];
    }
}




