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
        public ActionResult<IEnumerable<ProdutoDTO>> get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = _unitOfWork.ProdutoRepository.GetProdutos(produtosParameters);           

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            // Inclui no header utilizando a lib NewtonSoft.Json
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata)); 
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }
        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int id)
        {
            var produtos = _unitOfWork.ProdutoRepository.GetProdutosPorCategoria(id);

            if (produtos is null) return NotFound();

            var dto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(dto);
        }

        // api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _unitOfWork.ProdutoRepository.GetAll();

            if (produtos is null) return NotFound("Produtos não encontrados...");

            var dto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(dto);
        }

        // api/ObterProduto/id
        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _unitOfWork.ProdutoRepository.Get(p => p.ProdutoId == id);
         
            if (produto == null) return NotFound("Produto não encontrado");

            var dto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(dto);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto == null) return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoCriado = _unitOfWork.ProdutoRepository.Create(produto);

            var produtoDtoCriado = _mapper.Map<Produto>(produtoCriado);

            _unitOfWork.Commit();

            return new CreatedAtRouteResult("ObterProduto",
                new {id = produtoDtoCriado.ProdutoId}, produtoDto);

        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateRequest> Patch(int id,
            JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO == null || id <= 0) return BadRequest();

            var produto = _unitOfWork.ProdutoRepository.Get(c => c.ProdutoId == id);

            if (produto == null) return BadRequest();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest)) return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);

            _unitOfWork.ProdutoRepository.Update(produto);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        // api/produtos/id -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request PUT
        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId) return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);

            if (produtoAtualizado == null) return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");          

            var produtoDtoCriado = _mapper.Map<Produto>(produtoAtualizado);

            _unitOfWork.Commit();

            return Ok(produtoAtualizado);

        }

        // api/produtos -> mesmo tendo a mesma roda, o verbo é diferente. Então o método só será atendido pelo request DELETE
        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _unitOfWork.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto == null)
            {
                return StatusCode(500, $"Falha ao excluir o produto de id = {id}");
            }           

            var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);

            _unitOfWork.Commit();

            return Ok(produtoDeletado);

        }


    }
}
