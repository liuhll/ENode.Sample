using ENode.Eventing;

namespace ENode.Sample.Domain
{
    public class NoteTitleChangedEvent : DomainEvent<string>
    {
        public string Title { get; private set; }

        public NoteTitleChangedEvent() { }
        public NoteTitleChangedEvent(string title) : base()
        {
            Title = title;
        }
    }
}