ABOUT THE FILE "IFFM_60s_32bit_48k.wav"

The wave file "IFFM_60s_32bit_48k.wav" located in the current folder contains a MODIFIED VERSION of the original "International Female Fluctuating Masker" (IFFM) signal. The modified version has a sample rate of 48 kHz and a bit depth of 32 bits (float32).

Please note that, although the acoustic properties of the modified signal are very similar to those of the original, the two signals are not entirely identical. Therefore, the modified signal CANNOT be used as a fully equivalent replacement for the original signal.

TERMS OF USE:
The modified version of the IFFM signal falls under the same terms of use as the original IFFM signal, namely a) the copyright of the modified signal belongs to the European Hearing Instrument Manufacturers Association (EHIMA) and b) the modified signal can be used free of cost in the "field of hearing". For use in other fields, permission must be obtained from EHIMA. (For further information, see EHIMA's documentation for the IFFM and IFnoise at www.ehima.com/documents.)

The modified version of the IFFM was produced by Erik Witte with permission from the European Hearing Instrument Manufacturers Association (EHIMA).

MODIFICATION DESCRIPTION:
The modification of the IFFM signal was produced using the resampling software ResampAudio, version 10r0 (2017-06-05), run on Windows10 (64-bit Enterprise), with the following command run from the Windows Command Prompt, where "C:\Signals\IFFM-V1.0_60s_16bit.wav" refers to the original IFFM signal, downloaded (2022-09-24) from EHIMA's website (www.ehima.com/wp-content/uploads/2016/06/IFFM_and_IFnoise.zip).

The following command prompt input was used:
ResampAudio -s 48000 -F WAVE-NOEX -D float32 "C:\Signals\IFFM-V1.0_60s_16bit.wav" "C:\Signals\IFFM_48k\IFFM_60s_32bit_48k.wav"

Below is the command Prompt output:
 WAVE file: C:\Signals\IFFM-V1.0_60s_16bit.wav
   Description: IFFM-V1.0, 60 s, 16 bit
   Samples / channel : 2400307 (54.43 s)  07.04.2008
   Sampling frequency: 44100 Hz
   Number of channels: 1 (16-bit integer)

 Interpolation filter:
   ratio: 24, cutoff: 0.5, alpha: 7.85726, gain = 24
   delay: 816, no. coeffs: 1633, offset: 0, span: 1632

 WAVE file: C:\Signals\IFFM_48k\IFFM_60s_32bit_48k.wav
   Samples / channel : 2612579 (54.43 s)  2022-09-30 09:37:40 UTC
   Sampling frequency: 48000 Hz
   Number of channels: 1 (32-bit float)
 Sampling ratio: 160/147