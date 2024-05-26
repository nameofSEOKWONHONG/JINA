using System.Data;
using System.Data.Common;
using eXtensionSharp;
using FreeSql;
using Jina.Database.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Database;

public class FSql : IFSql
{
    public IFreeSql FreeSql { get; init; }
    public DbTransaction CurrentTransaction { get; private set; }
    
    public FSql(IFreeSql freeSql)
    {
        FreeSql = freeSql;
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted, CancellationToken ct = default)
    {
        var connection = await FreeSql.Ado.MasterPool.GetAsync();
        if (connection.xIsEmpty()) throw new Exception("freesql connection is empty");
        CurrentTransaction = await connection.Value.BeginTransactionAsync(isolationLevel, ct); 
    }

    public ISelect<TEntity> Select<TEntity>() where TEntity : class
    {
        if (this.CurrentTransaction.xIsNotEmpty())
            return this.FreeSql.Select<TEntity>()
                .WithTransaction(this.CurrentTransaction);
        else
            return this.FreeSql.Select<TEntity>();
    }
    
    public IInsert<TEntity> Insert<TEntity>() where TEntity : class
    {
        if (this.CurrentTransaction.xIsNotEmpty())
        {
            return this.FreeSql.Insert<TEntity>()
                .WithTransaction(this.CurrentTransaction);            
        }
        else
        {
            return this.FreeSql.Insert<TEntity>();
        }
    }

    public IUpdate<TEntity> Update<TEntity>() where TEntity : class
    {
        if (this.CurrentTransaction.xIsNotEmpty())
        {
            return this.FreeSql.Update<TEntity>()
                .WithTransaction(this.CurrentTransaction);            
        }
        else
        {
            return this.FreeSql.Update<TEntity>();
        }
    }
    
    public IDelete<TEntity> Delete<TEntity>() where TEntity : class
    {
        if (this.CurrentTransaction.xIsNotEmpty())
        {
            return this.FreeSql.Delete<TEntity>()
                .WithTransaction(this.CurrentTransaction);            
        }
        else
        {
            return this.FreeSql.Delete<TEntity>();
        }

    }
}

public static class FSqlExtensions
{
    public static IServiceCollection AddFSql(this IServiceCollection services, string connection)
    {
        services.AddScoped<IFSql, FSql>();
        services.AddSingleton<IFreeSql>(r =>
        {
            /*
             * freesql을 사용하려면 Entity 구성 Attribute를 사용하여 작성해야 함.
             * 장점 : Entity 구성 요소를 직접 사용하여 Fluent 방식으로 쿼리 작성 가능.
             *       동적 Table 생성 지원.
             *       SqlKata가 Entity와 string 조합이라면, 이쪽은 Entity 중심으로 개발 가능.
             * 단점 : EF와 혼합하여 사용할 경우 매우 곤란해 질 수 있음.
             *       Table 매핑부터 각종 설정까지 2가지를 모두 고려하여 작성할 수는 없음.
             *       따라서, 양자 택일이 강요됨.
             *       (예: ModelBuilder 특성과 Table Attribute의 특성)
             * 결론 : DB First로 작업한다면 EF 보다 좋을 수 있다.
             *       FK에 비관적이라면 이쪽이 더 좋을 수 있다.
             *       복잡한 집계 쿼리는 어짜피 프로시저의 몫이다.
             * 특이사항 : UOW의 Repository 패턴 기능과 EF DbContext 기능을 둘다 제공한다.
             *          재미있는게 ADO 기능도 제공하고, 뭔가 종합선물세트 느낌인데...
             *          예전처럼 결론은 중국 제작이라 믿고 쓸수 있을지인데...
             *          SQLKATA도 있으니 여차하면...
             */
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.SqlServer, connection)
#if DEBUG
                .UseMonitorCommand(cmd => Console.WriteLine($"Sql: {cmd.CommandText}"))
                //Automatically synchronize the entity structure to the database.
                //FreeSql will not scan the assembly, and will generate a table if and only when the CRUD instruction is executed.
                .UseAutoSyncStructure(false)
#endif
                .UseLazyLoading(true)
                    
                .Build();
            return fsql;
        });        
        return services;
    }
}