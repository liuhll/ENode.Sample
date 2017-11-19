using ENode.Commanding;
using ENode.Infrastructure;
using ENode.Sample.Commands;
using ENode.Sample.Domain;

namespace ENode.Sample.CommandHandlers
{
    //[Component]
    public class NoteCommandHandler : ICommandHandler<CreateNoteCommand>, ICommandHandler<ChangeNoteTitleCommand>
    {

        private readonly ILockService _lockService;

        public NoteCommandHandler(ILockService lockService)
        {
            _lockService = lockService;
        }

        public void Handle(ICommandContext context, CreateNoteCommand command)
        {
            _lockService.ExecuteInLock(typeof(Note).Name, () =>
            {
                context.Add(new Note(command.AggregateRootId, command.Title));
            });
        }

        public void Handle(ICommandContext context, ChangeNoteTitleCommand command)
        {
            context.Get<Note>(command.AggregateRootId).ChangeTitle(command.Title);

        }
    }
}
