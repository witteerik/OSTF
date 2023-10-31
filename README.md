# Open Speech Test Framework (OSTF)

The Open Speech Test Framework (OSTF) is an open software library specifically developed for the creation speech audiometric test materials, as well as for running actual speech audiometric tests. The aim of the OSTF is to be a useful tool in speech and hearing research. However, the OSTF is not indended for medical use, and is not a medical device.

The OSTF is still in active development, and will undergo significant changed in the near future. If you decide to try it out, don't expect everything to work quite yet(!), and make sure to do frequent git-pulls to make sure you have the latest version.

Except for the 3:rd partly libraries listed below, the OSTF source code was developed by Erik Witte, Reg. Audiologist, PhD, Örebro University, Sweden, in cooperation with Örebro Audiological Research Center, and Linköping University, Sweden.

# Documentation
... is to come, but for now, in order to get an OSTF application even started you will need to specify the path to the folder "OSTFMedia" in the file "local_settings.txt" located in each project folder (of the application you attempt to start). You may place the OSTFMedia folder anywhere on your computer, as long as you point to it in the "local_settings.txt" file. (Preferably don't put the OSTFMedia folder on a place which is synched to a remote network as that could make the application very slow. A suggestion is to use "MediaRootDirectory = C:\OSTFMedia", and then you will of course also have to copy the OSTFMedia folder and its containing files there).

# Terms of use
The OSTF source code (available at https://github.com/witteerik/OSTF) is licensed under the following terms:

MIT/X11 License

Copyright (c) 2023 Erik Witte

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

The OSTF uses the MAUI library CommunityToolkit.Maui.MediaElement, which is generally under the MIT license, but some of its dependencies are licensed under the Apache-2.0 license (which should be compatible with the MIT license).

The OSTF also uses the following 3:rd party software libraries licenced under the MIT/X11 or a compatible license.

 - PortAudio. 
The OSTF uses PortAudio Portable Real-Time Audio Library, v. 19, 
Copyright (c) 1999-2011, Ross Bencina and Phil Burk.

- MathNet. 
The OSTF uses MathNet.Numerics, v 5.0.0,
Copyright (c), Christoph Ruegg, Marcus Cuda, Jurgen Van Gael.

- InTheHand. 
The OSTF uses InTheHand.Net.Bluetooth, 4.0.4,
Copyright (c), Peter Foot.

The OSTF repository also contains the following sound file collections which are licensed SEPARATELY under the licenses indicated below:

- Wierstorf et al. (2011). 
The set of room impulse responses shipped with this software was developed by Wierstorf et al. (2011) and is separately licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 license. This means NOT FOR COMMERCIAL USE! If the source code or software itself is to be used commersially, these recordings must first be removed from the distributed files!
http://creativecommons.org/licenses/by-nc-sa/3.0/
Copyright (c) 2011 
Quality & Usability Lab 
Assessment of IP-based Applications
Deutsche Telekom Laboratories, TU Berlin
Ernst-Reuter-Platz 7, 10587 Berlin, Germany

- SiP-test recordings. 
The OSTF uses a set of recordings for the Swedish SiP-test (available at https://osf.io/q4rb3/), These recordings are separately licensed under the Creative Commons Attribution-NonCommercial 4.0 International License. This means NOT FOR COMMERCIAL USE! If the source code or software itself is to be used commersially, these recordings must first be removed from the distributed files!
http://creativecommons.org/licenses/by-nc/4.0/ .
Copyright (c) Erik Witte

- A modified version of "The International Female Fluctuating Masker" (IFFM).
The OSTF uses a modified version (resampled to 48 kHz, float32 format, file name: "IFFM_60s_32bit_48k.wav") of the IFFM signal. The modifed IFFM signal is NOT free to use, except for under the following terms, which are the same as for the original IFFM signal, namely a) the copyright of the modified IFFM signal belongs to the European Hearing Instrument Manufacturers Association (EHIMA) and b) the IFFM modified signal can be used free of cost in the "field of hearing". For use in other fields, permission must be obtained from EHIMA. (For further information, see EHIMA's documentation for the IFFM and IFnoise at www.ehima.com/documents.) For further information about the modified IFFM signal, see the ReadMe file supplied in the same folders as the signal.

- Modified versions of the recordings for the Swedish Hearing In Noise Test (HINT), originally created by Hällgren, Larsby and Arlinger and available from https://doi.org/10.17605/OSF.IO/4ZNCK. Like the original, our modified verison of the HINT recordings are separately licensed under the Creative Commons Attribution 4.0 International License https://creativecommons.org/licenses/by/4.0/. Our modifications are solely technical and do not notably alter the audio quality or the relative sound levels within the original recordings.
The following modifications have been made:
     - Conversion from the original 16-bit encoding to 32-bit IEEE floating point encoding.
     - Splitting left and right channels into separate sound files.
     - Addition of Speech Material Annotation (SMA) iXML wave chunks for linguistic segmentations of the sound files.

- Sets of different HRTF recordings from The Audiological Research Centre in Örebro, Sweden. These recordings are available from https://osf.io/89n34/ under a CC-BY 4.0 International License https://creativecommons.org/licenses/by/4.0/. Copyright (c) 2023, Tobias Åslund, Tobias Danneleit and Erik Witte (for different sets, and modifications).

- A selection of HRTF recordings from the Oldenburg Hearing Device (OlHeaD) HRTF Database, available from https://uol.de/mediphysik/downloads/hearingdevicehrtfs under the CC BY-NC-SA 4.0 license. 
     - F. Denk, S.M.A. Ernst, S.D. Ewert and B. Kollmeier (2018) Adapting hearing devices to the individual ear acoustics: Database and target response correction functions for various device styles. Trends in Hearing, vol 22, p. 1-19. DOI: 10.1177/2331216518779313