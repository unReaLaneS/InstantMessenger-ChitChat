using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    interface IDaoCrud<T>
    {

        void create(T entity);  // INSERT INTO
        //T read(T entity);        // SELECT FROM WHERE ..
        //T update(T entity);     // UPDATE
        //void delete(T entity);  // DELETE

    }
}
