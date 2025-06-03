namespace WhatsAppAPISolutionDL.Dto.Catalog
{
    public class CatalogDto
    {
        public Menu menu { get; set; }

        public class Menu
        {
            public Menu()
            {
                categories = new List<Category>();
                items = new List<Item>();
                modifiers = new List<Modifier>();
            }
            public List<Category> categories { get; set; }
            public List<Item> items { get; set; }
            public List<Modifier> modifiers { get; set; }
        }

        public class Category
        {
            public Category()
            {
                item_ids = new List<string>();
            }

            public string id { get; set; }
            public Name name { get; set; }
            public List<string> item_ids { get; set; }
        }

        public class Item
        {
            public Item()
            {
                modifier_ids = new List<string>();
            }

            public string id { get; set; }
            public Name name { get; set; }
            public Description description { get; set; }
            public PriceInfo price_info { get; set; }
            public Image image { get; set; }
            public List<string> modifier_ids { get; set; }
            public string type { get; set; }
            public string ItemURL { get; set; }
            public int displayOrder { get; set; }
        }

        public class Modifier
        {
            public Modifier()
            {
                modifier_items = new List<ModifierItem>();
            }

            public string id { get; set; }
            public Name name { get; set; }
            public Description description { get; set; }
            public int min_selection { get; set; }
            public int max_selection { get; set; }
            public List<ModifierItem> modifier_items { get; set; }

            public class ModifierItem
            {
                public string item_id { get; set; }
                public bool is_default { get; set; }
            }
        }

        public class Name
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class Description
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class PriceInfo
        {
            public decimal price { get; set; }
        }
        public class Image
        {
            public string url { get; set; }
        }
    }
}
