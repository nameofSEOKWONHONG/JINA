namespace Jina.Storage.Data;

public class StorageFile
{
    public int Id { get; set; }
    
    /// <summary>
    /// 더미 DOC KEY (실제로 사용하지 않음.)
    /// </summary>
    public string DocKey { get; set; }
    
    /// <summary>
    /// 더미 엔티티명 (실제로 사용하지 않음.) 
    /// </summary>
    public string EntityName { get; set; }
    
    /// <summary>
    /// 파일명 (해시) - 서버용
    /// </summary>
    public string HashedFileName { get; set; }

    /// <summary>
    /// 파일명 (확장자 포함)
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// 크기 (byte)
    /// </summary>
    public long Size { get; set; }
    
    /// <summary>
    /// Content Type
    /// </summary>
    public string ContentType { get; set; }
    
    /// <summary>
    /// download url
    /// </summary>
    public string DownloadUrl { get; set; }
    
    /// <summary>
    /// 업로드 uri
    /// </summary>
    public string FullUri { get; set; }
    
    public byte[] FileBuffer { get; set; }
}