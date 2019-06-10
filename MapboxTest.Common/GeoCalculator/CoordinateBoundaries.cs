using System;

namespace MapboxTest.Common.GeoCalculator
{
    public class CoordinateBoundaries
    {
        private double _latitude;
        //TODO: These are the values from the general Latitude and Longitude parallel and median calculations. 
        private int _latitudeDistanceInMiles = 69;
        private int _latitudeDistanceInNauticalMiles = 60;
        private double _latitudeDistanceInKilometers = 111.045;
        private int _latitudeDistanceInMeters = 111045;

        
        public double Latitude
        {
            get => _latitude;

            set
            {
                _latitude = value;
                Calculate();
            }
        }

        private double _longitude;

        
        public double Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                Calculate();
            }
        }

        private double _distance;

       
        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                Calculate();
            }
        }

        private DistanceUnit _distanceUnit;

        
        public DistanceUnit DistanceUnit
        {
            get => _distanceUnit;
            set
            {
                _distanceUnit = value;
                Calculate();
            }
        }

       
        public double MaxLatitude { get; private set; }

        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        
        public double MinLongitude { get; private set; }

        public CoordinateBoundaries(Coordinate originCoordinate, double distance, DistanceUnit distanceUnit = DistanceUnit.Miles)
            : this(originCoordinate.Latitude, originCoordinate.Longitude, distance, distanceUnit) { }

        public CoordinateBoundaries(double latitude, double longitude, double distance, DistanceUnit distanceUnit = DistanceUnit.Miles)
        {
            if (!Validate(latitude, longitude))
                throw new ArgumentException("Invalid coordinates");

            _latitude = latitude;
            _longitude = longitude;
            _distance = distance;
            _distanceUnit = distanceUnit;

            Calculate();
        }

        private void Calculate()
        {
            if (!Validate(Latitude, Longitude))
                throw new ArgumentException("Invalid coordinates");

            double divisor = GetDivisor();

            double latitudeConversionFactor = Distance / divisor;
            double longitudeConversionFactor = Distance / divisor / Math.Abs(Math.Cos(Latitude.ToRadian()));

            MinLatitude = Latitude - latitudeConversionFactor;
            MaxLatitude = Latitude + latitudeConversionFactor;

            MinLongitude = Longitude - longitudeConversionFactor;
            MaxLongitude = Longitude + longitudeConversionFactor;

            // Adjust passing over coordinate boundaries
            if (MinLatitude < -90) MinLatitude = 90 - (-90 - MinLatitude);
            if (MaxLatitude > 90) MaxLatitude = -90 + (MaxLatitude - 90);

            if (MinLongitude < -180) MinLongitude = 180 - (-180 - MinLongitude);
            if (MaxLongitude > 180) MaxLongitude = -180 + (MaxLongitude - 180);
        }

        private double GetDivisor()
        {
            switch (_distanceUnit)
            {
                case DistanceUnit.NauticalMiles:
                    return _latitudeDistanceInNauticalMiles;
                case DistanceUnit.Kilometers:
                    return _latitudeDistanceInKilometers;
                case DistanceUnit.Meters:
                    return _latitudeDistanceInMeters;
                default:
                    return _latitudeDistanceInMiles;
            }
        }

        static bool Validate(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90) return false;
            if (longitude < -180 || longitude > 180) return false;

            return true;
        }
    }
}