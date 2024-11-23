using APICatalogo.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace APICatalogo.Repositories
{
    public interface IProdutoRepository
    {
        // Definição dos contratos
        IQueryable<Produto> GetProdutos();
        Produto GetProduto(int id);
        Produto Create(Produto categoria);
        bool Update(Produto categoria);
        bool Delete(int id);

    }
}
