using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

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

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> get([FromQuery] CategoriasParameters categoriaParams)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriaParams);
           
            return ObterCategoria(categorias);
        }

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategoria(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            // Inclui no header utilizando a lib NewtonSoft.Json
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFIltradas([FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);

            return ObterCategoria(categoriasFiltradas);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            _logger.LogInformation("========== Get/Categorias ==========");

            var categorias = await _uof.CategoriaRepository.GetAllAsync();

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);

        }

        [HttpGet("{id:int}", Name = "ObteCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            _logger.LogInformation("========== Get/Categorias/{id} ==========");
            // Teste middleware de exceptions sem tratamentos
            //throw new Exception("Exceção ao retornar a categoria pelo Id");

            var categorias = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categorias == null)
            {
                _logger.LogWarning($"Categoria com id = {id} não encontrada...");
                return NotFound($"Categoria com id = {id} não encontrada...");
            }     
            
            var categoriaDto = categorias.ToCategoriaDTO();

            return Ok(categorias);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto == null)
            {
                _logger.LogWarning($"Dados inválidos...");
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            
            _uof.CommitAsync();

            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning($"Dados inválidos...");
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);            

            _uof.CommitAsync();

            var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

            return Ok(categoriaAtualizadaDto);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com id = {id} não encontrada...");
                return NotFound(new { retorno = "Categoria não localizado..." });
            }

            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);

            _uof.CommitAsync();

            var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

            return Ok(categoriaExcluidaDto);
        }

    }
}
