using System.Transactions;
using Jina.SequenceGenerator.Dto;
using Jina.SequenceGenerator.Entities;
using Jina.SequenceGenerator.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Jina.SequenceGenerator.Services;

public class NormalSequenceGenerator : ISequenceGenerator
{
    private readonly DbContext _db;
    public NormalSequenceGenerator(DbContext db)
    {
        _db = db;
    }

    public async Task<Int64> CreateAsync(SequenceOption option)
    {
        if (this._db.Database.CurrentTransaction == null) throw new TransactionException("sequence need transaction.");
        
        var seq = _db.Set<Sequence>();
        var max = await seq.Where(m => m.TenantId == option.TenantId && m.TableName == option.TableName).MaxAsync(m => m.Id);
        if (max <= 0)
        {
            max = option.Length;
            if (max <= 0) max = 1;
            await seq.AddAsync(new(option.TenantId, option.TableName, max));
        }
        else
        {
            if (option.Length > 0)
            {
                max += option.Length;    
            }
            else
            {
                max += 1;
            }
            
            seq.Update(entity: new(option.TenantId, option.TableName, max));
        }

        await _db.SaveChangesAsync();
        return max;
    }

    public async Task<Int64> CreateAsync<T>(SequenceOption option)
    {
        return await this.CreateAsync(new SequenceOption()
        {
            TenantId = option.TenantId, 
            TableName = nameof(T),
            Length = option.Length
        });
    }
}

