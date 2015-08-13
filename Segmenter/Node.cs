namespace JiebaNet.Segmenter
{
    public class Node
    {
        public char value;
        public Node parent;

        public Node(char value, Node parent)
        {
            this.value = value;
            this.parent = parent;
        }
    }
}