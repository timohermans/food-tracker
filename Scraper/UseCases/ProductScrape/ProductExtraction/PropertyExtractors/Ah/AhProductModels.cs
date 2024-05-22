namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;


public class AhObject
{
    public Common? common { get; set; }
    public Alternatives1? alternatives { get; set; }
    public Analytics? analytics { get; set; }
    public Autocompleteproducts? autocompleteProducts { get; set; }
    public Content? content { get; set; }
    public Crosssells? crossSells { get; set; }
    public Fakedoor? fakedoor { get; set; }
    public AhProduct? product { get; set; }
    public Productrecommendations? productRecommendations { get; set; }
    public Recipe? recipe { get; set; }
    public Search? search { get; set; }
    public Supershops? superShops { get; set; }
    public Taxonomy1? taxonomy { get; set; }
    public Uistate? uiState { get; set; }
    public Router1? router { get; set; }
    public string? locale { get; set; }
}

public class Common
{
    public Ui? ui { get; set; }
    public Alternatives? alternatives { get; set; }
    public Basket? basket { get; set; }
    public Member? member { get; set; }
    public Notifications? notifications { get; set; }
    public Globalsearch? globalSearch { get; set; }
    public Customheaders? customHeaders { get; set; }
    public Server? server { get; set; }
    public Productcard? productCard { get; set; }
    public Router? router { get; set; }
}

public class Ui
{
    public Alternativespanel? alternativesPanel { get; set; }
    public Navigation? navigation { get; set; }
    public Viewport? viewport { get; set; }
}

public class Alternativespanel
{
    public bool isOpen { get; set; }
    public bool isLoading { get; set; }
}

public class Navigation
{
    public bool personalMenuOpen { get; set; }
    public bool mainMenuOpen { get; set; }
    public bool searchIsOpen { get; set; }
}

public class Viewport
{
    public bool mobileWebView { get; set; }
    public bool phone { get; set; }
    public bool tablet { get; set; }
    public bool desktop { get; set; }
}

public class Alternatives
{
    public string? state { get; set; }
    public object[]? data { get; set; }
    public int productId { get; set; }
    public Datalakemodel? dataLakeModel { get; set; }
}

public class Datalakemodel
{
    public string? name { get; set; }
    public string? version { get; set; }
}

public class Basket
{
    public object[]? products { get; set; }
    public string? state { get; set; }
    public Summary? summary { get; set; }
    public object[]? list { get; set; }
}

public class Summary
{
    public int quantity { get; set; }
}

public class Member
{
    public string? state { get; set; }
    public object? data { get; set; }
    public string? memberStatus { get; set; }
}

public class Notifications
{
    public string? state { get; set; }
    public object[]? data { get; set; }
}

public class Globalsearch
{
    public object[]? suggestions { get; set; }
    public string? state { get; set; }
}

public class Customheaders
{
}

public class Server
{
    public object? shouldNavigateTo { get; set; }
}

public class Productcard
{
    public Favorites? favorites { get; set; }
}

public class Favorites
{
    public string? state { get; set; }
    public object[]? lists { get; set; }
    public Activelist? activeList { get; set; }
}

public class Activelist
{
    public string? state { get; set; }
}

public class Router
{
    public Location? location { get; set; }
}

public class Location
{
    public string? pathname { get; set; }
}

public class Alternatives1
{
    public string? state { get; set; }
    public object[]? alternatives { get; set; }
    public int productId { get; set; }
    public Datalakemodel1? dataLakeModel { get; set; }
}

public class Datalakemodel1
{
    public string? name { get; set; }
    public string? version { get; set; }
}

public class Analytics
{
    public object[]? filters { get; set; }
    public string? latestUISearchInteraction { get; set; }
}

public class Autocompleteproducts
{
    public object[]? suggestions { get; set; }
    public string? state { get; set; }
    public int timestamp { get; set; }
}

public class Content
{
    public string? state { get; set; }
    public Pages? pages { get; set; }
    public Partials? partials { get; set; }
    public Json? json { get; set; }
    public bool preview { get; set; }
    public Documents? documents { get; set; }
}

public class Pages
{
    public object? current { get; set; }
    public string? state { get; set; }
    public Data? data { get; set; }
    public Components? components { get; set; }
    public object? error { get; set; }
}

