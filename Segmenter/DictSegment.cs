using System;
using System.Collections.Generic;

namespace JiebaNet.Segmenter
{
    /**
     * 词典树分段，表示词典树的一个分枝
     */
    public class DictSegment : IComparable<DictSegment>
    {

        // 公用字典表，存储汉字
        private static readonly IDictionary<char, char> charMap = new Dictionary<char, char>(16);
        // 数组大小上限
        private static readonly int ARRAY_LENGTH_LIMIT = 3;

        // Map存储结构
        private IDictionary<char, DictSegment> childrenMap;
        // 数组方式存储结构
        private DictSegment[] childrenArray;

        // 当前节点上存储的字符
        private char nodeChar;
        // 当前节点存储的Segment数目
        // storeSize <=ARRAY_LENGTH_LIMIT ，使用数组存储， storeSize >ARRAY_LENGTH_LIMIT
        // ,则使用Map存储
        private int storeSize = 0;
        // 当前DictSegment状态 ,默认 0 , 1表示从根节点到当前节点的路径表示一个词
        private int nodeState = 0;

        DictSegment(char nodeChar)
        {
            //if (nodeChar == null) {
            //    throw new ArgumentNullException("nodeChar", "参数为空异常，字符不能为空");
            //}
            this.nodeChar = nodeChar;
        }

        char getNodeChar()
        {
            return nodeChar;
        }

        /*
         * 判断是否有下一个节点
         */
        bool hasNextNode()
        {
            return this.storeSize > 0;
        }

        /**
         * 匹配词段
         * 
         * @param charArray
         * @return Hit
         */
        Hit match(char[] charArray)
        {
            return this.match(charArray, 0, charArray.Length, null);
        }

        /**
         * 匹配词段
         * 
         * @param charArray
         * @param begin
         * @param length
         * @return Hit
         */
        Hit match(char[] charArray, int begin, int length)
        {
            return this.match(charArray, begin, length, null);
        }

        /**
         * 匹配词段
         * 
         * @param charArray
         * @param begin
         * @param length
         * @param searchHit
         * @return Hit
         */
        Hit match(char[] charArray, int begin, int length, Hit searchHit)
        {

            if (searchHit == null)
            {
                // 如果hit为空，新建
                searchHit = new Hit();
                // 设置hit的其实文本位置
                searchHit.setBegin(begin);
            }
            else
            {
                // 否则要将HIT状态重置
                searchHit.setUnmatch();
            }
            // 设置hit的当前处理位置
            searchHit.setEnd(begin);

            char keyChar = charArray[begin];
            DictSegment ds = null;

            // 引用实例变量为本地变量，避免查询时遇到更新的同步问题
            DictSegment[] segmentArray = this.childrenArray;
            IDictionary<char, DictSegment> segmentMap = this.childrenMap;

            // STEP1 在节点中查找keyChar对应的DictSegment
            if (segmentArray != null)
            {
                // 在数组中查找
                DictSegment keySegment = new DictSegment(keyChar);
                int position = Array.BinarySearch(segmentArray, 0, this.storeSize, keySegment);
                if (position >= 0)
                {
                    ds = segmentArray[position];
                }

            }
            else if (segmentMap != null)
            {
                // 在map中查找
                ds = segmentMap[keyChar];
            }

            // STEP2 找到DictSegment，判断词的匹配状态，是否继续递归，还是返回结果
            if (ds != null)
            {
                if (length > 1)
                {
                    // 词未匹配完，继续往下搜索
                    return ds.match(charArray, begin + 1, length - 1, searchHit);
                }
                else if (length == 1)
                {

                    // 搜索最后一个char
                    if (ds.nodeState == 1)
                    {
                        // 添加HIT状态为完全匹配
                        searchHit.setMatch();
                    }
                    if (ds.hasNextNode())
                    {
                        // 添加HIT状态为前缀匹配
                        searchHit.setPrefix();
                        // 记录当前位置的DictSegment
                        searchHit.setMatchedDictSegment(ds);
                    }
                    return searchHit;
                }

            }
            // STEP3 没有找到DictSegment， 将HIT设置为不匹配
            return searchHit;
        }

        /**
         * 加载填充词典片段
         * 
         * @param charArray
         */
        void fillSegment(char[] charArray)
        {
            this.fillSegment(charArray, 0, charArray.Length, 1);
        }

        /**
         * 屏蔽词典中的一个词
         * 
         * @param charArray
         */
        void disableSegment(char[] charArray)
        {
            this.fillSegment(charArray, 0, charArray.Length, 0);
        }

        // TODO: synchronized

