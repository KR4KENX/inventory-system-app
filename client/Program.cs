// See https://aka.ms/new-console-template for more information
using linux_app01;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

var hostname = System.Net.Dns.GetHostName();

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("User-Agent", "Adi");

await ProcessHttpRequest(client);

static async Task ProcessHttpRequest(HttpClient client)
{
    //var response = await client.GetStringAsync("http://api.w2016-server.com");

    //Console.WriteLine(response);

    //ADDING
    Product product = new();
    Console.WriteLine("add product");
    product.title = Console.ReadLine();
    product.description = Console.ReadLine();
    product.price = Int32.Parse(Console.ReadLine());

    string requestBody = JsonSerializer.Serialize(product);
    var contentData = new StringContent(requestBody);

    var responseToAdd = await client.PostAsync("http://localhost:8106/product", contentData);

    var message = await responseToAdd.Content.ReadAsStringAsync();
    Console.WriteLine(message);

    //READING
    Console.WriteLine("type in product Id");

    string productId = Console.ReadLine();
    if (productId is null)
        return;

    var response = await client.GetStringAsync($"http://localhost:8106/{productId}");

    Product parsedResponse = JsonSerializer.Deserialize<Product>(response);


    Console.WriteLine("Product title: " + parsedResponse?.title);
    Console.WriteLine("desc: " + parsedResponse?.description);
    Console.WriteLine("price: " + parsedResponse?.price);

    //BREAK
    Console.ReadKey();
}
