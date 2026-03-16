namespace Catalog.API.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<string> Category { get; set; } = [];
    public string Description { get; set; } = default!;
    public List<string> ImageFiles { get; set; } = [];
    public string ImageFile
    {
        get => ImageFiles.Count > 0 ? ImageFiles[0] : string.Empty;
        set
        {
            if (ImageFiles.Count == 0)
                ImageFiles.Add(value);
            else
                ImageFiles[0] = value;
        }
    }
    public decimal Price { get; set; }
    public string SellerId { get; set; } = default!;
}
