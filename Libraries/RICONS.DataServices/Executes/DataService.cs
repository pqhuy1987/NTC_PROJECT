using RICONS.DataServices.Context;

namespace RICONS.DataServices.Executes
{
    public partial class DataService : BaseService
    {
        public int Loop { get; set; }
        public DBContext Context { get; set; }
        
        public void CheckDbConnect()
        {
            if (Context == null)
            {
                Loop = 1;
                Context = new DBContext();
            }
            else
            {
                Loop++;
                if (Loop == 100)
                {
                    Dispose();
                    Context = new DBContext();
                }
            }
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}