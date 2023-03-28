using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4jClient;
using Models;
using System.Linq;

namespace SkinCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class BrandController : ControllerBase
    {
        private readonly IGraphClient _client;

        public BrandController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost] 
        [Route("Create_Brand")]      
        public async Task<IActionResult> CreateBrand([FromBody] Brand brand)
        {
            string name=brand.Name;
            var pr= await _client.Cypher.Match("(p:Brand {Name:$name})").WithParam("name",name).Return(p => p.As<Brand>()).ResultsAsync;
            var lista=pr.ToList();
            if(lista.Any())
            {
                return StatusCode(202,"Brand with that name already exists!");
            }
            if(!string.IsNullOrWhiteSpace(brand.Name) && !string.IsNullOrWhiteSpace(brand.Founder) && !string.IsNullOrWhiteSpace(brand.Summary)){
                await _client.Cypher.Create("(b:Brand $brand)")
                                .WithParam("brand", brand)
                                .ExecuteWithoutResultsAsync();

                return Ok();
            }
            return BadRequest();
            
        }

        [HttpGet] 
        [Route("Get_Brands")]      
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _client.Cypher.Match("(b:Brand)")
                                            .Return(b => b.As<Brand>())
                                            .ResultsAsync;
            
            if(!brands.Any()){
                return BadRequest();
            }
            return Ok(brands);

            
        }

        [HttpPut] 
        [Route("Update_Brand/{name}/{founder}/{summary}")]      
        public async Task<IActionResult> UpdateBrand(string name, string founder, string summary)
        {
            var ing = await _client.Cypher.Match("(i:Brand)")
                                          .Where((Brand i) => i.Name == name)
                                          .Return(i => i.As<Brand>())
                                          .ResultsAsync;
     
            if (!ing.Any()){
                return StatusCode(202,"Brand not found"); 
            }
     
            if(!string.IsNullOrWhiteSpace(name)){
                await _client.Cypher.Match("(i:Brand)")
                                    .Where((Brand i) => i.Name == name)
                                    .Set("i.Name = $name")
                                    .WithParam("name", name)
                                    .ExecuteWithoutResultsAsync();
            }
            if(!string.IsNullOrWhiteSpace(founder)){
                await _client.Cypher.Match("(i:Brand)")
                                    .Where((Brand i) => i.Name == name)
                                    .Set("i.Founder = $founder")
                                    .WithParam("founder", founder)
                                    .ExecuteWithoutResultsAsync();
            }
            if(!string.IsNullOrWhiteSpace(summary)){
                await _client.Cypher.Match("(i:Brand)")
                                    .Where((Brand i) => i.Name == name)
                                    .Set("i.Summary = $summary")
                                    .WithParam("summary", summary)
                                    .ExecuteWithoutResultsAsync();
            }
            return Ok();
        }
        [HttpDelete]
        [Route("Delete_Brand/{brName}")]
        public async Task<IActionResult> DeleteBrand(string brName){ 

            var product = await _client.Cypher.Match("(p:Brand)")
                            .Where((Brand p) => p.Name == brName)
                            .Return(p => p.As<Brand>())
                            .ResultsAsync;
            if (!product.Any())
            {
                return StatusCode(202,"Brand not found"); 
            }

            await _client.Cypher.Match("(p:Brand)")
                                .Where((Brand p)=>p.Name == brName)
                                .DetachDelete("p")
                                .ExecuteWithoutResultsAsync();
            return Ok();
        }

    }
}