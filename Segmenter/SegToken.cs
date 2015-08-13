using System;

namespace JiebaNet.Segmenter
{
    public class SegToken
    {
        public String word;
        public int startOffset;
        public int endOffset;

        public SegToken(String word, int startOffset, int endOffset)
        {
            this.word = word;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
        }

        public override String ToString()
        {
            return "[" + word + ", " + startOffset + ", " + endOffset + "]";
        }
    }
}