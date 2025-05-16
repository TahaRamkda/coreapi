namespace WhatsAppAPISolutionDL.Dto.Order.KFG
{
    public class KFGOrder
    {
        public KFGOrder()
        {
            discounts = new List<Discount>();
            payments = new List<Payment>();
            items = new List<Item>();
        }

        public string id { get; set; }
        public string brandId { get; set; }
        public string status { get; set; }
        public string fulfillmentType { get; set; }
        public string orderNotes { get; set; }
        public string cutleryNotes { get; set; }
        public bool asap { get; set; }
        public string orderCreatedAt { get; set; }
        public string prepareFrom { get; set; }
        public string deliverAt { get; set; }
        public Customer customer { get; set; }
        public Delivery delivery { get; set; }
        public Amount subTotal { get; set; }
        public Amount deliveryFee { get; set; }
        public List<Discount> discounts { get; set; }
        public Amount grandTotal { get; set; }
        public List<Payment> payments { get; set; }
        public List<Item> items { get; set; }
         
        public class Amount
        {
            public decimal amount { get; set; }
            public string currencyCode { get; set; }
        }

        public class Customer
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string contactNumber { get; set; }
            public string email { get; set; }
        }

        public class Delivery
        {
            public string deliveryNotes { get; set; }
            public string city { get; set; }
            public string block { get; set; }
            public string street { get; set; }
            public string building { get; set; }
            public string floor { get; set; }
            public string flat { get; set; }
            public string avenue { get; set; }
            public Location location { get; set; }

            public class Location
            {
                public double latitude { get; set; }
                public double longitude { get; set; }
            }
        }

        public class Discount
        {
            public string name { get; set; }
            public decimal amount { get; set; }
            public string currencyCode { get; set; }
        }

        public class Payment
        {
            public string name { get; set; }
            public decimal amount { get; set; }
            public string currencyCode { get; set; }
            public string referenceNumber { get; set; }
        }

        public class Item
        {
            public Item()
            {
                modifiers = new List<Modifier>();
            }

            public string posItemId { get; set; }
            public int quantity { get; set; }
            public string name { get; set; }
            public Amount unitPrice { get; set; }
            public Amount totalPrice { get; set; }
            public Amount discountAmount { get; set; }
            public List<Modifier> modifiers { get; set; }

            public class Modifier
            {
                public Modifier()
                {
                    modifiers = new List<Modifier>();
                }

                public string posItemId { get; set; }
                public int quantity { get; set; }
                public string name { get; set; }
                public Amount unitPrice { get; set; }
                public Amount totalPrice { get; set; }
                public Amount discountAmount { get; set; }
                public List<Modifier> modifiers { get; set; }
            }
        }
    }
}
