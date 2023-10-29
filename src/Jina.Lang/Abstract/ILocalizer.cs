namespace Jina.Lang.Abstract;

public interface ILocalizer
{
    string this[string name] { get; }
}