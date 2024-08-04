using System.Collections.ObjectModel;

namespace APICatalogo.Models;

public class Categoria
{
    // Inicilizar a Collection. É propriedade da classe onde usa a collection inicializá-la
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
    
    // FK com a tabela Produtos (só isso já seria o suficiente para mapear a FK, porém,
    // vamos incluir uma propriedade de navegação na classe produto também).
    public ICollection<Produto>? Produtos { get; set; }
}
