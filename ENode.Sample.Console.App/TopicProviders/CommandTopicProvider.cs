using ENode.Commanding;
using ENode.EQueue;
using ENode.Eventing;
using ENode.Sample.Commands;
using ENode.Sample.Common;

namespace ENode.Sample.Console.App.TopicProviders
{
    public class CommandTopicProvider : AbstractTopicProvider<ICommand>
    {
        public CommandTopicProvider()
        {
            RegisterTopic(EQueueTopics.NoteCommandTopic, new[]
            {
                typeof(ChangeNoteTitleCommand),
                typeof(CreateNoteCommand)
            });
        }

        //public override string GetTopic(ICommand source)
        //{
        //    return EQueueTopics.NoteCommandTopic;
        //}
    }
}