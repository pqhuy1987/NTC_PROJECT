using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace RICONS.DataServices.Executes
{
    public class Foo
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        // other props
    }

    public class BaseService
    {
        public JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public List<Foo> GetChildItems(List<Foo> foos, int id)
        {
            return foos
                .Where(x => x.ParentId == id)
                .Union(foos.Where(x => x.ParentId == id)
                    .SelectMany(y => GetChildItems(foos, y.Id))
                ).ToList();
        }

       
    }
}
