namespace WhatsAppAPISolutionDL.Dto.Order
{
    public class MetaOrderRequestDto
    {
        public string client_Id { get; set; }
        public string wam_Id { get; set; }
        public string update_dateTime { get; set; }
        public string from { get; set; }
        public string type { get; set; }
        public PhoneNumber phone_number_Id { get; set; }
        public Contact contact { get; set; }
        public Context context { get; set; }
        public Order order { get; set; }

        public class PhoneNumber
        {
            public string display_phone_number { get; set; }
            public string phone_number_id { get; set; }
        }
        public class Contact
        {
            public string wa_id { get; set; }
            public string name { get; set; }
        }

        public class Context
        {
            public string from { get; set; }
            public string wam_Id { get; set; }
        }

        public class Order
        {
            public Order()
            {
                product_items = new List<Item>();
            }

            public string catalog_id { get; set; }
            public string text { get; set; }
            public List<Item> product_items { get; set; }

            public class Item
            {
                public string product_retailer_id { get; set; }
                public string quantity { get; set; }
                public decimal item_price { get; set; }
                public string currency { get; set; }
            }
        }
    }
}
