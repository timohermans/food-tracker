using Core.Data.Types;

namespace Scraper.ProductExtraction;

public abstract class ProductExtractionResult
{
    public Product? Result { get; set; }
}

public class ProductFailResult : ProductExtractionResult
{
    public string ErrorMessage { get; private set; }

    public ProductFailResult(string errorMessage)
    {
        Result = null;
        ErrorMessage = errorMessage;
    }
}

public class ProductSuccess : ProductExtractionResult
{
    public new Product Result { get; set; }

    public ProductSuccess(Product product)
    {
        Result = product;
    }
}