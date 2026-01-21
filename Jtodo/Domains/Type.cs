namespace Jtodo.Domains
{
    public class Type
    {
        private UInt64 _id;
        private string _text;
        private int _color;

        public UInt64 Id { get => _id; private set => _id = value; }
        public string Text { get => _text; private set => _text = value; }
        public int Color { get => _color; private set => _color = value; }

        // Constructor for EF Core (private)
        private Type()
        {
            _text = string.Empty;
        }
        
        public Type(UInt64 Id, string Text, int Color)
        {
            _id = Id;
            _text = Text;
            _color = Color;
        }
    }
}
