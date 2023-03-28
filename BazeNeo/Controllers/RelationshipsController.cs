using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4jClient;
using Models;
using System.Linq;
using System.Collections.Generic;

namespace SkinCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class RelationshipsController : ControllerBase
    {
        private readonly IGraphClient _client;

        public RelationshipsController(IGraphClient client)
        {
            _client = client;
        }


        [HttpPost]
        [Route("Assign_IntendedFor/{product}/{skintype}")]
        public async Task<IActionResult> AssignIntededFor(string product,string skintype){

            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var stype = await _client.Cypher.Match("(p:SkinType)")
                            .Where((SkinType p) => p.SType == skintype)
                            .Return(p => p.As<SkinType>())
                            .ResultsAsync;
   
            if (!stype.Any())
            {
                return StatusCode(203,"Skintype not found"); 
            }

            await _client.Cypher.Match("(p:Product),(st:SkinType)")
                                .Where((Product p,SkinType st)=>p.ProductName==product && st.SType==skintype)
                                .Merge("(p)-[r:INTENDED_FOR]->(st)")
                                .ExecuteWithoutResultsAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Delete_IntendedFor/{product}/{skintype}")]
        public async Task<IActionResult> DeleteIntendedFor(string product, string skintype)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var ptype = await _client.Cypher.Match("(p:SkinType)")
                            .Where((SkinType p) => p.SType == skintype)
                            .Return(p => p.As<SkinType>())
                            .ResultsAsync;
   
            if (!ptype.Any())
            {
                return StatusCode(203,"Skintype not found"); 
            }
            await _client.Cypher.Match("(p:Product)-[rel:INTENDED_FOR]->(pt:SkinType)")
                                .Where((Product p, SkinType pt) => p.ProductName == product && pt.SType == skintype)
                                .Delete("rel")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }
        [HttpPost]
        [Route("Assign_UsedAs/{product}/{prodtype}")]
        public async Task<IActionResult> AssignUsedAs(string product, string prodtype)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var ptype = await _client.Cypher.Match("(p:ProductType)")
                            .Where((ProductType p) => p.Name == prodtype)
                            .Return(p => p.As<ProductType>())
                            .ResultsAsync;
   
            if (!ptype.Any())
            {
                return StatusCode(203,"Product type not found"); 
            }

            await _client.Cypher.Match("(p:Product), (pt:ProductType)")
                                .Where((Product p, ProductType pt) => p.ProductName == product && pt.Name == prodtype)
                                .Merge("(p)-[r:USED_AS]->(pt)")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }
    
        [HttpDelete]
        [Route("Delete_UsedAs/{product}/{prodtype}")]
        public async Task<IActionResult> DeleteUsedAs(string product, string prodtype)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var ptype = await _client.Cypher.Match("(p:ProductType)")
                            .Where((ProductType p) => p.Name == prodtype)
                            .Return(p => p.As<ProductType>())
                            .ResultsAsync;
   
            if (!ptype.Any())
            {
                return StatusCode(203,"Product type not found"); 
            }
            await _client.Cypher.Match("(p:Product)-[rel:IS]->(pt:ProductType)")
                                .Where((Product p, ProductType pt) => p.ProductName == product && pt.Name == prodtype)
                                .Delete("rel")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Assign_SoldBy/{product}/{brand}")]
        public async Task<IActionResult> AssignSoldBy(string product, string brand)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var br = await _client.Cypher.Match("(p:Brand)")
                            .Where((Brand p) => p.Name == brand)
                            .Return(p => p.As<Brand>())
                            .ResultsAsync;
   
            if (!br.Any())
            {
                return StatusCode(203,"Brand not found"); 
            }
            await _client.Cypher.Match("(p:Product),(b:Brand)")
                                .Where((Product p, Brand b)=> p.ProductName== product && b.Name== brand)
                                .Merge("(p)-[r:SOLD_BY]->(b)") 
                                .ExecuteWithoutResultsAsync();
                    
            return Ok();
        }

        [HttpDelete]
        [Route("Delete_SoldBy/{product}/{brand}")]
        public async Task<IActionResult> DeleteSoldBy(string product, string brand)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == product)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var br = await _client.Cypher.Match("(p:Brand)")
                            .Where((Brand p) => p.Name == brand)
                            .Return(p => p.As<Brand>())
                            .ResultsAsync;
   
            if (!br.Any())
            {
                return StatusCode(203,"Brand not found"); 
            }
            await _client.Cypher.Match("(p:Product)-[r:SOLD_BY]->(b:Brand)")
                                .Where((Product p, Brand b) => p.ProductName == product && b.Name== brand)
                                .Delete("r")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Assign_Contains/{iname}/{pname}/{percentage}")]
        public async Task<IActionResult> CreateContains(string iname,string pname,float percentage)
        {
            if(percentage < 0 && percentage > 100)
            {
                return StatusCode(203, "Precentage between 0 and 100");
            }

            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == pname)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var ing = await _client.Cypher.Match("(p:Ingredient)")
                            .Where((Ingredient p) => p.Name == iname)
                            .Return(p => p.As<Ingredient>())
                            .ResultsAsync;
   
            if (!ing.Any())
            {
                return StatusCode(203,"Ingredient not found"); 
            }

            await _client.Cypher.Match("(i:Ingredient),(p:Product)")
                                .Where((Ingredient i, Product p)=>i.Name==iname && p.ProductName==pname)
                                .Merge("(p)-[:CONTAINS{percentage:$percent}]->(i)")
                                .WithParam("percent",percentage)
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }
        [HttpDelete]
        [Route("Delete_Contains/{iname}/{pname}")]
        public async Task<IActionResult> DeleteContains(string iname,string pname){
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == pname)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Product not found"); 
            }

            var ing = await _client.Cypher.Match("(p:Ingredient)")
                            .Where((Ingredient p) => p.Name == iname)
                            .Return(p => p.As<Ingredient>())
                            .ResultsAsync;
   
            if (!ing.Any())
            {
                return StatusCode(203,"Ingredient not found"); 
            }

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("pname", pname);
            queryDict.Add("iname",iname);
            await _client.Cypher.Match("(p:Product {ProductName:$pname})-[rel:CONTAINS]->(i:Ingredient {Name:$iname})")
                                .WithParams(queryDict)
                                .Delete("rel").ExecuteWithoutResultsAsync();
            return Ok();
        }

        [HttpPost]
        [Route("Assign_Likes/{username}/{pname}")]
        public async Task<IActionResult> AssignLikes(string username,string pname)
        {
         

            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == pname)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Username not found"); 
            }

            var userr = await _client.Cypher.Match("(p:User)")
                            .Where((User p) => p.Username == username)
                            .Return(p => p.As<User>())
                            .ResultsAsync;
   
            if (!userr.Any())
            {
                return StatusCode(203,"Product not found"); 
            }
            var datum= System.DateTime.Now;
            await _client.Cypher.Match("(u:User),(p:Product)")
                                .Where((User u, Product p)=>u.Username==username && p.ProductName==pname)
                                .Merge("(u)-[:LIKES {Date:$datum}]->(p)")
                                .WithParam("datum", datum)
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("Delete_Assign_Liked/{username}/{pname}")]
        public async Task<IActionResult> DeleteAssignLiked(string username, string pname)
        {
            var prod = await _client.Cypher.Match("(p:Product)")
                            .Where((Product p) => p.ProductName == pname)
                            .Return(p => p.As<Product>())
                            .ResultsAsync;
   
            if (!prod.Any())
            {
                return StatusCode(202,"Username not found"); 
            }

            var userr = await _client.Cypher.Match("(p:User)")
                            .Where((User p) => p.Username == username)
                            .Return(p => p.As<User>())
                            .ResultsAsync;
                
            if (!userr.Any())
            {
                return StatusCode(203,"Product not found"); 
            }          

            await _client.Cypher.Match("(b:User)-[r:LIKES]->(p:Product)")
                                .Where((User b, Product p) => b.Username == username && p.ProductName == pname)
                                .Delete("r")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }
        
    }
}