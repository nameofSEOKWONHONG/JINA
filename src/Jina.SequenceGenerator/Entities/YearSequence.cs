using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jina.SequenceGenerator.Entities;

[Table("YearSequences", Schema = "dbo")]
public class YearSequence
{
    [Key, Column(Order = 0)]
    public string TenantId { get; set; }

    [Key, Column(Order = 1)]
    public string TableName { get; set; }
    
    [Key, Column(Order = 2)]
    public int Year { get; set; }
    
    [Key, Column(Order = 3)]
    public Int64 Id { get; set; }

    public YearSequence(string tenantId, string tableName, int year, Int64 id)
    {
        this.TenantId = tenantId;
        this.TableName = tableName;
        this.Year = year;
        this.Id = id;
    }
}