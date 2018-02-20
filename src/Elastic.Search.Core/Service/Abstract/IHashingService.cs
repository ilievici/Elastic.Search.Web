namespace Elastic.Search.Core.Service.Abstract
{
    public interface IHashingService
    {
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
    }
}
