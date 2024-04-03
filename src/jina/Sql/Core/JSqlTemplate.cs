namespace JSqlEngine.Core;

public class JSqlTemplate
{
    internal static string SQL_CODE = "@@CODE";
    
    internal const string JSQL_TEMPLATE = """
                                              function jsql(obj) {
                                                  var query = `
                                                  SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                                                  SET NOCOUNT ON;
                                                  `;
                                                  
                                                  const isCount = false;
                                                  
                                                  @@CODE
                                                  
                                                  return query.concat(sql);
                                              }
                                          """;

    internal const string JSQL_COUNT_TEMPLATE = """

                                                function jsql(obj) {
                                                    var query = `
                                                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                                                    SET NOCOUNT ON;
                                                    
                                                    SELECT COUNT(*)
                                                    FROM (
                                                    `;
                                                    
                                                    const isCount = true;
                                                    
                                                    @@CODE
                                                    
                                                    query = query.concat(sql);
                                                    
                                                    query += ' ) A ';
                                                    
                                                    return query;
                                                }

                                                """;

    internal const string JSQL_PAING_TEMPLATE = """

                                                function jsql(obj) {
                                                   var query = `
                                                   SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                                                   SET NOCOUNT ON;
                                                   `;
                                                   
                                                   const isCount = false;
                                                   
                                                   @@CODE
                                                   
                                                   query = query.concat(sql);
                                                   query += ' OFFSET @PAGE_NUMBER ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY ';
                                                   
                                                   return query;
                                                }

                                                """;    
}