namespace JiebaNet.Segmenter.PosSeg
{
    // TODO: duplicate
    public class Node
    {
        public string Value { get; private set; }
        public Node Parent { get; private set; }

        public Node(string value, Node parent)
        {
            Value = value;
            Parent = parent;
        }
    }
}