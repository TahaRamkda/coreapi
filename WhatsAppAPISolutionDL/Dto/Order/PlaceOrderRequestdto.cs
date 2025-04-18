using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsAppAPISolutionDL.Dto.Order
{
    public class PlaceOrderRequestdto
    {
        [JsonPropertyName("catalogId")]
        public string CatalogId { get; set; }

        [JsonPropertyName("waid")]
        public string Waid { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonPropertyName("product_items")]
        public List<ProductItem> Product_items { get; set; }

        public PlaceOrderRequestdto()
        {
            Contacts = new List<Contact>();
            Product_items = new List<ProductItem>();
        }

        public class Contact
        {
            [JsonPropertyName("profile")]
            public Profile Profile { get; set; }

            [JsonPropertyName("wa_id")]
            public string Wa_id { get; set; }
        }

        public class Profile
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class ProductItem
        {
            [JsonPropertyName("product_retailer_id")]
            public string Product_retailer_id { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }

            [JsonPropertyName("item_price")]
            public decimal Item_price { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }
        }
    }
}