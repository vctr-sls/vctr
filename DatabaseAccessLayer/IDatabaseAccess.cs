using DatabaseAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace DatabaseAccessLayer
{
    public interface IDatabaseAccess
    {
        Task<long> GetCount<T>() where T : EntityModel;

        Task<T> Create<T>(T model) where T : EntityModel;

        Task<T> GetById<T>(Guid id) where T : EntityModel;

        Task Commit();
    }
}
