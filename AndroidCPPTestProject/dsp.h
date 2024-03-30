#pragma once

#ifdef __cplusplus
extern "C" {
#endif

	static void fft_complex(
		double* real, // Pointer to the real part of the input/output data
		double* imag, // Pointer to the imaginary part of the input/output data
		int size, // Size (i.e. number of values) of the input/output data
		int direction, // Direction of the FFT (default: 1 (i.e. forward transform))
		bool reorder, // Whether to perform data reordering (default: true)
		bool scaleForwardTransform // Whether to scale the forward transform (default: true)
	);

#ifdef __cplusplus
}
#endif