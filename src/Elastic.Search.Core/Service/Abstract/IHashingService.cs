namespace Elastic.Search.Core.Service.Abstract
{
    public interface IHashingService
    {
        /// <summary>
        /// Hash string
        /// </summary>
        string HashString(string inputkey);
    }
}