        /**
         * 加载填充词典片段
         * 
         * @param charArray
         * @param begin
         * @param length
         * @param enabled
         */
        private void fillSegment(char[] charArray, int begin, int length, int enabled)
        {
            // 获取字典表中的汉字对象
            char beginChar = charArray[begin];

            // TODO: get/set key/val
            // 字典中没有该字，则将其添加入字典
            if (!charMap.ContainsKey(beginChar))
            {
                charMap[beginChar] = beginChar;
            }
            char keyChar = charMap[beginChar];

            // 搜索当前节点的存储，查询对应keyChar的keyChar，如果没有则创建
            DictSegment ds = lookforSegment(keyChar, enabled);
            if (ds != null)
            {
                // 处理keyChar对应的segment
                if (length > 1)
                {
                    // 词元还没有完全加入词典树
                    ds.fillSegment(charArray, begin + 1, length - 1, enabled);
                }
                else if (length == 1)
                {
                    // 已经是词元的最后一个char,设置当前节点状态为enabled，
                    // enabled=1表明一个完整的词，enabled=0表示从词典中屏蔽当前词
                    ds.nodeState = enabled;
                }
            }
        }


        /**
         * 查找本节点下对应的keyChar的segment *
         * 
         * @param keyChar
         * @param create
         *            =1如果没有找到，则创建新的segment ; =0如果没有找到，不创建，返回null
         * @return
         */
        private DictSegment lookforSegment(char keyChar, int create)
        {

            DictSegment ds = null;

            if (this.storeSize <= ARRAY_LENGTH_LIMIT)
            {
                // 获取数组容器，如果数组未创建则创建数组
                DictSegment[] segmentArray = getChildrenArray();
                // 搜寻数组
                DictSegment keySegment = new DictSegment(keyChar);
                int position = Array.BinarySearch(segmentArray, 0, this.storeSize, keySegment);
                if (position >= 0)
                {
                    ds = segmentArray[position];
                }

                // 遍历数组后没有找到对应的segment
                if (ds == null && create == 1)
                {
                    ds = keySegment;
                    if (this.storeSize < ARRAY_LENGTH_LIMIT)
                    {
                        // 数组容量未满，使用数组存储
                        segmentArray[this.storeSize] = ds;
                        // segment数目+1
                        this.storeSize++;
                        Array.Sort(segmentArray, 0, this.storeSize);
                    }
                    else
                    {
                        // 数组容量已满，切换Map存储
                        // 获取Map容器，如果Map未创建,则创建Map
                        IDictionary<char, DictSegment> segmentMap = getChildrenMap();
                        // 将数组中的segment迁移到Map中
                        migrate(segmentArray, segmentMap);
                        // 存储新的segment
                        segmentMap[keyChar] = ds;
                        // segment数目+1 ， 必须在释放数组前执行storeSize++ ， 确保极端情况下，不会取到空的数组
                        this.storeSize++;
                        // 释放当前的数组引用
                        this.childrenArray = null;
                    }
                }
            }
            else
            {
                // 获取Map容器，如果Map未创建,则创建Map
                IDictionary<char, DictSegment> segmentMap = getChildrenMap();
                if (!segmentMap.ContainsKey(keyChar) && create == 1)
                {
                    // 构造新的segment
                    ds = new DictSegment(keyChar);
                    segmentMap[keyChar] = ds;
                    // 当前节点存储segment数目+1
                    this.storeSize++;
                }
                // 搜索Map
                ds = segmentMap[keyChar];
            }

            return ds;
        }

        // TODO: synchronized
        /**
         * 获取数组容器 线程同步方法
         */
        private DictSegment[] getChildrenArray()
        {
            if (this.childrenArray == null)
            {
                lock (this)
                {
                    if (this.childrenArray == null)
                    {
                        this.childrenArray = new DictSegment[ARRAY_LENGTH_LIMIT];
                    }
                }
            }
            return this.childrenArray;
        }


        // TODO: synchronized
        /**
         * 获取Map容器 线程同步方法
         */
        private IDictionary<char, DictSegment> getChildrenMap()
        {
            if (this.childrenMap == null)
            {
                lock (this)
                {
                    if (this.childrenMap == null)
                    {
                        this.childrenMap = new Dictionary<char, DictSegment>(ARRAY_LENGTH_LIMIT * 2);
                    }
                }
            }
            return this.childrenMap;
        }

        /**
         * 将数组中的segment迁移到Map中
         * 
         * @param segmentArray
         */
        private void migrate(DictSegment[] segmentArray, IDictionary<char, DictSegment> segmentMap)
        {
            foreach (var segment in segmentArray)
            {
                if (segment != null)
                {
                    segmentMap[segment.nodeChar] = segment;
                }
            }
        }

        /**
         * 实现IComparable接口
         * 
         * @param o
         * @return int
         */
        public int CompareTo(DictSegment o)
        {
            // 对当前节点存储的char进行比较
            return this.nodeChar.CompareTo(o.nodeChar);
        }
    }
}