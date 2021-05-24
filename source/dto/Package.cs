
using System.Collections.Generic;

namespace ShippingQuote
{
    public class Package
    {
        public float Height { get; set;}
        public float Width {get; set;}
        public float Length {get; set;}

        public Package(float height, float width, float length)
        {
            this.Height = height;
            this.Width = width;
            this.Length = length;
        }
    }
}