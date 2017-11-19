using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENode.Commanding;

namespace ENode.Sample.Commands
{
    public class CreateNoteCommand : Command<string>
    {
        public string Title { get; set; }
    }
}
