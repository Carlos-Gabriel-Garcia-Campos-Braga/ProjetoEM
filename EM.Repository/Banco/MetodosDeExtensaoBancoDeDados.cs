using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository.Banco
{
    public static class MetodosDeExtensaoBancoDeDados
    {
        public static void CreateParameter(this DbParameterCollection dbParameter, string parameterName, object? value)
        {
            dbParameter.Add(new FbParameter(parameterName, value));
        }

        public static object? ToDb(this object? value) =>  value == null ? DBNull.Value : value;
    }
}

