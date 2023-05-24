using kodbankasi.IServices;
using kodbankasi.Models;
using Microsoft.Extensions.Configuration;

namespace kodbankasi.Services
{
    public class KategoriService : Service<Kategori>,IKategoriService
    {
        public KategoriService(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
