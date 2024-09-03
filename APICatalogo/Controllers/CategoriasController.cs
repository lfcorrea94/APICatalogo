using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")] // Roteamento padrão para o nome do controlador 'Categorias'
    [ApiController] // Aplica regras de inferência para fontes de dados (abstrair notações como FromBody, FromForm, etc)
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;       
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return _context.Categorias.Include(p=> p.Produtos).AsNoTracking().ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                var categorias = _context.Categorias.AsNoTracking().ToList();

                if (categorias is null) return NotFound("Produtos não encontrados...");
                return categorias;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     "Ocorreu um erro ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int}", Name = "ObteCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                // Teste (tirar o try dps pra testar)
                // throw new Exception("Exceção ao retornar a categoria pelo Id");

                var categorias = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

                if (categorias == null) return NotFound("Categoria não encontrado...");

                return Ok(categorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tratar a sua solicitação.");
            }

        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria == null) return BadRequest();
            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId) return BadRequest();

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(categoria);

        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria == null) return NotFound(new { retorno = "Categoria não localizado..." });

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }

    }
}
