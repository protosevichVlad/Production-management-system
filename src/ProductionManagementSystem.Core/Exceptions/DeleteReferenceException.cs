namespace ProductionManagementSystem.Core.Exceptions
{
    public class DeleteReferenceException : BaseException
    {
        public string Title { get; set; }

        public DeleteReferenceException(string title, string message) : base(message)
        {
            Title = title;
        }
    }
}