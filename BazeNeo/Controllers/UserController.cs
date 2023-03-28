using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4jClient;
using Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace SkinCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class UserController : ControllerBase
    {
        private readonly IGraphClient _client;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;


        public UserController(IGraphClient client, IUserServices userServices, IConfiguration configuration)
        {
            _client = client;
            _userServices= userServices;
            _configuration= configuration;
        }


        [HttpPost] 
        [Route("Register_User/{username}/{password}")]  
        public async Task<IActionResult> RegisterUser(string username,string password) 
        {
            if(!string.IsNullOrWhiteSpace(username) && username.Length<=20 && !string.IsNullOrWhiteSpace(password) && password.Length<=20)
            {
                var user=await _client.Cypher.Match("(u:User)").Where((User u)=>u.Username==username).Return(u=>u.As<User>()).ResultsAsync;
                if (user.Any())
                {
                    return StatusCode(202,"User with that username already exists!"); 
                }
                Dictionary<string, object> queryDict = new Dictionary<string, object>();
                queryDict.Add("username", username);
                queryDict.Add("password",password);
                await _client.Cypher.Create("(b:User {Username:$username,Password:$password})")
                                    .WithParams(queryDict)
                                    .ExecuteWithoutResultsAsync();
                return Ok(); 
            }

            return BadRequest(); 
        }


        [HttpGet] 
        [Route("LogIn_User/{username}/{password}")]      
        public async Task<IActionResult> LogInUser(string username, string password) 
        {

            if(!string.IsNullOrWhiteSpace(username) && username.Length<=20 && !string.IsNullOrWhiteSpace(password) && password.Length<=20)
            {
                User us= new User();
                var user=await _client.Cypher.Match("(u:User)").Where((User u)=>u.Username==username && u.Password==password).Return(u=>u.As<User>()).Limit(1).ResultsAsync;
                if (!user.Any())
                {
                    return StatusCode(202,"Wrong password or username!"); 
                }
                else
                {
                    us= user.FirstOrDefault();
                    string token = CreateToken(us);

                    return Ok(
                            new
                            {   
                                Token = token   
                            }        
                            );
                }
            }

            return BadRequest(); 
        }


        [HttpGet]
        [Route("MostLiked_Product")]

         public async Task<IActionResult> MostLikedProduct() 
        {
            var user = await _client.Cypher
                .Match("(p:Product)<-[r:LIKES]-()")
                .Return((p, r) => new {
                    Product = p.As<Product>(),
                    Likes = r.Count()

                })
                .OrderByDescending("Likes")
                .Limit(5)
                .ResultsAsync;

            return Ok(user);
                              
        }

        [HttpGet]
        [Route("LikeList_User/{username}")]
        public async Task<IActionResult> LikeListUser(string username) 
        {
            var products = await _client.Cypher
                .Match("(u:User)-[r:LIKES]->(p:Product)")
                .Where((User u)=>u.Username==username)
                .Return((p) => new {
                    Product = p.As<Product>(),

                })
                .ResultsAsync;

            return Ok(products);
            
                              
        }

        [HttpGet]
        [Route("SearchEngine_Products/{searchText}")]
        public async Task<IActionResult> SearchEnigneProducts(string searchText) 
        {
           
             List<Product> prod = new List<Product>();
           
                var products = await _client.Cypher
                    .Match("(p:Product),(b:Brand)")
                    .Where("p.ProductName CONTAINS $searchText")
                    .OrWhere("(p)-[:SOLD_BY]-> (b) AND b.Name CONTAINS $searchText")
                    .WithParam("searchText", searchText)
                    .ReturnDistinct(p => p.As<Product>())
                    .ResultsAsync;
                var pl = products.ToList();
            if(!pl.Any())
                return StatusCode(202,"Not found");
            foreach(Product p in products){
          
                     prod.Add(p);

                
            }
          
            return Ok(prod);                     
        }




        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(60).ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

         private string? ValidateToken(string token)
        {
                if (token == null) 
                return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value);
                try
                {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var userUsername= jwtToken.Claims.First(x=> x.Type== ClaimTypes.Name).Value;
                string idd= userId.ToString()+ " "+ userUsername;
                return idd;
                }
                catch
                {
                return null;
                }
        }
   
         private bool  checkUser(int idp, string username){
             Request.Headers.TryGetValue("Authorization", out var token);
            string jwt = token.ToString().Split(" ")[1];
            var idandusername= ValidateToken(jwt);
            var userId= int.Parse(idandusername.Split(" ")[0]);
            var  userName= idandusername.Split(" ")[1];
            if(userName!= username){
                 return false; //nije dobra username
            }
            if(userId==null || idp!= userId ){
                return false; //nije dobar id 
            }
            return true;
        }  
    }

}
