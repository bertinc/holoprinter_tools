using System;
using HoloprinterTools.PixelExposure.Interop;

namespace HoloprinterTools.PixelExposure.Core.Services
{
    /// <summary>
    /// High-level wrapper for RCWA calculations
    /// Provides a clean C# API around the native rcwa_dll.dll functionality
    /// </summary>
    public class RcwaCalculator
    {
        /// <summary>
        /// Performs a wavelength sweep analysis
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
        /// <returns>Tuple containing (S-Pol array, P-Pol array)</returns>
        public static (double[] sPol, double[] pPol) WavelengthSweep(
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
            double theta)
        {
            // Calculate number of points based on wavelength range and step
            int numPoints = (int)((stopWavelength - startWavelength) / stepWavelength) + 1;
            double[] sPol = new double[numPoints];
            double[] pPol = new double[numPoints];

            try
            {
                RcwaDllInterop.wavelength_sweep(
                    spatialFreq,
                    effThickness,
                    deltaN,
                    bulkIndex,
                    glassIndex,
                    braggTilt,
                    harmonicOrder,
                    centerWavelength,
                    startWavelength,
                    stopWavelength,
                    stepWavelength,
                    theta,
                    sPol,
                    pPol);
            }
            catch (DllNotFoundException ex)
            {
                throw new InvalidOperationException(
                    "rcwa_dll.dll not found. Ensure the DLL is in the application directory or on the system path.",
                    ex);
            }
            catch (EntryPointNotFoundException ex)
            {
                throw new InvalidOperationException(
                    "wavelength_sweep function not found in rcwa_dll.dll. Verify DLL version.",
                    ex);
            }

            return (sPol, pPol);
        }

        /// <summary>
        /// Performs a modulation sweep analysis
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
        /// <returns>Tuple containing (S-Pol array, P-Pol array)</returns>
        public static (double[] sPol, double[] pPol) ModulationSweep(
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
            double theta)
        {
            // Calculate number of points based on delta range and step
            int numPoints = (int)((stopDelta - startDelta) / stepDelta) + 1;
            double[] sPol = new double[numPoints];
            double[] pPol = new double[numPoints];

            try
            {
                RcwaDllInterop.modulation_sweep(
                    spatialFreq,
                    effThickness,
                    deltaN,
                    bulkIndex,
                    glassIndex,
                    braggTilt,
                    harmonicOrder,
                    centerWavelength,
                    startDelta,
                    stopDelta,
                    stepDelta,
                    theta,
                    sPol,
                    pPol);
            }
            catch (DllNotFoundException ex)
            {
                throw new InvalidOperationException(
                    "rcwa_dll.dll not found. Ensure the DLL is in the application directory or on the system path.",
                    ex);
            }
            catch (EntryPointNotFoundException ex)
            {
                throw new InvalidOperationException(
                    "modulation_sweep function not found in rcwa_dll.dll. Verify DLL version.",
                    ex);
            }

            return (sPol, pPol);
        }

        /// <summary>
        /// Calculates the number of data points in a wavelength sweep
        /// </summary>
        public static int CalculateWavelengthPointCount(
            double startWavelength,
            double stopWavelength,
            double stepWavelength)
        {
            return (int)((stopWavelength - startWavelength) / stepWavelength) + 1;
        }

        /// <summary>
        /// Calculates the number of data points in a modulation sweep
        /// </summary>
        public static int CalculateModulationPointCount(
            double startDelta,
            double stopDelta,
            double stepDelta)
        {
            return (int)((stopDelta - startDelta) / stepDelta) + 1;
        }
    }
}
