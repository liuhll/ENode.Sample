using ENode.Eventing;

namespace ENode.Sample.Domain
{
    public class NoteCreatedEvent : DomainEvent<string>
    {
        public string Title { get; private set; }

        public NoteCreatedEvent() { }
        public NoteCreatedEvent(string title) : base()
        {
            Title = title;
        }

    }
}
