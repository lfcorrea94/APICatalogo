using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories.Interface;
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
        //private readonly IRepository<Produto> _repository;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(IProdutoRepository produtoRepository)
        {
            //_repository = repository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int id)
        {
            var produtos = _produtoRepository.GetProdutosPorCategoria(id);

            if (produtos is null) return NotFound();

            return Ok(produtos);
        }

        // api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _produtoRepository.GetAll();

            if (produtos is null) return NotFound("Produtos não encontrados...");
            
            return Ok(produtos);
        }

        // api/ObterProduto/id
        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _produtoRepository.Get(p => p.ProdutoId == id);
         
            if (produto == null) return NotFound("Produto não encontrado");
     
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto == null) return BadRequest();
            var produtoCriado = _produtoRepository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto",
                new {id = produtoCriado.ProdutoId}, produtoCriado);

        }

        // api/produtos/id -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request PUT
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId) return BadRequest();

           var produtoAtualizado = _produtoRepository.Update(produto);

            if (produtoAtualizado == null)
            {
                return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");
            }

            return Ok(produtoAtualizado);

        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request DELETE
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _produtoRepository.Get(p => p.ProdutoId == id);

            if (produto == null)
            {
                return StatusCode(500, $"Falha ao excluir o produto de id = {id}");
            }           

            var produtoDeletado = _produtoRepository.Delete(produto); 

            return Ok(produtoDeletado);

        }


    }
}
