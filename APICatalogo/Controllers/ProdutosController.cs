using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
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
        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;
        }

        // api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _repository.GetProdutos().ToList();

            if (produtos is null) return NotFound("Produtos não encontrados...");
            
            return Ok(produtos);
        }

        // api/produtos/id
        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.GetProduto(id);
         
            if (produto == null) return NotFound("Produto não encontrado");
     
            return Ok(produto);
        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request POST
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto == null) return BadRequest();
            var produtoCriado = _repository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto",
                new {id = produtoCriado.ProdutoId}, produtoCriado);

        }

        // api/produtos/id -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request PUT
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId) return BadRequest();

           var retorno = _repository.Update(produto);

            if (!retorno)
            {
                return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");
            }

            return Ok(retorno);

        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request DELETE
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var retorno = _repository.Delete(id);

            if (!retorno)
            {
                return StatusCode(500, $"Falha ao excluir o produto de id = {id}");
            }           

            return Ok($"Produto de id = {id} foi excluído com sucesso.");

        }


    }
}
