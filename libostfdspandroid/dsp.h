#pragma once
#include <unordered_map>
using namespace std;

extern "C" {

	void copyToDouble(float* sourceArray, int size, double* targetArray);

	void copyToFloat(double* sourceArray, int size, float* targetArray);

	void createInterleavedArray(float* concatenatedArrays, int channelCount, int channelLength, float* interleavedArray, bool applyGain, float* channelGainFactor);

	void deinterleaveArray(float* interleavedArray, int channelCount, int channelLength, float* concatenatedArrays);

	int multiplyDoubleArray(double* values, int size, double factor);

	int multiplyDoubleArraySection(double* values, int arraySize, double factor, int startIndex, int sectionLength);

	int multiplyFloatArray(float* values, int size, float factor);

	int multiplyFloatArraySection(float* values, int arraySize, float factor, int startIndex, int sectionLength);

	double	calculateFloatSumOfSquare(float* values, int arraySize, int startIndex, int sectionLength);

	void addTwoFloatArrays(
		float* array1, // Pointer to the first input/output data array. Upon return this corresponding data array contains the sum of the values in array1 and array2.
		float* array2, // Pointer to the input data array containing the values which should be added to array1. 
		int size // Size (i.e. number of values) of the input arrays (need to be equal between array1 and array2)
	);


	void fft_complex(
		double* real, // Pointer to the real part of the input/output data
		double* imag, // Pointer to the imaginary part of the input/output data
		int size, // Size (i.e. number of values) of the input/output data
		double* pccos, // Pointer to an array of precalculated cosine values used in the FFT
		double* pcsin, // Pointer to an array of precalculated sine values used in the FFT
		int direction, // Direction of the FFT (default: 1 (i.e. forward transform))
		bool reorder, // Whether to perform data reordering (default: true)
		bool scaleForwardTransform // Whether to scale the forward transform (default: true)
	);

	void complexMultiplication(double* real1, double* imag1, double* real2, double* imag2, int size);

}





