using System.Threading.Tasks;
using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using ENode.Sample.Common;
using ENode.Sample.Domain;

namespace ENode.Sample.Denormalizers
{
    //[Component]
    public class NoteDenormalizer : AbstractDenormalizer,
        IMessageHandler<NoteCreatedEvent>,
        IMessageHandler<NoteTitleChangedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(NoteCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection => connection.InsertAsync(new
            {
                Id = evnt.AggregateRootId,
                Title = evnt.Title,
                CreatedOn = evnt.Timestamp,
                UpdatedOn = evnt.Timestamp,
                Version = evnt.Version,
                EventSequence = evnt.Sequence
            }, ConfigSettings.NoteTable)); 
        }

        public Task<AsyncTaskResult> HandleAsync(NoteTitleChangedEvent evnt)
        {
            return TryUpdateRecordAsync(connection => connection.UpdateAsync(new
            {
                Title = evnt.Title,
                UpdatedOn = evnt.Timestamp,
                Version = evnt.Version,
                EventSequence = evnt.Sequence
            }, new
            {
                Id = evnt.AggregateRootId,
                Version = evnt.Version - 1
            }, ConfigSettings.NoteTable));
        }
    }
}
