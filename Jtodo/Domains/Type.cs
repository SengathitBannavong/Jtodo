using System.Drawing;

namespace Jtodo.Domains
{
    // Type is feel like category or tag
    class Type
    {
        private readonly UInt64 _id;
        private readonly string _name;
        private readonly Color _color;

        public UInt64 Id => _id;
        public string Name => _name;
        public Color Color => _color;
        public Type(UInt64 Id,string Name,Color Color)
        {
            _name = Name;
            _id = Id;
            _color = Color;
        }
    }
}
