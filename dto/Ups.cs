
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ShippingQuote
{
    [Serializable]
    [XmlRoot("xml")]
    public class Ups
    {
        public string Source { get; set;}
        public string Destination {get; set;}
        public List<Package> Packages {get; set;}

        public Ups(string source, string destination, List<Package> packages)
        {
            this.Source = source;
            this.Destination = destination;
            this.Packages = packages;
        }
    }
}