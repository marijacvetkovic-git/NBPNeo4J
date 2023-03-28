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
   
    public class IngredientController : ControllerBase
    {
        private readonly IGraphClient _client;

        public IngredientController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost] 
        [Route("Create_Ingredient")]      
        public async Task<IActionResult> CreateIngredient([FromBody] Ingredient ing) 
        {
            string name=ing.Name;
            var pr= await _client.Cypher.Match("(p:Ingredient {Name:$name})").WithParam("name",name).Return(p => p.As<Ingredient>()).ResultsAsync;
            var lista=pr.ToList();
            if(lista.Any())
            {
                return StatusCode(202,"Ingredient with that name already exists!");
            }
            if(!string.IsNullOrWhiteSpace(ing.Name) && !string.IsNullOrWhiteSpace(ing.Usage) && ing.Irritancy != null && ing.Irritancy >= 0 && ing.Irritancy <= 5){
                await _client.Cypher.Create("(b:Ingredient $ing)")
                                    .WithParam("ing", ing)
                                    .ExecuteWithoutResultsAsync();
                return Ok(); 
            }

            return BadRequest(); 
        }

        [HttpPut] 
        [Route("Update_Ingredient/{name}/{usage}/{irritancy}")]      
        public async Task<IActionResult> UpdateIngredient(string name, string usage, int irritancy)
        {
            var ing = await _client.Cypher.Match("(i:Ingredient)")
                                          .Where((Ingredient i) => i.Name == name)
                                          .Return(i => i.As<Ingredient>())
                                          .ResultsAsync;
     
            if (!ing.Any()){
                return StatusCode(202,"Ingredient not found"); 
            }
     
            if(!string.IsNullOrWhiteSpace(name)){
                await _client.Cypher.Match("(i:Ingredient)")
                                    .Where((Ingredient i) => i.Name == name)
                                    .Set("i.Name = $name")
                                    .WithParam("name", name)
                                    .ExecuteWithoutResultsAsync();
            }
            if(!string.IsNullOrWhiteSpace(usage)){
                await _client.Cypher.Match("(i:Ingredient)")
                                    .Where((Ingredient i) => i.Name == name)
                                    .Set("i.Usage = $usage")
                                    .WithParam("usage", usage)
                                    .ExecuteWithoutResultsAsync();
            }
            if(irritancy != null){
                await _client.Cypher.Match("(i:Ingredient)")
                                    .Where((Ingredient i) => i.Name == name)
                                    .Set("i.Irritancy = $irritancy")
                                    .WithParam("irritancy", irritancy)
                                    .ExecuteWithoutResultsAsync();
            }
            return Ok();
        }

        [HttpGet] 
        [Route("Get_Ingredients")]      
        public async Task<IActionResult> GetIngredients()
        {
            var ings = await _client.Cypher.Match("(i:Ingredient)")
                                            .Return(i => i.As<Ingredient>())
                                            .ResultsAsync;
            
            if(!ings.Any()){
                return BadRequest();
            }
            return Ok(ings);
            
        }

        [HttpDelete]
        [Route("Delete_Ingredient/{ingName}")]
        public async Task<IActionResult> DeleteIngredient(string ingName){ 

            var product = await _client.Cypher.Match("(p:Ingredient)")
                            .Where((Ingredient p) => p.Name == ingName)
                            .Return(p => p.As<Ingredient>())
                            .ResultsAsync;
            if (!product.Any())
            {
                return StatusCode(202,"Ingredient not found"); 
            }

            await _client.Cypher.Match("(p:Ingredient)")
                                .Where((Ingredient p)=>p.Name == ingName)
                                .DetachDelete("p")
                                .ExecuteWithoutResultsAsync();
            return Ok();
        }

    }
}