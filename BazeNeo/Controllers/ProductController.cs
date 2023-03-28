using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4jClient;
using Models;
using System.Linq;
using System.Collections.Generic;
using DTO;

namespace SkinCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ProductController : ControllerBase
    {
        private readonly IGraphClient _client;

        public ProductController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost] 
        [Route("Create_Product")]      
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            string name=product.ProductName;
            var pr= await _client.Cypher.Match("(p:Product {ProductName:$name})").WithParam("name",name).Return(p => p.As<Product>()).ResultsAsync;
            var lista=pr.ToList();
            if(lista.Any())
            {
                return StatusCode(202,"Product with that name already exists");
            }
            if(!string.IsNullOrWhiteSpace(product.ProductName) && !string.IsNullOrWhiteSpace(product.Use) && !string.IsNullOrWhiteSpace(product.Summary)){
                await _client.Cypher.Create("(p:Product $product)")
                                    .WithParam("product", product)
                                    .ExecuteWithoutResultsAsync();

                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("Get_Products")]
        public async Task<IActionResult> GetProducts(){
            var products = await _client.Cypher.Match("(p: Product)")
                                                .Return(p => p.As<Product>())
                                                .ResultsAsync;
            if(!products.Any()){
                return BadRequest();
            
            }

            return Ok(products);
       }

        [HttpGet]
        [Route("Get_ProductForUpdate/{prodName}")]
        public async Task<IActionResult> GetProductForUpdate(string prodName){
            var product = await _client.Cypher.Match("(p: Product)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(p => p.As<Product>())
                                                .ResultsAsync;
            if(!product.Any()){
                return BadRequest();
            }
            return Ok(product);
        }


        [HttpGet]
        [Route("Get_Product/{prodName}")]
        public async Task<IActionResult> GetProduct(string prodName){
            var product = await _client.Cypher.Match("(p: Product)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(p => p.As<Product>())
                                                .ResultsAsync;
            if(!product.Any()){
                return BadRequest();
            }
            var brand = await _client.Cypher.Match("(p:Product)-[r:SOLD_BY]->(b:Brand)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(b => b.As<Brand>())
                                                .ResultsAsync;
            var prodtype = await _client.Cypher.Match("(p:Product)-[r:USED_AS]->(b:ProductType)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(b => b.As<ProductType>())
                                                .ResultsAsync;
            return Ok(
            new{
                ProduktName= product.Single().ProductName,
                ProductUse = product.Single().Use,
                ProductSummary = product.Single().Summary,
                Brand=brand.Single().Name,
                ProdType=prodtype.Single().Name
            });
       }

       [HttpGet]
       [Route("Get_ProductIngredients/{prodName}")]
       public async Task<IActionResult> GetProductIngredients(string prodName){
            var product = await _client.Cypher.Match("(p: Product)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(p => p.As<Product>())
                                                .ResultsAsync;
            if(!product.Any()){
                return BadRequest();
            }
            
            var ing = await _client.Cypher.Match("(p:Product)-[r:CONTAINS]->(b:Ingredient)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(b => b.As<Ingredient>())
                                                .ResultsAsync;


            return Ok(ing);
       }

    [HttpGet]
       [Route("Get_ProductSkinTypes/{prodName}")]
       public async Task<IActionResult> GetProductSkinTypes(string prodName){
            var product = await _client.Cypher.Match("(p: Product)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(p => p.As<Product>())
                                                .ResultsAsync;
            if(!product.Any()){
                return BadRequest();
            }
            
           var skintype = await _client.Cypher.Match("(p:Product)-[r:INTENDED_FOR]->(b:SkinType)")
                                                .Where((Product p)=>p.ProductName == prodName)
                                                .Return(b => b.As<SkinType>())
                                                .ResultsAsync;


            return Ok(skintype);
       }

        [HttpDelete]
        [Route("Delete_Product/{productName}")]
        public async Task<IActionResult> DeleteProduct(string productName){ 
            var product = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == productName)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
            if (!product.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            await _client.Cypher.Match("(p:Product)")
                                .Where((Product p)=>p.ProductName == productName)
                                .DetachDelete("p")
                                .ExecuteWithoutResultsAsync();
            return Ok();
        }

        [HttpPut] 
        [Route("Update_Product/{productname}/{use}/{summary}")]      
        public async Task<IActionResult> UpdateProduct(string productname, string use, string summary)
        {
            var product = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName== productname)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
            if (!product.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            if(!string.IsNullOrWhiteSpace(productname)) 
            {
             
                await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName== productname)
                            .Set("p.ProductName = $productname")
                            .WithParam("productname", productname)
                            .ExecuteWithoutResultsAsync();
            }
            if(!string.IsNullOrWhiteSpace(use)){
               
                await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName== productname)
                            .Set("p.Use = $use")
                            .WithParam("use", use)
                            .ExecuteWithoutResultsAsync();
            }
            if(!string.IsNullOrWhiteSpace(summary)){
              
                await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName== productname)
                            .Set("p.Summary = $summary")
                            .WithParam("summary", summary)
                            .ExecuteWithoutResultsAsync();
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetNotLiked/{username}")]
        public async Task<IActionResult> GetNotLiked(string username)
        {
            var products = await _client.Cypher
                .Match("(p: Product)")
                .Where("NOT (p)<-[:LIKES]-(:User {Username: $username})")
                .WithParam("username", username)
                .Return(p => p.As<Product>())
                .ResultsAsync;

            return Ok(products);
        }
    }
}
    
