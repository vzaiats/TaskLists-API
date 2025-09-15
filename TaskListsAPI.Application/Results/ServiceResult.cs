namespace TaskListsAPI.Application.Results
{
    // Service result with success result, data and errors
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        #region Methods

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Error(string error)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        #endregion
    }
}