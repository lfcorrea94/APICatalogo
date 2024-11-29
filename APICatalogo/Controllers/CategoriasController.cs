using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")] // Roteamento padrão para o nome do controlador 'Categorias'
    [ApiController] // Aplica regras de inferência para fontes de dados (abstrair notações como FromBody, FromForm, etc)
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            _logger.LogInformation("========== Get/Categorias ==========");

            var categorias = _uof.CategoriaRepository.GetAll();

            return Ok(categorias);

        }

        [HttpGet("{id:int}", Name = "ObteCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            _logger.LogInformation("========== Get/Categorias/{id} ==========");
            // Teste middleware de exceptions sem tratamentos
            //throw new Exception("Exceção ao retornar a categoria pelo Id");

            var categorias = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categorias == null)
            {
                _logger.LogWarning($"Categoria com id = {id} não encontrada...");
                return NotFound($"Categoria com id = {id} não encontrada...");
            }            
            return Ok(categorias);
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria == null)
            {
                _logger.LogWarning($"Dados inválidos...");
                return BadRequest();
            }

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            
            _uof.Commit();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                _logger.LogWarning($"Dados inválidos...");
                return BadRequest();
            }
            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);            

            _uof.Commit();

            return Ok(categoriaAtualizada);

        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com id = {id} não encontrada...");
                return NotFound(new { retorno = "Categoria não localizado..." });
            }

            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);

            _uof.Commit();

            return Ok(categoriaExcluida);
        }

    }
}
