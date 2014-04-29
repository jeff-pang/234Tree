using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _234Tree
{
    public class Node
    {
        public ulong?[] Keys { get; private set; }
        Node[] _edges;
        public int KeyCount { get; private set; }
        public int EdgesCount { get; private set; }        

        public Node(ulong key)
        {
            //using a fixed array is more conveinient but wasteful as we need a little more loops to compare and reshuffle the keys and edges
            Keys = new ulong?[]{key,null,null};
            _edges = new Node[] { null, null, null, null };
            KeyCount = 1;
        }

        /// <summary>
        /// Find if the position of the key
        /// </summary>
        /// <param name="k">the key value to find</param>
        /// <returns>return -1 if not found or 0,1,2 respectively</returns>
        public int FindKeyPosition(ulong k)
        {
            for (int x = 0; x < 3; x++)
            {
                if(Keys[x] == k)
                {
                    return x;
                }
            }

            return -1;
        }

        public int HasKey(ulong k)
        {
            for (int x = 0; x < KeyCount; x++)
            {
                if(Keys[x]==k)
                {
                    return x;
                }
            }

            return -1;
        }
        public void SetEdge(int position,Node edge)
        {
            //shift all the previous positions by 1

            bool hasEmptySlot = false;
            if(_edges[position]!=null)
            {
                for (int x = 3; x > position && x > 0 ; x--)
                {
                    if (_edges[x]==null)
                    {
                        hasEmptySlot = true;
                    }
                    _edges[x] = _edges[x - 1];
                }
            }
            else
            {
                hasEmptySlot = true;
            }

            if (hasEmptySlot)
            {
                EdgesCount++;
            }
            _edges[position] = edge;
        }

        public Node Traverse(ulong k)
        {
            int pos=FindEdgePosition(k);
            return _edges[pos];
        }
        public Node GetEdge(int position)
        {
            if (position < _edges.Length)
            {
                return _edges[position];
            }
            else
            {
                return null;
            }
        }
        public int FindEdgePosition(ulong k)
        {
            //1. left most key will never be null
            //2. if the right key from the current key is null, return the left edge of the current key
            //3. if k is in between left value (which starts at 0) and current key return the left edge of current key

            ulong leftVal = 0;

            for (int x = 0; x < 3; x++)
            {

                if (Keys[x] == null)
                {
                    if(leftVal<k)
                    {
                        return x;
                    }
                    else
                    {
                        return x - 1;
                    }
                }
                else if (leftVal <= k && k <= Keys[x])
                {
                    return x;
                }

                leftVal = Keys[x].Value;
            }

            if (Keys[2].Value < k)
            {
                return 3;
            }

            return -1;
        }
        
        /// <summary>
        /// Restructure the node spliting the left and right keys into 2 single nodes
        /// </summary>
        /// <returns>Restructured subtree by the order: left, right</returns>
        public Node[] Split()
        {
            if(KeyCount!=2)
            {
                throw new InvalidOperationException(string.Format("This node has {0} keys, can only split a 2 keys node", KeyCount));
            }

            Node newRight = new Node(Keys[1].Value);
            newRight._edges[0] = this._edges[2];
            newRight._edges[1] = this._edges[3];

            this._edges[2] = null;
            this._edges[3] = null;
            Keys[1] = null;
            Keys[2] = null;

            this.KeyCount = 1;
            this.EdgesCount = 0;

            //counting edges
            //a better solution is to use a byte and binary operations to represent and count edges
            //e.g 1010 to represent left and middle right edges are occupied
            for (int x = 0; x < 4;x++ )
            {
                if (newRight._edges[x] != null)
                {
                    newRight.EdgesCount++;
                }

                if (this._edges[x] != null)
                {
                    this.EdgesCount++;
                }
            }

            return new Node[] { this, newRight };
        }

        public ulong? Pop(int position)
        {
            if(KeyCount == 1)
            {
                throw new InvalidOperationException("Cannot pop value from a 1 key node");
            }

            if (Keys[position] != null)
            {
                ulong k = Keys[position].Value;
                Keys[position] = null;

                for (int x = position; x < KeyCount - 1; x++)//shift all values to the left
                {
                    Keys[x] = Keys[x + 1];
                    Keys[x + 1] = null;
                }

                KeyCount--;
                return k;
            }
            
            return null;
        }

        public void Push(ulong k)
        {
            if(KeyCount==3)
            {
                throw new InvalidOperationException("Cannot push value into a 3 keys node");
            }

            int pos=FindEdgePosition(k);
            if (pos == 0)//left
            {
                this.Keys[2] = this.Keys[1];
                this.Keys[1] = this.Keys[0];
                this.Keys[0] = k;
            }
            else if(pos == 1)//middle
            {
                this.Keys[2] = this.Keys[1];
                this.Keys[1] = k;
            }
            else //right
            {
                this.Keys[2] = k;
            }

            KeyCount++;
        }
        public override string ToString()
        {
            return string.Format(@"Node({0},{1},{2})", Keys[0],Keys[1],Keys[2]);

        }
    }
}
