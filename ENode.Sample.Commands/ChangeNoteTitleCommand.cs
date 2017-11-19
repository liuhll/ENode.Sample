using ENode.Commanding;

namespace ENode.Sample.Commands
{
    public class ChangeNoteTitleCommand : Command<string>
    {
        public string Title { get; set; }
    }
}