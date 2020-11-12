using System;

namespace ModelLibrary
{
    public struct AtmosphereParameters
        {
            public double Pressure { get; private set; }
            public double Density { get; private set; }
            public double SoundVelosity { get; private set; }
            public AtmosphereParameters(double Pressure, double Density, double SoundVelosity)
                : this()
            {
                this.Pressure = Pressure;
                this.Density = Density;
                this.SoundVelosity = SoundVelosity;
            }
        }
    public static class Atmosphere
    {
        public static AtmosphereParameters ParametersAtHeight(double Height)
        {
            double Temperature;
            double Pressure;
            if (Height <= 11E3)
            {
                Temperature = 293 - 6.5E-3 * Height;
                Pressure = 101325 * Math.Pow(293 / Temperature, -9.8 * 28E-3 / (8.31 * 6.5E-3));
            }
            else if (Height <= 20E3)
            {
                Temperature = 216.65;
                Pressure = 22632 * Math.Exp(-9.8 * 28E-3 * (Height - 11E3) / (8.31 * Temperature));
            }
            else if (Height <= 32E3)
            {
                Temperature = 216.65 + 1E-3 * (Height - 20E3);
                Pressure = 5474.899 * Math.Pow(216.65 / Temperature, 9.8 * 28E-3 / 8.31E-3);
            }
            else if (Height <= 47E3)
            {
                Temperature = 228.65 + 2.8E-3 * (Height - 32E3);
                Pressure = 868.0187 * Math.Pow(228.65 / Temperature, 9.8 * 28E-3 / (8.31 * 2.8E-3));
            }
            else if (Height < 51E3)
            {
                Temperature = 110.9063;
                Pressure = 270.65 * Math.Exp(-9.8 * 28E-3 * (Height - 47E3) / (8.31 * 110.9063));
            }
            else if (Height < 71E3)
            {
                Temperature = 270.65 - 2.8E-3 * (Height - 51E3);
                Pressure = 66.93887 * Math.Pow(270.65 / Temperature, -9.8 * 28E-3 / (8.31 * 2.8E-3));
            }
            else if (Height <= 178E3)
            {
                Temperature = 214.65 - 2E-3 * (Height - 71E3);
                Pressure = 3.956420 * Math.Pow(214.65 / Temperature, -9.8 * 28E-3 / (8.31 * 2E-3));
            }
            else
            {
                Temperature = 0.001;
                Pressure = 0;
            }
            return new AtmosphereParameters(Pressure, Pressure * 28.9644E-3 / (8.314 * Temperature), 20.046796 * Math.Sqrt(Temperature));
        }
    }
}