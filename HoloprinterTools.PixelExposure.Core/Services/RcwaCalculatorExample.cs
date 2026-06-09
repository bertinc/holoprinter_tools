using System;
using System.Collections.Generic;
using System.Linq;

namespace HoloprinterTools.PixelExposure.Core.Services
{
    /// <summary>
    /// Example usage of RcwaCalculator for performing wavelength and modulation sweep analyses.
    /// This demonstrates the proper calling pattern for the high-level managed wrapper API.
    /// </summary>
    public class RcwaCalculatorExample
    {
        /// <summary>
        /// Example: Perform a wavelength sweep analysis for a typical holographic grating.
        /// Returns efficiency data (S-Pol and P-Pol) across a wavelength range.
        /// </summary>
        public static void ExampleWavelengthSweep()
        {
            // Typical holographic grating parameters
            // These represent a photorefractive material used in optical data storage
            double spatialFreq = 1000.0;           // Spatial frequency (l/mm)
            double effThickness = 10.0;            // Effective thickness (µm)
            double deltaN = 0.05;                  // Refractive index modulation (Δn)
            double bulkIndex = 1.5;                // Bulk substrate refractive index
            double glassIndex = 1.52;              // Glass/optical element refractive index
            double braggTilt = 0.5;                // Bragg tilt angle (degrees)
            int harmonicOrder = 1;                 // Diffraction order

            // Wavelength sweep parameters
            double centerWavelength = 633.0;       // Center wavelength (nm) - red laser
            double startWavelength = 600.0;        // Start of range (nm)
            double stopWavelength = 680.0;         // End of range (nm)
            double stepWavelength = 2.0;           // Step size (nm)
            double theta = 45.0;                   // Incident angle (degrees)

            try
            {
                // Perform the wavelength sweep
                var (sPol, pPol) = RcwaCalculator.WavelengthSweep(
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
                    theta
                );

                // Process results
                Console.WriteLine($"Wavelength Sweep Results:");
                Console.WriteLine($"  S-Pol data points: {sPol.Length}");
                Console.WriteLine($"  P-Pol data points: {pPol.Length}");

                if (sPol.Length > 0)
                {
                    Console.WriteLine($"  S-Pol efficiency range: {sPol.Min():F2}% to {sPol.Max():F2}%");
                    Console.WriteLine($"  P-Pol efficiency range: {pPol.Min():F2}% to {pPol.Max():F2}%");
                }

                // In a real UI application, you would:
                // 1. Create chart data points from wavelength range and efficiency arrays
                // 2. Bind to a chart control (e.g., OxyPlot, LiveCharts)
                // 3. Display S-Pol (dashed red), P-Pol (dashed blue), and Average efficiency curves
                // 4. Add a vertical reference line at center wavelength
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during wavelength sweep: {ex.Message}");
            }
        }

