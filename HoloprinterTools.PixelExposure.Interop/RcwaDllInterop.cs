using System.Runtime.InteropServices;

namespace HoloprinterTools.PixelExposure.Interop
{
    /// <summary>
    /// P/Invoke wrapper for rcwa_dll.dll
    /// Provides direct access to native RCWA (Rigorous Coupled-Wave Analysis) calculations
    /// </summary>
    public static class RcwaDllInterop
    {
        /// <summary>
        /// Performs wavelength sweep analysis across a range of wavelengths
        /// </summary>
        /// <param name="spatialFreq">Spatial frequency (l/mm)</param>
        /// <param name="effThickness">Effective thickness (µm)</param>
        /// <param name="deltaN">Refractive index modulation (Δn)</param>
        /// <param name="bulkIndex">Bulk substrate refractive index</param>
        /// <param name="glassIndex">Glass/optical element refractive index</param>
        /// <param name="braggTilt">Bragg tilt angle (degrees)</param>
        /// <param name="harmonicOrder">Diffraction order (typically 0 or 1)</param>
        /// <param name="centerWavelength">Center wavelength (nm)</param>
        /// <param name="startWavelength">Start of wavelength range (nm)</param>
        /// <param name="stopWavelength">End of wavelength range (nm)</param>
        /// <param name="stepWavelength">Wavelength step size (nm)</param>
        /// <param name="theta">Incident angle (degrees)</param>
        /// <param name="returnSPol">Output array for S-polarization efficiency values</param>
        /// <param name="returnPPol">Output array for P-polarization efficiency values</param>
        [DllImport("rcwa_dll.dll", CharSet = CharSet.Ansi)]
        public static extern void wavelength_sweep(
            double spatialFreq,
            double effThickness,
            double deltaN,
            double bulkIndex,
            double glassIndex,
            double braggTilt,
            int harmonicOrder,
            double centerWavelength,
            double startWavelength,
            double stopWavelength,
            double stepWavelength,
            double theta,
            [In, Out] double[] returnSPol,
            [In, Out] double[] returnPPol);

        /// <summary>
        /// Performs modulation sweep analysis across a range of delta-N values
        /// </summary>
        /// <param name="spatialFreq">Spatial frequency (l/mm)</param>
        /// <param name="effThickness">Effective thickness (µm)</param>
        /// <param name="deltaN">Refractive index modulation (Δn)</param>
        /// <param name="bulkIndex">Bulk substrate refractive index</param>
        /// <param name="glassIndex">Glass/optical element refractive index</param>
        /// <param name="braggTilt">Bragg tilt angle (degrees)</param>
        /// <param name="harmonicOrder">Diffraction order (typically 0 or 1)</param>
        /// <param name="centerWavelength">Center wavelength (nm)</param>
        /// <param name="startDelta">Start of delta-N range</param>
        /// <param name="stopDelta">End of delta-N range</param>
        /// <param name="stepDelta">Delta-N step size</param>
        /// <param name="theta">Incident angle (degrees)</param>
        /// <param name="returnSPol">Output array for S-polarization efficiency values</param>
        /// <param name="returnPPol">Output array for P-polarization efficiency values</param>
        [DllImport("rcwa_dll.dll", CharSet = CharSet.Ansi)]
        public static extern void modulation_sweep(
            double spatialFreq,
            double effThickness,
            double deltaN,
            double bulkIndex,
            double glassIndex,
            double braggTilt,
            int harmonicOrder,
            double centerWavelength,
            double startDelta,
            double stopDelta,
            double stepDelta,
            double theta,
            [In, Out] double[] returnSPol,
            [In, Out] double[] returnPPol);
    }
}
