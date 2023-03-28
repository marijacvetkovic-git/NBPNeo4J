using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4jClient;
using Models;
using System.Linq;
using System;
using System.Collections.Generic;
using DTO;

namespace SkinCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class FilterController : ControllerBase
    {
        private readonly IGraphClient _client;

        public FilterController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPut]
        [Route("Filter")]
        public async Task<IActionResult> Filter([FromBody] FiltersDTO filter){
                    var query= _client.Cypher;
                            if(filter.Brands!= null && filter.Brands.Any()){
                                query= query.Match("(b:Brand) <-[:SOLD_BY]-(p:Product)");
                                if(filter.SkinTypes!= null & filter.SkinTypes.Any())
                                {
                                    query= query.Match("(p)-[:INTENDED_FOR]->(st:SkinType)");
                                }
                                if(filter.ProductTypes!= null && filter.ProductTypes.Any()){
                                    query=query.Match("(p)-[:USED_AS]->(pt:ProductType)");
                                }
                                if(filter.Indredients!= null && filter.Indredients.Any()){
                                    query= query.Match("(p)-[:CONTAINS]->(i:Ingredient)");
                                }
                                query= query.Where("1=1");
                            }
                            else if(filter.SkinTypes!= null & filter.SkinTypes.Any()){
                                query= query.Match("(p:Product)-[:INTENDED_FOR]->(st:SkinType)");
                                 if(filter.ProductTypes!= null && filter.ProductTypes.Any()){
                                    query=query.Match("(p)-[:USED_AS]->(pt:ProductType)");
                                }
                                if(filter.Indredients!= null && filter.Indredients.Any()){
                                    query= query.Match("(p)-[:CONTAINS]->(i:Ingredient)");
                                }
                                query= query.Where("1=1");
                            }
                            else if(filter.ProductTypes!= null && filter.ProductTypes.Any()){
                                query= query.Match("(p:Product)-[:USED_AS]->(pt:ProductType)");
                                 if(filter.Indredients!= null && filter.Indredients.Any()){
                                    query= query.Match("(p)-[:CONTAINS]->(i:Ingredient)");
                                }
                                query= query.Where("1=1");
                            }
                            else if(filter.Indredients!= null && filter.Indredients.Any()){
                                query= query.Match("(p:Product)-[:CONTAINS]->(i:Ingredient)")
                                .Where("1=1");
                            }
                            else{
                                return Ok("Nije izabran filter");
                            }

                                if(filter.Brands!= null && filter.Brands.Any()){
                                    query= query.AndWhere("b.Name IN $brandNames")
                                     .WithParam("brandNames", filter.Brands);
                                }
                                if(filter.SkinTypes!= null & filter.SkinTypes.Any()){
                                    query= query.AndWhere("st.SType IN $filters")
                                    .WithParam("filters", filter.SkinTypes);
                                }
                                if(filter.ProductTypes!= null && filter.ProductTypes.Any()){
                                    query=query
                                    .AndWhere("pt.Name IN $filter")
                                     .WithParam("filter",filter.ProductTypes);
                                }
                                if(filter.Indredients!= null && filter.Indredients.Any()){
                                    query= query
                                    .AndWhere("i.Name IN $ingredientNames")
                                    .WithParam("ingredientNames", filter.Indredients);
                                }
                                
                                var result = await query.ReturnDistinct(p => p.As<Product>())
                                                        .ResultsAsync;

                                var pl = result.ToList();

             
                    return Ok(pl);                
        }

        [HttpGet]
        [Route("Recommend_ProductsByBrand/{username}")]
        public async Task<IActionResult> RecommendProductsByBrand(string username){
            
            var query= await _client.Cypher.Match("(u:User)-[:LIKES]->(p:Product)-[rel:SOLD_BY]->(b:Brand)")
                                     .Where((User u)=> u.Username== username)
                                     .ReturnDistinct((b, rel)=> new {
                                        Brand= b.As<Brand>().Name,
                                        Likes= rel.Count()
                                     })
                                     .OrderByDescending("Likes")
                                     .Limit(1)
                                     .ResultsAsync;
            var lista= query.Single();
            var brandName= lista.Brand;

            var topProducts= await _client.Cypher.Match("(b:Brand)<-[:SOLD_BY]-(p:Product)<-[rel:LIKES]-()")
                                                 .Where((Brand b)=> b.Name==brandName)
                                                 .ReturnDistinct((p,rel)=> new{
                                                    Product= p.As<Product>(),
                                                    Likes= rel.Count()
                                                 })
                                                 .OrderByDescending("Likes")
                                                 .Limit(5)
                                                 .ResultsAsync;
            

            return Ok(topProducts.ToList());
        }


        [HttpGet]
        [Route("Recommend_ProductsBySimilarity/{username}")]
        public async Task<IActionResult> RecommendProductBySimilarity(string username){
            
            

            var lastLiked= await _client.Cypher.Match("(u:User)-[rel:LIKES]->(p:Product)")
                                               .Where((User u)=> u.Username== username)
                                               .ReturnDistinct((p, rel)=> new{
                                                    Product= p.As<Product>(),
                                                    LastId= rel.As<string>()
                                                })
                                               .OrderByDescending("LastId")
                                               .Limit(5)
                                               .ResultsAsync;
            var lista= new List<Product>();

            foreach (var p in lastLiked.ToList()){
                lista.Add(p.Product);
            }
            var userSkin= new List<string>();
            var notLiked= new List<Product>();

            var products=new List<Product>();
            foreach(Product product in lista){
           
                var query= await _client.Cypher.Match("(u:User)-[rel:LIKES]->(p:Product)-[:INTENDED_FOR]->(st:SkinType)")
                                           .Where((Product p)=> p.ProductName==product.ProductName)
                                           .AndWhere((User u)=>u.Username!=username)
                                           .ReturnDistinct((u, rel, st)=> new{
                                                User= u.As<User>().Username,
                                                DateLiked= rel.As<string>(),
                                                SkinType= st.As<SkinType>().SType
                                           })
                                           .OrderByDescending("DateLiked")
                                           .Limit(1)
                                           .ResultsAsync;
                    var listt=query.ToList();
                    if(listt.Count!=0)
                    {
                        userSkin.Add(query.Single().User);
                        userSkin.Add(query.Single().SkinType);
                    }
                    else 
                    {
                        notLiked.Add(product);
                    }
                    
            }
            if(userSkin.Count<10)
            {
                foreach(Product pro in notLiked)
                {
                    string pr = pro.ProductName;

                    var query =  await _client.Cypher.Match("(u:User)-[rel:LIKES]->(p:Product)-[rel1:USED_AS]->(t:ProductType),(p1:Product{ProductName: $pr})-[r:USED_AS]->(t1:ProductType)")
                    .WithParam("pr",pr)
                    .Where((User u)=>u.Username!=username)
                    .AndWhere("t1.Name = t.Name")
                    .ReturnDistinct((p, rel, st)=> new{
                        Product= p.As<Product>(),
                        DateLiked= rel.As<string>(),
                        })
                    .OrderByDescending("DateLiked")
                    .Limit(5)
                    .ResultsAsync;             
                    List<Product> prod= new List<Product>();
                    var listt= query.ToList();
                    foreach(var list in listt){
                        prod.Add(list.Product);
                    }
                    var flag= false;
                    int ii=0;
                    while(flag==false && ii<prod.Count()){
                        if(products.Where(p=>p.ProductName==prod[ii].ProductName).FirstOrDefault()==null){
                            products.Add(prod[ii]);
                            flag=true;
                        }
                        ii++;
                    }

                }
       
                
            }
            int i=0;
            while(i<userSkin.Count)
            {
                string usercic=userSkin[i];
                string skinTip= userSkin[i+1];
                var recommendeee= await _client.Cypher.Match("(u1:User)-[rel1:LIKES]->(p:Product)-[m:INTENDED_FOR]->(st1:SkinType)")
                                    .Where("NOT ( :User {Username:$username})-[:LIKES]->(p)")
                                    .WithParam("username",username)
                                    .AndWhere((User u1)=> u1.Username== usercic)
                                    .AndWhere((SkinType st1)=> st1.SType== skinTip)
                                    .ReturnDistinct((p, rel1)=> new{
                                        Product= p.As<Product>(),
                                        DateLiked= rel1.As<string>()
                                    })
                                    .OrderByDescending("DateLiked")
                                    .Limit(5)
                                    .ResultsAsync;
                                    var flag=false;
                                    List<Product> prod= new List<Product>();
                                    var listt= recommendeee.ToList();
                                    foreach(var list in listt){
                                     prod.Add(list.Product);
                                    }
                                     int ii=0;
                                    while(flag==false && ii<prod.Count()){
                                    if(products.Where(p=>p.ProductName==prod[ii].ProductName).FirstOrDefault()==null){
                                    products.Add(prod[ii]);
                                    flag=true;
                                    }
                                    ii++;
                                    }
                i=i+2;
            }
            var productss=products.Distinct().ToList();
            if(productss.Count<5)
            {
            
                var dict = new Dictionary<Product,Product>();
                foreach (var pr in productss)
                {
                    dict.Add(pr,pr);

                }
                var rest= 5-productss.Count;
                var recommendeee= await _client.Cypher.Match("(p:Product)<-[r:LIKES]-()")
                                    .Where("NOT ( :User {Username:$username})-[:LIKES]->(p)")
                                    .WithParam("username",username)
                                    .AndWhere("NOT p.ProductName IN $productss")
                                    .WithParam("productss",productss)
                                    .Return((p, r) => new {
                                        Product = p.As<Product>(),
                                        Likes = r.Count()
                                    })
                                    .OrderByDescending("Likes")
                                    .Limit(rest)
                                    .ResultsAsync;
                                    foreach(var k in recommendeee.ToList())
                                        productss.Add(k.Product);    
           
                
            }
                return Ok(productss);
        }
        }
}
