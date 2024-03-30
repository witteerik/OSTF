#pragma once
#include <vector>

#define FFT_H __declspec(dllexport)

extern "C" FFT_H void copyToDouble(float* sourceArray, int size, double* targetArray);

extern "C" FFT_H void copyToFloat(double* sourceArray, int size, float* targetArray);

extern "C" FFT_H void createInterleavedArray(float* concatenatedArrays, int channelCount, int channelLength, float* interleavedArray, bool applyGain, float* channelGainFactor);

extern "C" FFT_H void deinterleaveArray(float* interleavedArray, int channelCount, int channelLength, float* concatenatedArrays);

extern "C" FFT_H int multiplyDoubleArray(double* values, int size, double factor);

extern "C" FFT_H int multiplyDoubleArraySection(double* values, int arraySize, double factor, int startIndex, int sectionLength);

extern "C" FFT_H int multiplyFloatArray(float* values, int size, float factor);

extern "C" FFT_H int multiplyFloatArraySection(float* values, int arraySize, float factor, int startIndex, int sectionLength);

extern "C" FFT_H double	calculateFloatSumOfSquare(float* values, int arraySize, int startIndex, int sectionLength);

extern "C" FFT_H void addTwoFloatArrays(
	float* array1, // Pointer to the first input/output data array. Upon return this corresponding data array contains the sum of the values in array1 and array2.
	float* array2, // Pointer to the input data array containing the values which should be added to array1. 
	int size // Size (i.e. number of values) of the input arrays (need to be equal between array1 and array2)
);

extern "C" FFT_H void fft_complex(
	double* real, // Pointer to the real part of the input/output data
	double* imag, // Pointer to the imaginary part of the input/output data
	int size, // Size (i.e. number of values) of the input/output data
	int direction, // Direction of the FFT (default: 1 (i.e. forward transform))
	bool reorder, // Whether to perform data reordering (default: true)
	bool scaleForwardTransform // Whether to scale the forward transform (default: true)
);

extern "C" FFT_H void complexMultiplication(double* real1, double* imag1, double* real2, double* imag2, int size);
