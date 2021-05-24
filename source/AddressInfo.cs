namespace ShippingQuote
{
    public class AddressInfo
    {
        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public AddressInfo(string city, string province, string country)
        {            
            this.City = city;
            this.Province = province;
            this.Country = country;
        }

        public string GetFullAddress()
        {
            char[] chars = {',', ' '};
            return $"{this.City}, {this.Province}, {this.Country}".Trim(chars);
        }
    }
}