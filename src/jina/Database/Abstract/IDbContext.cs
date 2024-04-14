using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Jina.Database.Abstract;

public interface IDbContext
{
    DatabaseFacade Database { get; }
}