        /// <summary>
        /// Example: Perform a modulation sweep analysis to find optimal refractive index modulation.
        /// Returns efficiency data across a range of ΔN values.
        /// </summary>
        public static void ExampleModulationSweep()
        {
            // Same grating parameters as wavelength sweep
            double spatialFreq = 1000.0;
            double effThickness = 10.0;
            double deltaN = 0.05;                  // Center modulation depth
            double bulkIndex = 1.5;
            double glassIndex = 1.52;
            double braggTilt = 0.5;
            int harmonicOrder = 1;

            // Modulation sweep parameters
            double centerWavelength = 633.0;
            double theta = 45.0;
            double startDeltaN = 0.0;              // Start of modulation range
            double stopDeltaN = 0.15;              // End of modulation range
            double stepDeltaN = 0.001;             // Step size

            try
            {
                // Perform the modulation sweep
                var (sPol, pPol) = RcwaCalculator.ModulationSweep(
                    spatialFreq,
                    effThickness,
                    deltaN,
                    bulkIndex,
                    glassIndex,
                    braggTilt,
                    harmonicOrder,
                    centerWavelength,
                    startDeltaN,
                    stopDeltaN,
                    stepDeltaN,
                    theta
                );

                // Process results
                Console.WriteLine($"Modulation Sweep Results:");
                Console.WriteLine($"  S-Pol data points: {sPol.Length}");
                Console.WriteLine($"  P-Pol data points: {pPol.Length}");

                if (sPol.Length > 0)
                {
                    // Find optimal ΔN where S-Pol and P-Pol are balanced
                    int maxSPolIdx = Array.IndexOf(sPol, sPol.Max());
                    int maxPPolIdx = Array.IndexOf(pPol, pPol.Max());

                    double optimalDeltaN = startDeltaN + (maxSPolIdx + maxPPolIdx) / 2.0 * stepDeltaN;

                    Console.WriteLine($"  Optimal ΔN (approximate): {optimalDeltaN:F4}");
                    Console.WriteLine($"  S-Pol efficiency range: {sPol.Min():F2}% to {sPol.Max():F2}%");
                    Console.WriteLine($"  P-Pol efficiency range: {pPol.Min():F2}% to {pPol.Max():F2}%");
                }

                // In a real UI application, you would:
                // 1. Create chart data points from ΔN range and efficiency arrays
                // 2. Bind to a chart control
                // 3. Display S-Pol and P-Pol curves
                // 4. Find and highlight the optimal ΔN crossing point
                // 5. Update the main wavelength sweep with the optimal ΔN value
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during modulation sweep: {ex.Message}");
            }
        }

        /// <summary>
        /// Example: Combined workflow - perform modulation sweep to find optimal ΔN,
        /// then perform wavelength sweep with that optimal value.
        /// </summary>
        public static void ExampleOptimizedWorkflow()
        {
            // Grating parameters
            double spatialFreq = 1000.0;
            double effThickness = 10.0;
            double deltaN = 0.05;                  // Starting/center modulation depth
            double bulkIndex = 1.5;
            double glassIndex = 1.52;
            double braggTilt = 0.5;
            int harmonicOrder = 1;
            double centerWavelength = 633.0;
            double theta = 45.0;

            try
            {
                // Step 1: Run modulation sweep to find optimal ΔN
                Console.WriteLine("Step 1: Finding optimal ΔN via modulation sweep...");
                var (modSPol, modPPol) = RcwaCalculator.ModulationSweep(
                    spatialFreq, effThickness, deltaN, bulkIndex, glassIndex,
                    braggTilt, harmonicOrder, centerWavelength,
                    0.0, 0.15, 0.001, theta
                );

                // Find the crossing point or balanced efficiency
                double optimalDeltaN = 0.05;  // Default fallback
                if (modSPol.Length > 0 && modPPol.Length > 0)
                {
                    // Simple heuristic: find where average efficiency is maximized
                    double[] avgEfficiency = modSPol.Zip(modPPol, (s, p) => (s + p) / 2.0).ToArray();
                    int optimalIdx = Array.IndexOf(avgEfficiency, avgEfficiency.Max());
                    optimalDeltaN = 0.0 + optimalIdx * 0.001;

                    Console.WriteLine($"  Optimal ΔN found: {optimalDeltaN:F4}");
                }

                // Step 2: Run wavelength sweep with optimal ΔN
                Console.WriteLine("Step 2: Running wavelength sweep with optimized ΔN...");
                var (wlSPol, wlPPol) = RcwaCalculator.WavelengthSweep(
                    spatialFreq, effThickness, optimalDeltaN, bulkIndex, glassIndex,
                    braggTilt, harmonicOrder, centerWavelength,
                    600.0, 680.0, 2.0, theta
                );

                Console.WriteLine($"  Wavelength sweep completed: {wlSPol.Length} points");
                if (wlSPol.Length > 0)
                {
                    double avgEff = (wlSPol.Average() + wlPPol.Average()) / 2.0;
                    Console.WriteLine($"  Average efficiency: {avgEff:F2}%");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error in optimized workflow: {ex.Message}");
            }
        }
    }
}
