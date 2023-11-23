using Jina.SequenceGenerator.Dto;

namespace Jina.SequenceGenerator.Services.Abstract;

public interface ISequenceGenerator
{
    Task<Int64> CreateAsync(SequenceOption option);
    Task<Int64> CreateAsync<T>(SequenceOption option);
}