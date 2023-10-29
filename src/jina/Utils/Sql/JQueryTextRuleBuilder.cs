using eXtensionSharp;

namespace Jina.Utils.Sql;

public class JQueryTextRuleBuilder
{
    private static Lazy<JQueryTextRuleBuilder> _instance = new Lazy<JQueryTextRuleBuilder>(() => new JQueryTextRuleBuilder());
    public static JQueryTextRuleBuilder Instance => _instance.Value;

    private const string SELECT_QUERY_TEMPLATE = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

@@QUERY
";
    private const string QUERY_CODE = "@@QUERY";
    
    private JQueryTextRuleBuilder()
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
        return JQueryTextRuleBuilder.Instance.SelectBuild(query);
    }
}