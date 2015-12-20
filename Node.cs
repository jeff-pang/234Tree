using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _234Tree
{
    public class Node
    {
        public List<Node> Edges { get; private set; }
        public List<ulong> Keys { get; private set; }
        public Node Parent { get; set; }
        public Node(ulong key)
        {
            //using a fixed array is more conveinient but wasteful as we need a little more loops to compare and reshuffle the keys and edges
            Keys = new List<ulong>();
            Keys.Add(key);
            Edges = new List<Node>();
        }

        /// <summary>
        /// Finds a key's position if it exists
        /// </summary>
        /// <returns>Key position 0,1 or 2 if found, otherwise -1</returns>
        public int HasKey(ulong k)
        {
            for (int x = 0; x < Keys.Count; x++)
            {
                if(Keys[x]==k)
                {
                    return x;
                }
            }

            return -1;
        }
        
        public void InsertEdge(Node edge)
        {
            for (int x = 0; x < Edges.Count;x++ )
            {
                if(Edges[x].Keys[0]>edge.Keys[0])
                {
                    Edges.Insert(x, edge);
                    return;
                }
            }

            Edges.Add(edge);
            edge.Parent = this;
        }
        
        public bool RemoveEdge(Node n)
        {
            return Edges.Remove(n);
        }

        public Node RemoveEdge(int position)
        {
            Node edge = null;
            if(Edges.Count>position)
            {
                edge = Edges[position];
                edge.Parent = null;
                Edges.RemoveAt(position);
            }

            return edge;
        }

        public Node Traverse(ulong k)
        {
            int pos=FindEdgePosition(k);

            if (pos < Edges.Count && pos>-1)
            {
                return Edges[pos];
            }
            else
            {
                return null;
            }
        }
        public Node GetEdge(int position)
        {
            if (position < Edges.Count)
            {
                return Edges[position];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Find the position (edge) where k's value falls between 2 keys.
        /// </summary>
        /// <param name="k"></param>
        /// <returns>0,1,2 or 3, unless one of the keys equals k, returns -1</returns>
        public int FindEdgePosition(ulong k)
        {
            if (Keys.Count != 0)
            {
                ulong left = 0;
                for (int x = 0; x < Keys.Count; x++)
                {
                    if (left <= k && k < Keys[x])
                    {
                        return x;
                    }
                    else
                    {
                        left = Keys[x];
                    }
                }

                if (k > Keys[Keys.Count - 1])
                {
                    return Keys.Count;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 0;
            }
        }

        public void Fuse(Node n1)
        {
            int totalKeys = n1.Keys.Count;
            int totalEdges=n1.Edges.Count;

            totalKeys += this.Keys.Count;
            totalEdges += this.Edges.Count;

            if(totalKeys>3)
            {
                throw new InvalidOperationException("Total keys of all nodes exceeded 3");
            }

            
            if(totalEdges>4)
            {
                throw new InvalidOperationException("Total edges of all nodes exceeded 4");
            }


            for (int x = 0; x < n1.Keys.Count; x++)
            {
                ulong k = n1.Keys[x];
                this.Push(k);
            }

            for (int x = Edges.Count - 1; x >= 0; x--)
            {
                Node e = n1.RemoveEdge(x);
                this.InsertEdge(e);
            }
        }

        public void Fuse(Node n1,Node n2)
        {
            int totalKeys = n1.Keys.Count;
            int totalEdges = n1.Edges.Count;

            totalKeys += n2.Keys.Count;
            totalEdges += n2.Edges.Count;
            totalKeys += this.Keys.Count;
            totalEdges += this.Edges.Count;
            
            if (totalKeys > 3)
            {
                throw new InvalidOperationException("Total keys of all nodes exceeded 3");
            }

            if (totalEdges > 4)
            {
                throw new InvalidOperationException("Total edges of all nodes exceeded 4");
            }

            this.Fuse(n1);
            this.Fuse(n2);
        }
        
        /// <summary>
        /// Restructure the node spliting the left and right keys into 2 single nodes
        /// </summary>
        /// <returns>Restructured subtree by the order: left, right</returns>
        public Node[] Split()
        {
            if(Keys.Count!=2)
            {
                throw new InvalidOperationException(string.Format("This node has {0} keys, can only split a 2 keys node", Keys.Count));
            }

            Node newRight = new Node(Keys[1]);

            for (int x = 2 ; x<Edges.Count ;x++ )
            {
                newRight.Edges.Add(this.Edges[x]);
            }

            for (int x = Edges.Count-1; x >=2; x--)
            {
                this.Edges.RemoveAt(x);
            }

            for (int x = 1; x < Keys.Count;x++ )
            {
                Keys.RemoveAt(x);
            }

            return new Node[] { this, newRight };
        }

        public ulong? Pop(int position)
        {
            if(Keys.Count == 1)
            {
                throw new InvalidOperationException("Cannot pop value from a 1 key node");
            }

            if (position<Keys.Count)
            {
                ulong k = Keys[position];
                Keys.RemoveAt(position);
                
                return k;
            }
            
            return null;
        }

        public void Push(ulong k)
        {
            if(Keys.Count==3)
            {
                throw new InvalidOperationException("Cannot push value into a 3 keys node");
            }

            if (Keys.Count == 0)
            {
                Keys.Add(k);
            }
            else
            {
                ulong left = 0;
                for (int x = 0; x < Keys.Count; x++)
                {
                    if (left <= k && k < Keys[x] )
                    {
                        Keys.Insert(x, k);
                        return;
                    }
                    else
                    {
                        left = Keys[x];
                    }
                }
                Keys.Add(k);
            }
        }
        public override string ToString()
        {
            string comma = "";
            StringBuilder sb = new StringBuilder();
            for(int x=0;x<Keys.Count;x++)
            {
                sb.Append(comma + Keys[x]);
                comma = ",";
            }

            return sb.ToString();
        }
    }
}
