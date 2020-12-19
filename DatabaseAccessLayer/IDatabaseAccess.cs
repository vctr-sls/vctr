using DatabaseAccessLayer.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DatabaseAccessLayer
{
    public interface IDatabaseAccess
    {
        Task<long> GetCount<T>() where T : EntityModel;

        Task<T> GetById<T>(Guid id) where T : EntityModel;

        IQueryable<T> GetAll<T>() where T : EntityModel;

        IQueryable<T> GetWhere<T>(Expression<Func<T, bool>> preticate) where T : EntityModel;

        T Create<T>(T model) where T : EntityModel;

        T Update<T>(T model) where T : EntityModel;

        T Delete<T>(T model) where T : EntityModel;

        Task Commit();
    }
}
