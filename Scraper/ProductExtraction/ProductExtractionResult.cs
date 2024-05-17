using Core.Data.Types;

namespace Scraper.ProductExtration;

public abstract class ProductExtractionResult {
    public Product? Result { get; set; }
}

public class ProductFailResult : ProductExtractionResult {
    public ProductFailResult()
    {
        Result = null;
    }
}

public class ProductSuccess : ProductExtractionResult {
    public ProductSuccess(Product product) {
        Result = product;
    }
}