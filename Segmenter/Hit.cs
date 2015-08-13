using System;

namespace JiebaNet.Segmenter
{

    /**
     * 表示一次词典匹配的命中
     */
    public class Hit
    {
        //Hit不匹配
        private static readonly int UNMATCH = 0x00000000;
        //Hit完全匹配
        private static readonly int MATCH = 0x00000001;
        //Hit前缀匹配
        private static readonly int PREFIX = 0x00000010;

        //该HIT当前状态，默认未匹配
        private int hitState = UNMATCH;

        //记录词典匹配过程中，当前匹配到的词典分支节点
        private DictSegment matchedDictSegment;
        /*
	 * 词段开始位置
	 */
        private int begin;
        /*
	 * 词段的结束位置
	 */
        private int end;


        /**
	 * 判断是否完全匹配
	 */

        public Boolean isMatch()
        {
            return (this.hitState & MATCH) > 0;
        }

        /**
	 * 
	 */

        public void setMatch()
        {
            this.hitState = this.hitState | MATCH;
        }

        /**
	 * 判断是否是词的前缀
	 */

        public Boolean isPrefix()
        {
            return (this.hitState & PREFIX) > 0;
        }

        /**
	 * 
	 */

        public void setPrefix()
        {
            this.hitState = this.hitState | PREFIX;
        }

        /**
	 * 判断是否是不匹配
	 */

        public Boolean isUnmatch()
        {
            return this.hitState == UNMATCH;
        }

        /**
	 * 
	 */

        public void setUnmatch()
        {
            this.hitState = UNMATCH;
        }

        public DictSegment getMatchedDictSegment()
        {
            return matchedDictSegment;
        }

        public void setMatchedDictSegment(DictSegment matchedDictSegment)
        {
            this.matchedDictSegment = matchedDictSegment;
        }

        public int getBegin()
        {
            return begin;
        }

        public void setBegin(int begin)
        {
            this.begin = begin;
        }

        public int getEnd()
        {
            return end;
        }

        public void setEnd(int end)
        {
            this.end = end;
        }

    }
}