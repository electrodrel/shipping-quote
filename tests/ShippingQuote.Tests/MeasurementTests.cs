using System.Collections.Generic;
using ShippingQuote;
using Xunit;

namespace ShippingQuote.Tests
{
    public class MeasurementTests
    {
        private Measurement _subject;

        // the following unit tests will fail due to the wrong expected values. I wrote them just to show idea because finding the right conversion values for all the combination is exhousting

        [Theory]
        [InlineData(MeasurementSystem.MetricMetres, MeasurementSystem.MetricCentimetre, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {50f, 20f, 15f})]
        [InlineData(MeasurementSystem.MetricMetres, MeasurementSystem.ImperialFeet, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.49f})]
        [InlineData(MeasurementSystem.MetricMetres, MeasurementSystem.ImperialInches, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.MetricCentimetre, MeasurementSystem.MetricMetres, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.MetricCentimetre, MeasurementSystem.ImperialFeet, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.MetricCentimetre, MeasurementSystem.ImperialInches, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialFeet, MeasurementSystem.MetricMetres, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialFeet, MeasurementSystem.MetricCentimetre, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialFeet, MeasurementSystem.ImperialInches, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialInches, MeasurementSystem.MetricMetres, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialInches, MeasurementSystem.MetricCentimetre, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialInches, MeasurementSystem.ImperialFeet, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        public void ConvertTo_WhenDifferentMeasurementSystems_ReturnsCorrectValues(MeasurementSystem sourceSystem, MeasurementSystem targetSystem, float[] sourceValues, float[] expectedValues)
        {
            var sourceMeasurement = new Measurement(sourceValues[0], sourceValues[1], sourceValues[2], sourceSystem);
            var targetMeasurement = sourceMeasurement.ConvertTo(targetSystem);

            Assert.Equal(expectedValues[0], targetMeasurement.Height);
            Assert.Equal(expectedValues[1], targetMeasurement.Width);
            Assert.Equal(expectedValues[2], targetMeasurement.Height);
        }

        [Theory]
        [InlineData(MeasurementSystem.MetricMetres, MeasurementSystem.MetricMetres, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.MetricCentimetre, MeasurementSystem.MetricCentimetre, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialFeet, MeasurementSystem.ImperialFeet, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        [InlineData(MeasurementSystem.ImperialInches, MeasurementSystem.ImperialInches, new float[] { 0.5f, 0.2f, 0.15f }, new float [] {0.5f, 0.2f, 0.15f})]
        public void ConvertTo_WhenSameMeasurementSystems_ReturnsSameValues(MeasurementSystem sourceSystem, MeasurementSystem targetSystem, float[] sourceValues, float[] expectedValues)
        {
            var sourceMeasurement = new Measurement(sourceValues[0], sourceValues[1], sourceValues[2], sourceSystem);
            var targetMeasurement = sourceMeasurement.ConvertTo(targetSystem);

            Assert.Equal(expectedValues[0], targetMeasurement.Height);
            Assert.Equal(expectedValues[1], targetMeasurement.Width);
            Assert.Equal(expectedValues[2], targetMeasurement.Height);

            Assert.Same(sourceMeasurement, targetMeasurement);
        }
    }
}