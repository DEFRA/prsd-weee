namespace EA.Weee.Domain.Lookup
{
    using System;

    public class MessageBanner
    {
        public MessageBanner()
        {
        }

        public MessageBanner(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
    }
}
