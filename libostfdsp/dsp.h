#pragma once
#include <vector>

#define FFT_H __declspec(dllexport)

extern "C" FFT_H void multiplyArrayBy(float* values, int size, float factor);

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