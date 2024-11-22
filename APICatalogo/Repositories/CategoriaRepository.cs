using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        // lógica de acesso a dados;
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Categoria> GetCategorias()
        {
            return _context.Categorias.ToList();
        }
        public Categoria GetCategoria(int id)
        {
            return _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        }
        public Categoria Create(Categoria categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return categoria;

        }
        public Categoria Update(Categoria categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);

            // indica que o objeto em memória foi atualizado
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();

            return categoria;
        }
        public Categoria Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            
            if (categoria == null) throw new ArgumentNullException(nameof(categoria));

            _context.Remove(categoria);
            _context.SaveChanges();

            return categoria;
        }

    }
}
