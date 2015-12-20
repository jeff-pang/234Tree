using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _234Tree
{
    public class Tree
    {
        public Node Root;
        public Tree()
        {
            Root = null;
        }

        public void Insert(ulong value)
        {
            if (Root == null)
            {
                Root = new Node(value);
                return;
            }

            Node curr = Root;
            Node parent=null;

            while(curr!=null)
            {
                //If we encounter a node with 3 keys, restructure the node, pushing the middle key upwards, 
                if (curr.Keys.Count == 3)
                {
                    if (parent == null)//the only time when the parent is null is when we are at the root node
                    {
                        ulong k=curr.Pop(1).Value;
                        Node newRoot = new Node(k);
                        Node[] newNodes = curr.Split();
                        newRoot.InsertEdge(newNodes[0]);
                        newRoot.InsertEdge(newNodes[1]);
                        Root = newRoot;//make the new subtree's root node the entire tree's root node
                        //curr now points to the left node

                        curr = newRoot;
                    }
                    else
                    {
                        ulong? k=curr.Pop(1);//pop middle key and push it up

                        if(k!=null)
                        {
                            parent.Push(k.Value);//push the value up to the parent
                        }

                        Node[] newNodes = curr.Split();

                        //int pos0=parent.FindEdgePosition(newNodes[0].Keys[0].Value);
                        //parent.SetEdge(pos0, newNodes[0]);

                        int pos1 = parent.FindEdgePosition(newNodes[1].Keys[0]);
                        parent.InsertEdge(newNodes[1]);

                        int posCurr=parent.FindEdgePosition(value);
                        curr=parent.GetEdge(posCurr);
                    }
                }

                parent=curr;
                curr=curr.Traverse(value);
                if(curr==null)//leave node
                {
                    parent.Push(value);
                }
            }
        }

        public Node Find(ulong k)
        {
            Node curr = Root;

            while (curr != null)
            {
                if(curr.HasKey(k)>=0)
                {
                    return curr;
                }
                else
                {
                    int p=curr.FindEdgePosition(k);
                    curr=curr.GetEdge(p);
                }
            }

            return null;
        }

        public void Remove(ulong k)
        {
            //1 if in the leaf node, simply remove it.
            //2 as we encounter 1 key nodes,
            // a) pull the key from the siblings if they have 2 or more keys, via rotation
            // b) if both siblings have only 1 key, the parent (except if it is root) will always have 2 or more keys, 
            //    so pull a key from parent and fuse with it's sibling.
            // c) if siblings have only 1 key and parent is a 1 key root node, fuse all 3 nodes into 1

            Node curr = Root;
            Node parent = null;
            while (curr != null)
            {
                //check for 1 key nodes
                if(curr.Keys.Count==1)
                {
                    if (curr!=Root)//skip root node
                    {
                        ulong cK=curr.Keys[0];
                        int edgePos=parent.FindEdgePosition(cK);
                        
                        bool? takeRight = null;
                        Node sibling = null;

                        if (edgePos > -1)//edge is found
                        {
                            if (edgePos < 3)//use right sibling if it is not the right most node
                            {
                                sibling = parent.GetEdge(edgePos + 1);
                                if (sibling.Keys.Count > 1)
                                {
                                    takeRight = true;
                                }
                            }

                            if (takeRight == null)//if this is the right most node, or there wasn't any left sibling with >1 keys
                            {
                                if (edgePos > 0)//use left sibling if it is not the left most node
                                {
                                    sibling = parent.GetEdge(edgePos - 1);
                                    if (sibling.Keys.Count > 1)
                                    {
                                        takeRight = false;//use left
                                    }
                                }
                            }

                            if (takeRight != null)//case 2a) perform rotation with sibling
                            {
                                ulong pK = 0;
                                ulong sK = 0;

                                if (takeRight.Value)//take from right sibling
                                {
                                    pK = parent.Pop(edgePos).Value;//take parent's key (corresponding to this edge)
                                    sK = sibling.Pop(0).Value;//take sibling's left most key

                                    if (sibling.Edges.Count > 0)
                                    {
                                        Node edge = sibling.RemoveEdge(0);//move left most edge
                                        curr.InsertEdge(edge);
                                    }
                                }
                                else//take from left sibling
                                {
                                    pK = parent.Pop(edgePos).Value;//take parent's key (corresponding to this edge)
                                    sK = sibling.Pop(sibling.Keys.Count-1).Value;//take sibling's right most key

                                    if (sibling.Edges.Count > 0)
                                    {
                                        Node edge = sibling.RemoveEdge(sibling.Edges.Count-1);//move right most edge
                                        curr.InsertEdge(edge);
                                    }
                                }

                                parent.Push(sK);
                                curr.Push(pK);
                            }
                            else//case 2b) or 2c) no siblings with >1 keys available
                            {
                                ulong? pK = null;
                                if(parent.Edges.Count>=2)//case 2b
                                {
                                    if (edgePos == 0)//if n is left most node, take parent's first key
                                    {
                                        pK = parent.Pop(0);
                                    }
                                    else if(edgePos == parent.Edges.Count)//if n is the right most node take parent's right most key
                                    {
                                        pK = parent.Pop(parent.Keys.Count-1);
                                    }
                                    else//take parent's middle key
                                    {
                                        pK = parent.Pop(1);
                                    }

                                    if (pK != null)
                                    {
                                        curr.Push(pK.Value);
                                        Node sib=null;
                                        if (edgePos !=parent.Edges.Count)//use right sibling if it is not the rightmost node
                                        {
                                            sib = parent.RemoveEdge(edgePos + 1);
                                        }
                                        else
                                        {
                                            sib = parent.RemoveEdge(parent.Edges.Count-1);
                                        }

                                        curr.Fuse(sib);
                                    }
                                }
                                else//case 2c
                                {
                                    curr.Fuse(parent,sibling);
                                    Root = curr;
                                    parent = null;
                                }
                            }
                        }
                    }
                }

                int rmPos = -1;
                if ((rmPos = curr.HasKey(k)) >= 0)
                {
                    //if it is a leaf node, remove the key
                    if(curr.Edges.Count==0)
                    {
                        if (curr.Keys.Count == 0)
                        {
                            parent.Edges.Remove(curr);
                        }
                        else
                        {
                            curr.Pop(rmPos);
                        }
                    }
                    else//otherwise, replace it with the next higher key
                    {
                        Node successor = Min(curr.Edges[rmPos]);
                        ulong sK = successor.Keys[0];
                        if(successor.Keys.Count>1)
                        {
                            successor.Pop(0);
                        }
                        else
                        {
                            if (successor.Edges.Count == 0)//just remove it if it is leaf
                            {
                                Node p = successor.Parent;
                                p.RemoveEdge(successor);
                            }
                            else
                            {
                                //not leaf so we have to rotate
                            }
                        }
                    }

                    curr = null;
                }
                else
                {
                    //not found, so we move down the tree
                    int p = curr.FindEdgePosition(k);
                    parent = curr;
                    curr = curr.GetEdge(p);
                }
            }
        }

        public ulong[] Inorder(Node n = null)
        {
            if (n == null)
            {
                n = Root;
            }

            List<ulong> items = new List<ulong>();
            Tuple<Node,int> curr=new Tuple<Node,int>(n,0);
            Stack<Tuple<Node, int>> stack = new Stack<Tuple<Node, int>>();
            while(stack.Count>0 || curr.Item1!=null)
            {
                if (curr.Item1 != null)//Case 1
                {
                    stack.Push(curr);
                    Node leftChild = curr.Item1.GetEdge(curr.Item2);//move to leftmost unvisited child
                    curr = new Tuple<Node, int>(leftChild, 0);
                }
                else//Case 2
                {
                    curr = stack.Pop();
                    Node currNode=curr.Item1;
                    
                    //because for every node, it can possibly have more edges than key
                    //if the current index corresponds to a key, we want to add the key into the list.
                    //else we just want to traverse it's edges.
                    if (curr.Item2 < currNode.Keys.Count)
                    {
                        items.Add(currNode.Keys[curr.Item2]);
                        curr = new Tuple<Node, int>(currNode, curr.Item2 + 1);
                    }
                    else
                    {
                        Node rightChild = currNode.GetEdge(curr.Item2 + 1);//get the rightmost child, may be null

                        //if right most child is null, we will visit 'Case 2' again in the next loop,
                        //and the parent will be popped off the stack
                        curr = new Tuple<Node, int>(rightChild, curr.Item2 + 1);
                    }
                }
            }
            return items.ToArray();
        }

        public Node Min(Node n=null)
        {
            if(n==null)
            {
                n = Root;
            }

            Node curr=n;
            if (curr != null)
            {
                while (curr.Edges.Count > 0)
                {
                    curr = curr.Edges[0];
                }
            }

            return curr;
        }
    }
}
