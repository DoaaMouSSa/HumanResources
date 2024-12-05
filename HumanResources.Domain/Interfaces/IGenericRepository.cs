using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetAllWithNoCondtion();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        public Task UpdateAsync(T entity);
        void Delete(T entity);
    }
}
