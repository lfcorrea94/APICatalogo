using System.Linq.Expressions;

namespace APICatalogo.Repositories.Interface
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll();
        // utilização de um delegate pra retornar a entidade T com filtro genérico. Nada mais é do que uma expressão lambda genérica
        public T? Get(Expression<Func<T, bool>> predicate);
        public T Create(T entity);
        public T Update(T entity);
        public T Delete(T entity);

    }
}
