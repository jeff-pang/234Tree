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
                if (curr.KeyCount == 3)
                {
                    if (parent == null)//the only time when the parent is null is when we are at the root node
                    {
                        ulong k=curr.Pop(1).Value;
                        Node newRoot = new Node(k);
                        Node[] newNodes = curr.Split();
                        newRoot.SetEdge(0, newNodes[0]);
                        newRoot.SetEdge(1, newNodes[1]);
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

                        int pos1 = parent.FindEdgePosition(newNodes[1].Keys[0].Value);
                        parent.SetEdge(pos1, newNodes[1]);

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
                    if (curr.Item2 < currNode.KeyCount)
                    {
                        items.Add(currNode.Keys[curr.Item2].Value);
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

    }
}
