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
   
    public class SkinTypeController : ControllerBase
    {
        private readonly IGraphClient _client;

        public SkinTypeController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost] 
        [Route("Create_SkinType")]      
        public async Task<IActionResult> CreateSkinType([FromBody] SkinType type)
        {
             string name=type.SType;
            var pr= await _client.Cypher.Match("(p:SkinType {SType:$name})").WithParam("name",name).Return(p => p.As<SkinType>()).ResultsAsync;
            var lista=pr.ToList();
            if(lista.Any())
            {
                return StatusCode(202,"SkinType with that name already exists!");
            }
            if(!string.IsNullOrWhiteSpace(type.SType)){
                await _client.Cypher.Create("(b:SkinType $type)")
                                    .WithParam("type", type)
                                    .ExecuteWithoutResultsAsync();

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet] 
        [Route("Get_SkinType")]      
        public async Task<IActionResult> GetSkinType()
        {
            var st = await _client.Cypher.Match("(s:SkinType)")
                                            .Return(s=> s.As<SkinType>())
                                            .ResultsAsync;
            
            if(!st.Any()){
                return BadRequest();
            }
            return Ok(st);
            
        }

    }
}