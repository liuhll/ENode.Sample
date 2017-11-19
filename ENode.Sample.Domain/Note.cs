using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENode.Domain;

namespace ENode.Sample.Domain
{
    
    public class Note : AggregateRoot<string>
    {
        private string _title;

        public string Title {
            get { return _title; }
        }

        public Note(string id, string title) : base(id)
        {
            ApplyEvent(new NoteCreatedEvent(title));
        }

        public void ChangeTitle(string title)
        {
            ApplyEvent(new NoteTitleChangedEvent(title));
        }

        private void Handle(NoteCreatedEvent evnt)
        {
            _id = evnt.AggregateRootId;
            _title = evnt.Title;
        }
        private void Handle(NoteTitleChangedEvent evnt)
        {
            _id = evnt.AggregateRootId;
            _title = evnt.Title;
        }

    }
}
