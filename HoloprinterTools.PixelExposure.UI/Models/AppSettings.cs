namespace HoloprinterTools.PixelExposure.UI.Models
{
    public class AppSettings
    {
        // User Input Tab Parameters
        public string Name { get; set; } = string.Empty;
        public double Length { get; set; } = 0.0;
        public double Width { get; set; } = 0.0;
        public double HeightOfBottomPainting { get; set; } = 0.0;
        public double HeightOfLightArray { get; set; } = 8.0;
        public int MinWavelength { get; set; } = 400;
        public int MaxWavelength { get; set; } = 700;

        // Printer Settings Modal Parameters
        // a. Printbed dimensions
        public double PrintbedWidth { get; set; } = 0.0;
        public double PrintbedLength { get; set; } = 0.0;

        // b. Exposure laser wavelength (in nm)
        public double LaserWavelength { get; set; } = 0.0;

        // c. Exposure Beam 1 angles (in degrees)
        public double Beam1MaxAngle { get; set; } = 53.0;
        public double Beam1MinAngle { get; set; } = 21.0;

        // d. Exposure Beam 2 angles (in degrees)
        public double Beam2MaxAngle { get; set; } = 51.0;
        public double Beam2MinAngle { get; set; } = -30.0;

        // e. Pixel size
        public double PixelSize { get; set; } = 0.0;

        // f. Beam power in mW
        public double Beam1Power { get; set; } = 0.0;
        public double Beam2Power { get; set; } = 0.0;

        // g. Settle time
        public double SettleTime { get; set; } = 0.0;

        // h. Minimum diffraction efficiency threshold (in %)
        public double MinDiffractionEfficiency { get; set; } = 60.0;

        // i. Maximum delta-n
        public double MaxDeltaN { get; set; } = 0.035;

        // j. Viewing parameters
        public double NominalViewingHeight { get; set; } = 0.0;
        public double MinimumViewingDistance { get; set; } = 0.0;
    }
}
