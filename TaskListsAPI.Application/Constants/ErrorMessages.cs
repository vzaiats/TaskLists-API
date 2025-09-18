namespace TaskListsAPI.Application.Constants
{
    public static class ErrorMessages
    {
        public const string UserNotFound = "User not found";
        public const string CollectionNotFound = "Collection not found";
        public const string TaskNotFound = "Task not found";
        public const string AccessDenied = "Access denied";
        public const string OnlyOwnerCanDelete = "Only owner can delete collection";
        public const string MaxThreeUsers = "Max 3 users allowed";
        public const string NameRequired = "Name is required.";
        public const string NameLength = "Name must be between 1 and 255 characters.";
        public const string OwnerIdRequired = "OwnerId is required.";
        public const string TaskItemTitleRequired = "Title is required.";
        public const string TaskItemTitleLength = "Title cannot exceed 255 characters.";
        public const string TaskItemCollectionIdRequired = "TaskCollectionId is required.";
        public const string ShareTaskCollectionUserIdRequired = "UserId is required.";
        public const string UpdateTaskCollectionNameRequired = "Name is required.";
        public const string UpdateTaskCollectionNameLength = "Name must be between 1 and 255 characters.";
        public const string UpdateTaskItemTitleRequired = "Title is required.";
        public const string UpdateTaskItemTitleLength = "Title cannot exceed 255 characters.";
        public const string UpdateTaskItemIsCompletedRequired = "IsCompleted is required.";
    }
}