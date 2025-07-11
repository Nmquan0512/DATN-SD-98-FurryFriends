namespace FurryFriends.API.Models
{
    public class ApiResult<T>
    {
        public T Data { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public bool Success => Errors == null || Errors.Count == 0;
    }
}
