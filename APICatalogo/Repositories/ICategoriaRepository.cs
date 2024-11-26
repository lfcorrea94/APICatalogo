﻿using APICatalogo.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace APICatalogo.Repositories
{
    public interface ICategoriaRepository
    {
        // assinatura dos métodos que serão implementados
        IEnumerable<Categoria> GetCategorias();
        Categoria GetCategoria(int id);
        Categoria Create(Categoria categoria);
        Categoria Update(Categoria categoria);
        Categoria Delete(int id);

    }
}
