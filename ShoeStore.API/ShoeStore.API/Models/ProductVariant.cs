namespace ShoeStore.API.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Size { get; set; }
        public int Stock { get; set; }
    }
}