public class Data
{
}

public class Components
{
}

public class Partials
{
    public string? state { get; set; }
    public Data1? data { get; set; }
}

public class Data1
{
}

public class Json
{
    public object? current { get; set; }
    public string? state { get; set; }
    public Data2? data { get; set; }
}

public class Data2
{
}

public class Documents
{
}

public class Crosssells
{
    public int count { get; set; }
    public object? productId { get; set; }
    public bool fetchProducts { get; set; }
}

public class Fakedoor
{
    public string? state { get; set; }
    public object[]? invalidProductIds { get; set; }
    public Popup? popup { get; set; }
}

public class Popup
{
    public bool open { get; set; }
    public object? card { get; set; }
    public object? freeProduct { get; set; }
}

public class AhProduct
{
    public string? state { get; set; }
    public object[]? invalidProductIds { get; set; }
    public Card? card { get; set; }
}

public class Card
{
    public string? type { get; set; }
    public int id { get; set; }
    public Product1[]? products { get; set; }
    public Meta? meta { get; set; }
    public Angledimage[]? angledImages { get; set; }
}

public class Meta
{
    public string? gln { get; set; }
    public string? gtin { get; set; }
    public Description? description { get; set; }
    public Nutrition[]? nutritions { get; set; }
    public Contents? contents { get; set; }
    public Ingredients? ingredients { get; set; }
    public Usage? usage { get; set; }
    public Storage? storage { get; set; }
    public Contact? contact { get; set; }
    public Resources? resources { get; set; }
    public Marketing? marketing { get; set; }
}

public class Description
{
    public string[]? descriptions { get; set; }
}

public class Contents
{
    public string[]? netContents { get; set; }
    public object? statement { get; set; }
    public object? drainedWeight { get; set; }
    public string? servingSize { get; set; }
    public string? servingsPerPackage { get; set; }
    public bool eMark { get; set; }
}

public class Ingredients
{
    public Allergens? allergens { get; set; }
    public string? statement { get; set; }
    public object? nonfoodIngredientStatement { get; set; }
}

public class Allergens
{
    public string[]? list { get; set; }
    public string[]? contains { get; set; }
    public string[]? mayContain { get; set; }
    public string[]? freeFrom { get; set; }
}

public class Usage
{
    public object[]? instructions { get; set; }
    public Preparationinstruction[]? preparationInstructions { get; set; }
    public string? servingSuggestion { get; set; }
    public object[]? dosageInstructions { get; set; }
    public object[]? warnings { get; set; }
    public object[]? signalWords { get; set; }
    public object[]? hazardStatements { get; set; }
    public object[]? precautions { get; set; }
    public bool bacteriaWarning { get; set; }
}

public class Preparationinstruction
{
    public string[]? contentLines { get; set; }
}

public class Storage
{
    public string[]? instructions { get; set; }
    public object[]? lifeSpan { get; set; }
}

public class Contact
{
    public string[]? name { get; set; }
    public string[]? address { get; set; }
    public Communicationchannel[]? communicationChannels { get; set; }
}

public class Communicationchannel
{
    public string? type { get; set; }
    public string? value { get; set; }
}

public class Resources
{
    public object[]? attachments { get; set; }
    public Icon[]? icons { get; set; }
}

public class Icon
{
    public string? type { get; set; }
    public string? id { get; set; }
    public string? title { get; set; }
}

public class Marketing
{
    public string? description { get; set; }
    public string[]? features { get; set; }
}

public class Nutrition
{
    public Nutrient[]? nutrients { get; set; }
    public object[]? additionalInfo { get; set; }
    public string? dailyValueIntakeReference { get; set; }
    public string? servingSize { get; set; }
    public string? servingSizeDescription { get; set; }
    public string? preparationState { get; set; }
    public string? basisQuantity { get; set; }
    public string? basisQuantityDescription { get; set; }
}

public class Nutrient
{
    public string? name { get; set; }
    public string? type { get; set; }
    public string? value { get; set; }
    public string? dailyValue { get; set; }
}

