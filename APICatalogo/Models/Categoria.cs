using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models;

[Table("Categorias")]
public class Categoria
{
    // Inicilizar a Collection. É propriedade da classe onde usa a collection inicializá-la
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
    [Key]
    public int CategoriaId { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    
    // FK com a tabela Produtos (só isso já seria o suficiente para mapear a FK, porém,
    // vamos incluir uma propriedade de navegação na classe produto também).
    public ICollection<Produto>? Produtos { get; set; }
}
