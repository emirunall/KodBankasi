using kodbankasi.Dto;
using kodbankasi.IServices;
using kodbankasi.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace kodbankasi.Services
{
    public class KodService : Service<Kod>,IKodService
    {
        private readonly string _connectionString;
        public KodService(IConfiguration configuration) : base(configuration)
        {
            _connectionString = "Server=EMIR;Database=kodbankasidb;Trusted_Connection=True;";
        }

      public async Task<IEnumerable<KodDto>> GetAllKodWithKategoriNamesAsync()
{
    var kodDtoList = new List<KodDto>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT k.id, k.Name, k.Description, ka.Name AS KategoriName, ka.id AS KategoriId
            FROM Kod k
            LEFT JOIN Kategori ka ON k.KategoriId = ka.id", connection);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var kodDto = new KodDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    KategoriName = reader.IsDBNull(reader.GetOrdinal("KategoriName")) ? null : reader.GetString(reader.GetOrdinal("KategoriName")),
                    KategoriId = reader.IsDBNull(reader.GetOrdinal("KategoriId")) ? null : reader.GetInt32(reader.GetOrdinal("KategoriId"))
                };

                kodDtoList.Add(kodDto);
            }
        }
    }

    return kodDtoList;
}
        public async Task<KodDto> GetByIdWithKategoriNamesAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
        SELECT k.id, k.Name, k.Description, ka.Name AS KategoriName, ka.id AS KategoriId
        FROM Kod k
        LEFT JOIN Kategori ka ON k.KategoriId = ka.id
        WHERE k.id = @id", connection);

                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var kodDto = new KodDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            KategoriName = reader.IsDBNull(reader.GetOrdinal("KategoriName")) ? null : reader.GetString(reader.GetOrdinal("KategoriName")),
                            KategoriId = reader.IsDBNull(reader.GetOrdinal("KategoriId")) ? null : reader.GetInt32(reader.GetOrdinal("KategoriId"))
                        };

                        return kodDto;
                    }
                }
            }

            return null;
        }



    }
}
