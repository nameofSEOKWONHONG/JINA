using eXtensionSharp;

namespace Jina.Utils.Sql;

public class QueryTextRuleBuilder
{
    private static Lazy<QueryTextRuleBuilder> _instance = new Lazy<QueryTextRuleBuilder>(() => new QueryTextRuleBuilder());
    public static QueryTextRuleBuilder Instance => _instance.Value;

    private const string SELECT_QUERY_TEMPLATE = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

@@QUERY
";
    private const string QUERY_CODE = "@@QUERY";
    
    private QueryTextRuleBuilder()
    {
        
    }

    public string SelectBuild(string query)
    {
        if (query.ToUpper().Contains("SELECT *").xIsTrue())
            throw new Exception("The '*' in the SELECT statement is not allowed");
        
        if (query.ToUpper().Contains("SELECT").xIsTrue() &&
            query.ToUpper().Contains("FROM"))
        {
            if (query.ToUpper().Contains("WITH(NOLOCK)").xIsFalse())
            {
                throw new Exception("You must use nonblocking keyword. (ex: FROM SYSUSER A WITH(NOLOCK)");
            }            
        }
        return SELECT_QUERY_TEMPLATE.Replace(QUERY_CODE, query);
    }
}

public static class QueryTextBuilderExtensions
{
    public static string vSelectBuild(this string query)
    {
        return QueryTextRuleBuilder.Instance.SelectBuild(query);
    }
}