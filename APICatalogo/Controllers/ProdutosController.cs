using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers
{
    //[Route("[controller]/aprovar-orcamento")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParameters);

            return ObterProdutos(produtos);
        }

        [HttpGet("filter/preco/pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco filtro)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(filtro);

            return ObterProdutos(produtos);
        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.TotalItemCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("produtos/{id}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

            if (produtos is null) return NotFound();

            var dto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(dto);
        }

        // api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();

            if (produtos is null) return NotFound("Produtos não encontrados...");

            var dto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(dto);
        }

        // api/ObterProduto/id
        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
         
            if (produto == null) return NotFound("Produto não encontrado");

            var dto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto == null) return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoCriado = _unitOfWork.ProdutoRepository.Create(produto);

            var produtoDtoCriado = _mapper.Map<Produto>(produtoCriado);

            _unitOfWork.CommitAsync();

            return new CreatedAtRouteResult("ObterProduto",
                new {id = produtoDtoCriado.ProdutoId}, produtoDto);

        }

        [HttpPatch("{id}/UpdatePartial")]
        public async Task<ActionResult<ProdutoDTOUpdateRequest>> Patch(int id,
            JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO == null || id <= 0) return BadRequest();

            var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

            if (produto == null) return BadRequest();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest)) return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);

            _unitOfWork.ProdutoRepository.Update(produto);
            _unitOfWork.CommitAsync();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        // api/produtos/id -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request PUT
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId) return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);

            if (produtoAtualizado == null) return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");          

            var produtoDtoCriado = _mapper.Map<Produto>(produtoAtualizado);

            _unitOfWork.CommitAsync();

            return Ok(produtoAtualizado);

        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request DELETE
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

            if (produto == null)
            {
                return StatusCode(500, $"Falha ao excluir o produto de id = {id}");
            }           

            var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);

            _unitOfWork.CommitAsync();

            return Ok(produtoDeletado);

        }


    }
}
