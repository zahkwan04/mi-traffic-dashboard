using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace systemapps.Repositories
{
    public interface IDatabaseRepository
    {
        Task<bool> TestConnectionAsync(string ip, string dbName, string user, string password);
        Task<DataTable> ExecuteQueryAsync(string ip, string dbName, string user, string password, string sql, params MySqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string ip, string dbName, string user, string password, string sql, params MySqlParameter[] parameters);
    }

    public class MySqlRepository : IDatabaseRepository
    {
        private string GetConnectionString(string ip, string dbName, string user, string password) 
            => $"Server={ip};Database={dbName};Uid={user};Pwd={password};SslMode=none";

        public async Task<bool> TestConnectionAsync(string ip, string dbName, string user, string password)
        {
            try
            {
                using var con = new MySqlConnection(GetConnectionString(ip, dbName, user, password));
                await con.OpenAsync();
                using var cmd = new MySqlCommand("SELECT 1", con);
                await cmd.ExecuteScalarAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> ExecuteQueryAsync(string ip, string dbName, string user, string password, string sql, params MySqlParameter[] parameters)
        {
            using var dt = new DataTable();
            using var con = new MySqlConnection(GetConnectionString(ip, dbName, user, password));
            await con.OpenAsync();
            using var cmd = new MySqlCommand(sql, con);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        public async Task<int> ExecuteNonQueryAsync(string ip, string dbName, string user, string password, string sql, params MySqlParameter[] parameters)
        {
            using var con = new MySqlConnection(GetConnectionString(ip, dbName, user, password));
            await con.OpenAsync();
            using var cmd = new MySqlCommand(sql, con);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
