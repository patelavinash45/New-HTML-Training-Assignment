namespace Services.ViewModels
{
    public class ChatMessage
    {
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public string Message { get; set; }
        
        public string Time { get; set; }
    }
}

