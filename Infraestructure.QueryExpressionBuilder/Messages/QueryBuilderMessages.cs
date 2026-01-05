using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.QueryExpressionBuilder.Messages
{
    internal static class QueryBuilderMessages
    {
        internal const string QueryIsAlreadyInitialized = "Não foi possível compilar a expressão. O método Where só deverá ser utilizado para iniciar a query, utilize os métodos And e Or após o método Where.";
        internal const string QueryIsNotInitialized = "Não foi possível compilar a expressão. A query não foi iniciada, utilize o método WHere antes de chamar quaisquer métodos.";

        internal const string QueryBuilderNotInitialized = "A consulta (query) não foi inicializada no construtor. Certifique-se de chamar o método InitializeQueryBuilder antes de usar a consulta.";
        internal const string ParametersQuantityNotEquals = "A consulta (query) não foi compilada. O número de comparações deve ser igual ao número de argumentos fornecidos";
        internal const string ExpressionIsNull = "A expressão não pôde ser adicionada à Árvore de Expressões. A expressão não pode ser nula.";
        internal const string NestOperationNotSupported = "A expressão não pôde ser adicionada à Árvorre de Expressões. O tipo de expressão informado não é suportado.";
    }
}
