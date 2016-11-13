#region

using System.Linq;

#endregion

namespace Iam.Common.Contracts
{
    public interface IAdminContext
    {
        IQueryable<T> Repository<T>() where T : class, IModel;
        void Add<T>(T entity) where T : class, IModel;
        void Update<T>(T entity) where T : class, IModel;
        void Commit();
    }
}