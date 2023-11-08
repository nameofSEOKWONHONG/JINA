namespace Jina.Storage.Abstract;

public interface IStorage
{
    Task<string> FileDownloadUrl(string fullUri, string fileName);
    Task<bool> FileDelete(string userId, string hashedFileName);
    Task<bool> FileExist(string userId, string hashedFileName);
}