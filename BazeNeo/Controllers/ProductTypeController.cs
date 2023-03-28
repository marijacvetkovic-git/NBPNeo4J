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
   
    public class ProductTypeController : ControllerBase
    {
        private readonly IGraphClient _client;

        public ProductTypeController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost] 
        [Route("Create_ProductType")]      
        public async Task<IActionResult> CreateProductType([FromBody] ProductType type)
        {
            string name=type.Name;
            var pr= await _client.Cypher.Match("(p:ProductType {Name:$name})").WithParam("name",name).Return(p => p.As<ProductType>()).ResultsAsync;
            var lista=pr.ToList();
            if(lista.Any())
            {
                return StatusCode(202,"ProductType with that name already exists!");
            }
            if(!string.IsNullOrWhiteSpace(type.Name)){
                await _client.Cypher.Create("(b:ProductType $type)")
                                    .WithParam("type", type)
                                    .ExecuteWithoutResultsAsync();

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet] 
        [Route("Get_ProductType")]      
        public async Task<IActionResult> GetProductTypes()
        {
            var pt = await _client.Cypher.Match("(p:ProductType)")
                                            .Return(p => p.As<ProductType>())
                                            .ResultsAsync;
            
            if(!pt.Any()){
                return BadRequest();
            }
            return Ok(pt);
            
        }
    }
}