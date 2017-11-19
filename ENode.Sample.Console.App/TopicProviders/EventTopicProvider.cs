using ENode.Commanding;
using ENode.EQueue;
using ENode.Eventing;
using ENode.Sample.Common;
using ENode.Sample.Domain;

namespace ENode.Sample.Console.App.TopicProviders
{
    public class EventTopicProvider : AbstractTopicProvider<IDomainEvent>
    {
        public EventTopicProvider()
        {
            RegisterTopic(EQueueTopics.NoteEventTopic, new[]
            {
                typeof(NoteCreatedEvent),
                typeof(NoteTitleChangedEvent)
            });
        }

        //public override string GetTopic(IDomainEvent source)
        //{
        //    return EQueueTopics.NoteEventTopic;
        //}
    }
}