using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    //[Route("[controller]/aprovar-orcamento")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
                _context = context;
        }

        // api/produtos
        [HttpGet("primeiro")]
        public ActionResult<Produto> GetPrimeiro()
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

            if (produto is null) return NotFound("Produto não encontrados...");

            return produto;
        }

        // api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _context.Produtos.AsNoTracking().ToList();

            if (produtos is null) return NotFound("Produtos não encontrados...");
            
            return produtos;
        }

        // api/produtos/id
        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
         
            if (produto == null) return NotFound("Produto não encontrado");
     
            return produto;
        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request POST
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto == null) return BadRequest();
            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produto);
        }

        // api/produtos/id -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request PUT
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId) return BadRequest();

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);

        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request DELETE
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            if (produto == null) return NotFound(new { retorno = "Produto não localizado..." });

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(produto);

        }


    }
}
