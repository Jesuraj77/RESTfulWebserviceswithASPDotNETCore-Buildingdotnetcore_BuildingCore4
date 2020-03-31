

using BuildingCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingCore.DataLayer.NHibernateConfigurations
{

    public interface IMapperSession
    {
        void BeginTransaction();
        System.Threading.Tasks.Task Commit();
        System.Threading.Tasks.Task Rollback();
        void CloseTransaction();
        System.Threading.Tasks.Task Save(List<Customer> entity);
        System.Threading.Tasks.Task Delete(Customer entity);
        IQueryable<Customer> Customer { get; }
    }
}

