namespace ShippingQuote
{
    public class Measurement
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }

        public MeasurementSystem MeasurementSystem { get; }

        public Measurement(float height, float width, float length, MeasurementSystem measurementSystem)
        {
            this.Height = height;
            this.Width = width;
            this.Length = length;
            this.MeasurementSystem = measurementSystem;
        }

        public Measurement ConvertTo(MeasurementSystem measurementSystem)
        {
            if (MeasurementSystem == this.MeasurementSystem)
            {
                return this;
            }

            switch (measurementSystem)
            {
                case MeasurementSystem.MetricMetres:
                    return ConvertToMetricMetres();
                case MeasurementSystem.MetricCentimetre:
                    return ConvertToMetricCentimetre();
                case MeasurementSystem.ImperialFeet:
                    return ConvertToImperialFeet();
                case MeasurementSystem.ImperialInches:
                    return ConvertToImperialInches();
            }
            throw new System.Exception("Incorrect Measurement System");
        }

        private Measurement ConvertToMetricMetres()
        {
            switch (this.MeasurementSystem)
            {
                case MeasurementSystem.MetricCentimetre:
                    return new Measurement(this.Height * 100, this.Width * 100, this.Length * 100, MeasurementSystem.MetricMetres);
                case MeasurementSystem.ImperialFeet:
                    return new Measurement(this.Height * 0.3048f, this.Width * 0.3048f, this.Length * 0.3048f, MeasurementSystem.MetricMetres);                    
                case MeasurementSystem.ImperialInches:
                    return new Measurement(this.Height * 0.0254f, this.Width * 0.0254f, this.Length * 0.0254f, MeasurementSystem.MetricMetres);
            }
            throw new System.Exception("Incorrect Measurement System");
        }

        private Measurement ConvertToMetricCentimetre()
        {
            switch (this.MeasurementSystem)
            {
                case MeasurementSystem.MetricMetres:
                    return new Measurement(this.Height * 0.01f, this.Width * 0.01f, this.Length * 0.01f, MeasurementSystem.MetricCentimetre);
                case MeasurementSystem.ImperialFeet:
                    return new Measurement(this.Height * 30.48f, this.Width * 30.48f, this.Length * 30.48f, MeasurementSystem.MetricCentimetre);                    
                case MeasurementSystem.ImperialInches:
                    return new Measurement(this.Height * 2.54f, this.Width * 2.54f, this.Length * 2.54f, MeasurementSystem.MetricCentimetre);
            }
            throw new System.Exception("Incorrect Measurement System");
        }

        private Measurement ConvertToImperialFeet()
        {
            switch (this.MeasurementSystem)
            {
                case MeasurementSystem.MetricMetres:
                    return new Measurement(this.Height * 3.2808f, this.Width * 3.2808f, this.Length * 3.2808f, MeasurementSystem.ImperialFeet);
                case MeasurementSystem.MetricCentimetre:
                    return new Measurement(this.Height * 0.0328f, this.Width * 0.0328f, this.Length * 0.0328f, MeasurementSystem.ImperialFeet);                    
                case MeasurementSystem.ImperialInches:
                    return new Measurement(this.Height * 0.0833f, this.Width * 0.0833f, this.Length * 0.0833f, MeasurementSystem.ImperialFeet);
            }
            throw new System.Exception("Incorrect Measurement System");
        }

        private Measurement ConvertToImperialInches()
        {
            switch (this.MeasurementSystem)
            {
                case MeasurementSystem.MetricMetres:
                    return new Measurement(this.Height * 39.37f, this.Width * 39.37f, this.Length * 39.37f, MeasurementSystem.ImperialInches);
                case MeasurementSystem.MetricCentimetre:
                    return new Measurement(this.Height * 0.3937f, this.Width * 0.3937f, this.Length * 0.3937f, MeasurementSystem.ImperialInches);                    
                case MeasurementSystem.ImperialFeet:
                    return new Measurement(this.Height * 12f, this.Width * 12f, this.Length * 12f, MeasurementSystem.ImperialInches);
            }
            throw new System.Exception("Incorrect Measurement System");
        }
    }
}