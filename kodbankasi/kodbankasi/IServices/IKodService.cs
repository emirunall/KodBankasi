using kodbankasi.Dto;
using kodbankasi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kodbankasi.IServices
{
    public interface IKodService: IService<Kod>
    {
        Task<IEnumerable<KodDto>> GetAllKodWithKategoriNamesAsync();
        Task<KodDto> GetByIdWithKategoriNamesAsync(int id);

    }
}
