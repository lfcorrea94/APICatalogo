using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public PagedList<Categoria> GetCategorias(CategoriasParameters categoriaParams)
        {
            var categoria = GetAll().OrderBy(p => p.CategoriaId).AsQueryable();

            var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categoria, categoriaParams.PageNumber, categoriaParams.PageSize);

            return categoriasOrdenadas;

        }
    }
}
