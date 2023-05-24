using kodbankasi.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace kodbankasi.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public Service(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "Server=EMIR;Database=kodbankasidb;Trusted_Connection=True;";
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand($"SELECT * FROM {typeof(T).Name}", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<T>();

                        foreach (var property in entity.GetType().GetProperties())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                property.SetValue(entity, reader[property.Name]);
                            }
                        }

                        entities.Add(entity);
                    }
                }
            }

            return entities;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = Activator.CreateInstance<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand($"SELECT * FROM {typeof(T).Name} WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        foreach (var property in entity.GetType().GetProperties())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                property.SetValue(entity, reader[property.Name]);
                            }
                        }
                    }
                    else
                    {
                        entity = null;
                    }
                }
            }

            return entity;
        }

        public async Task<int> CreateAsync(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand();
                var columns = "";
                var values = "";
                var parameters = new List<SqlParameter>();

                foreach (var property in entity.GetType().GetProperties())
                {
                    if (!property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        columns += $"{property.Name}, ";
                        values += $"@{property.Name}, ";
                        parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(entity)));
                    }
                }
                char[] charsToTrim = { ',', ' ' };
                columns = columns.TrimEnd(charsToTrim);
                values = values.TrimEnd(charsToTrim);

                command.CommandText = $"INSERT INTO {typeof(T).Name} ({columns}) VALUES ({values}); SELECT CAST(scope_identity() AS int);";
                command.Connection = connection;

                command.Parameters.AddRange(parameters.ToArray());

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand();
                var setValues = "";
                var parameters = new List<SqlParameter>();

                foreach (var property in entity.GetType().GetProperties())
                {
                    if (!property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        setValues += $"{property.Name} = @{property.Name}, ";
                        parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(entity)));
                    }
                }
                char[] charsToTrim = { ',', ' ' };
                setValues = setValues.TrimEnd(charsToTrim);

                command.CommandText = $"UPDATE {typeof(T).Name} SET {setValues} WHERE id = @id;";
                command.Connection = connection;

                command.Parameters.AddWithValue("@id", entity.GetType().GetProperty("id").GetValue(entity));
                command.Parameters.AddRange(parameters.ToArray());

                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand($"DELETE FROM {typeof(T).Name} WHERE id = @id;", connection);

                command.Parameters.AddWithValue("@id", id);

                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
