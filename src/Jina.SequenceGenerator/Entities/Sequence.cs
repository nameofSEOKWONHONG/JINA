using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jina.SequenceGenerator.Entities;

[Table("Sequences", Schema = "dbo")]
public class Sequence
{
    [Key, Column(Order = 0)]
    public string TenantId { get; set; }

    [Key, Column(Order = 1)]
    public string TableName { get; set; }
    
    [Key, Column(Order = 2)]
    public Int64 Id { get; set; }

    public Sequence(string tenantId, string tableName, Int64 id)
    {
        this.TenantId = tenantId;
        this.TableName = tableName; 
        this.Id = id;
    }
}