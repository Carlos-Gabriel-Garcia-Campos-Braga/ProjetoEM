using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;

namespace EM.Repository.Banco
{
    public static class MetodosDeExtensaoBancoDeDados
    {
        public static void CreateParameter(this DbParameterCollection dbParameter, string parameterName, object? value)
        {
            dbParameter.Add(new FbParameter(parameterName, value));
        }

        public static object? ToDb(this object? value) => value ?? DBNull.Value;
    }
}