public class Product1
{
    public int id { get; set; }
    public Control? control { get; set; }
    public string? title { get; set; }
    public string? link { get; set; }
    public bool availableOnline { get; set; }
    public bool orderable { get; set; }
    public Highlight? highlight { get; set; }
    public Propertyicon[]? propertyIcons { get; set; }
    public Image[]? images { get; set; }
    public Price? price { get; set; }
    public int itemCatalogId { get; set; }
    public string? brand { get; set; }
    public string? category { get; set; }
    public string? theme { get; set; }
    public int hqId { get; set; }
    public long[]? gtins { get; set; }
    public string? summary { get; set; }
    public string? descriptionFull { get; set; }
    public int taxonomyId { get; set; }
    public Taxonomy[]? taxonomies { get; set; }
    public int contributionMargin { get; set; }
    public Properties? properties { get; set; }
}

public class Control
{
    public string? theme { get; set; }
    public string? type { get; set; }
}

public class Highlight
{
    public string? name { get; set; }
}

public class Price
{
    public float now { get; set; }
    public string? unitSize { get; set; }
}

public class Properties
{
    public string? nutriscore { get; set; }
    public string[]? lifestyle { get; set; }
}

public class Propertyicon
{
    public string? name { get; set; }
    public string? title { get; set; }
}

public class Image
{
    public int height { get; set; }
    public int width { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? ratio { get; set; }
}

public class Taxonomy
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? imageSiteTarget { get; set; }
    public object[]? images { get; set; }
    public bool shown { get; set; }
    public int level { get; set; }
    public int sortSequence { get; set; }
    public int[]? parentIds { get; set; }
}

public class Angledimage
{
    public string? __typename { get; set; }
    public string? angle { get; set; }
    public Small? small { get; set; }
    public Medium? medium { get; set; }
    public Large? large { get; set; }
}

public class Small
{
    public string? __typename { get; set; }
    public int height { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
}

public class Medium
{
    public string? __typename { get; set; }
    public int height { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
}

public class Large
{
    public string? __typename { get; set; }
    public int height { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
}

public class Productrecommendations
{
    public Datalake? dataLake { get; set; }
    public Productservice? productService { get; set; }
    public Similarproducts? similarProducts { get; set; }
}

public class Datalake
{
    public string? state { get; set; }
    public object[]? cards { get; set; }
    public Datalakemodel2? dataLakeModel { get; set; }
    public object? title { get; set; }
}

public class Datalakemodel2
{
    public string? name { get; set; }
    public string? version { get; set; }
}

public class Productservice
{
    public string? state { get; set; }
    public object[]? cards { get; set; }
}

public class Similarproducts
{
    public string? state { get; set; }
    public object[]? cards { get; set; }
}

public class Recipe
{
    public string? state { get; set; }
    public object[]? recipes { get; set; }
}

public class Search
{
    public string? state { get; set; }
    public string? currentQuery { get; set; }
    public int timestamp { get; set; }
    public object[]? results { get; set; }
    public Filters? filters { get; set; }
    public Page? page { get; set; }
    public object[]? taxonomies { get; set; }
    public object[]? querySuggestions { get; set; }
}

public class Filters
{
    public object[]? properties { get; set; }
    public object[]? brands { get; set; }
    public object[]? taxonomies { get; set; }
    public object[]? prices { get; set; }
}

public class Page
{
    public int size { get; set; }
    public int totalElements { get; set; }
    public int totalPages { get; set; }
    public int number { get; set; }
}

public class Supershops
{
    public object[]? visibleShops { get; set; }
}

public class Taxonomy1
{
    public string? state { get; set; }
    public object[]? topLevel { get; set; }
    public object[]? brandTaxonomies { get; set; }
    public object[]? taxonomies { get; set; }
    public object[]? invalidTaxonomies { get; set; }
}

public class Uistate
{
    public Filters1? filters { get; set; }
    public Alternatives2? alternatives { get; set; }
}

public class Filters1
{
    public bool isOpen { get; set; }
    public object? selectedFilter { get; set; }
}

public class Alternatives2
{
    public bool isOpen { get; set; }
    public int type { get; set; }
    public int productId { get; set; }
}

public class Router1
{
    public Location1? location { get; set; }
    public string? action { get; set; }
}

public class Location1
{
    public string? pathname { get; set; }
    public string? search { get; set; }
    public string? hash { get; set; }
    public object? state { get; set; }
    public string? key { get; set; }
    public Query? query { get; set; }
}

public class Query
{
}

