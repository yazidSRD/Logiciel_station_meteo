using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet23_Station_météo_WPF.code
{
    public class unitConversion
    {
        public Int32 Conversion(Int32 value, string unit, Dictionary<string, Int32> values)
        {
            switch (unit)
            {
                case "°C":
                    return value;
                case "°F":
                    return CelsiusToFahrenheit(value);
                case "K":
                    return CelsiusToKelvin(value);



                case "%":
                    return value;
                case "g/m³":
                    return VaporDensity(value, values["Temperature"], values["PressionAtmospherique"]);
                case "pr":
                    return PointDeRosee(value, values["Temperature"]);



                case "m/s":
                    return value;
                case "Km/h":
                    return ConvertirMSEnKmH(value);
                case "mph":
                    return ConvertirMSEnMph(value);
                case "nd":
                    return ConvertirMSEnNoeuds(value);



                case "hPa":
                    return value;
                case "atm":
                    return ConvertirhPaEnAtm(value);
                case "psi":
                    return ConvertirhPaEnPsi(value);
                case "bar":
                    return ConvertirhPaEnBar(value);



                case "mm":
                    return value;
                case "cm":
                    return ConvertirMmEnCm(value);
                case "in":
                    return ConvertirMmEnIn(value);
                case "L/m²":
                    return ConvertirMmEnLm2(value);



                case "W/m²":
                    return value;
                case "kW/m²":
                    return RadiationToKW(value);
                case "UV":
                    return RadiationToUV(value);
                case "Btu/h.ft²":
                    return RadiationToBtu(value);
                default: return value;
            }
        }
        private Int32 CelsiusToFahrenheit(Int32 celsius)
        {
            return Convert.ToInt32(celsius * 1.8f + 32f);
        }
        private Int32 CelsiusToKelvin(Int32 celsius)
        {
            return Convert.ToInt32(celsius + 273.15f);
        }
        private Int32 VaporDensity(Int32 humidity, Int32 temperature, Int32 pressure)
        {
            // Calcul de la pression partielle de la vapeur d'eau
            double saturationPressure = 6.1078 * Math.Pow(10, ((7.5 * temperature) / (temperature + 237.3)));
            double vaporPressure = (humidity / 100) * saturationPressure;

            // Calcul de la densité de vapeur d'eau
            double gasConstant = 287.058; // constante des gaz parfaits en J/(kg.K)
            double waterMolecularWeight = 0.0180153; // poids moléculaire de l'eau en kg/mol
            double vaporDensity = (vaporPressure * waterMolecularWeight) / (gasConstant * temperature);

            // Correction pour la pression atmosphérique
            double correctionFactor = pressure / 101325;
            vaporDensity = vaporDensity * correctionFactor;
            return Convert.ToInt32(vaporDensity*1000);
        }
        private Int32 PointDeRosee(Int32 humiditeRelative, Int32 temperature)
        {
            // Constantes utilisées dans la formule
            const double a = 17.27;
            const double b = 237.7;

            // Calcul de la température de rosée en degrés Celsius
            double alpha = ((a * temperature) / (b + temperature)) + Math.Log(humiditeRelative / 100.0);
            double temperatureDeRosee = (b * alpha) / (a - alpha);

            return Convert.ToInt32(temperatureDeRosee);
        }
        private Int32 ConvertirMSEnKmH(Int32 vitesseMS)
        {
            // Conversion en Km/h
            double vitesseKmH = vitesseMS * 3.6;

            return Convert.ToInt32(vitesseKmH);
        }
        private Int32 ConvertirMSEnMph(Int32 vitesseMS)
        {
            // Conversion en mph
            double vitesseMph = vitesseMS * 2.23694;

            return Convert.ToInt32(vitesseMph); ;
        }
        private Int32 ConvertirMSEnNoeuds(Int32 vitesseMS)
        {
            // Conversion en noeuds
            double vitesseNoeuds = vitesseMS * 1.94384;

            return Convert.ToInt32(vitesseNoeuds);
        }
        private Int32 ConvertirhPaEnAtm(Int32 hpa)
        {
            const double atmPerHpa = 0.000986923; // Nombre d'atmosphères par hPa
            return Convert.ToInt32(hpa * atmPerHpa);
        }
        private Int32 ConvertirhPaEnPsi(Int32 hpa)
        {
            const double barPerHpa = 0.001; // Nombre de bars par hPa
            return Convert.ToInt32(hpa * barPerHpa);
        }
        private Int32 ConvertirhPaEnBar(Int32 pressionPa)
        {
            // Conversion en bars
            double pressionBar = pressionPa / 100000.0;

            return Convert.ToInt32(pressionBar);
        }
        private Int32 ConvertirMmEnCm(Int32 pluviometrieMm)
        {
            // Conversion en cm
            double pluviometrieCm = pluviometrieMm / 10.0;

            return Convert.ToInt32(pluviometrieCm);
        }
        private Int32 ConvertirMmEnIn(Int32 pluviometrieMm)
        {
            // Conversion en in
            double pluviometrieIn = pluviometrieMm / 25.4;

            return Convert.ToInt32(pluviometrieIn);
        }
        private Int32 ConvertirMmEnLm2(Int32 pluviometrieMm)
        {
            // Conversion en L/m²
            double pluviometrieLm2 = pluviometrieMm * 0.1;

            return Convert.ToInt32(pluviometrieLm2);
        }
        private Int32 RadiationToUV(Int32 radiation)
        {
            // Coefficients pour estimer l'indice UV à partir de la radiation solaire
            double a = 1.0 / 60.0;
            double b = 1.3 / 1000.0;

            // Calcul de l'indice UV approximatif
            double uv = a * radiation + b;

            return Convert.ToInt32(uv);
        }
        private Int32 RadiationToKW(Int32 radiation)
        {
            return Convert.ToInt32(radiation / 1000.0);
        }
        private Int32 RadiationToBtu(Int32 radiation)
        {
            // Conversion de W/m² en W/ft²
            double radiation_ft2 = radiation * 10.764;

            // Conversion de W/ft² en Btu/h.ft²
            double btu = radiation_ft2 * 3.413;

            return Convert.ToInt32(btu);
        }
    }
}
