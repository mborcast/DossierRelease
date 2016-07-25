using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DossierParser
{
    public class BinarySearchTree<T> where T : IComparable<T>
    {
        internal class BinaryTreeNode<T> : IComparable<BinaryTreeNode<T>> where T : IComparable<T>
        {
            internal T value;

            internal BinaryTreeNode<T> parent;
            internal BinaryTreeNode<T> leftChild;
            internal BinaryTreeNode<T> rightChild;

            public BinaryTreeNode(T pValue)
            {
                if (pValue == null)
                {
                    throw new ArgumentNullException("Cannot insert null value");
                }

                this.value = pValue;
                this.parent = null;
                this.leftChild = null;
                this.rightChild = null;
            }

            public override string ToString()
            {
                return this.value.ToString();
            }

            public override int GetHashCode()
            {
                return this.value.GetHashCode();
            }

            public override bool Equals(object pObj)
            {
                BinaryTreeNode<T> lOther = (BinaryTreeNode<T>)pObj;
                return this.CompareTo(lOther) == 0;
            }

            public int CompareTo(BinaryTreeNode<T> pOther)
            {
                return this.value.CompareTo(pOther.value);
            }
        }

        private BinaryTreeNode<T> root;

        public BinarySearchTree()
        {
            this.root = null;
        }

        public void Insert(T pValue)
        {
            this.root = Insert(pValue, null, root);
        }

        private BinaryTreeNode<T> Insert(T pValue, BinaryTreeNode<T> pParent, BinaryTreeNode<T> pNode)
        {
            if (pNode == null)
            {
                pNode = new BinaryTreeNode<T>(pValue);
                pNode.parent = pParent;
            }
            else
            {
                int compareTo = pValue.CompareTo(pNode.value);

                if (compareTo < 0)
                {
                    pNode.leftChild = Insert(pValue, pNode, pNode.leftChild);
                }
                else if (compareTo > 0)
                {
                    pNode.rightChild = Insert(pValue, pNode, pNode.rightChild);
                }
            }

            return pNode;
        }

        private BinaryTreeNode<T> Find(T pValue)
        {
            BinaryTreeNode<T> lNode = this.root;
            int compareTo;
            while (lNode != null)
            {
                compareTo = pValue.CompareTo(lNode.value);

                if (compareTo < 0)
                {
                    lNode = lNode.leftChild;
                }
                else if (compareTo > 0)
                {
                    lNode = lNode.rightChild;
                }
                else
                {
                    break;
                }
            }

            return lNode;
        }

        public bool Contains(T pValue)
        {
            return this.Find(pValue) != null;
        }

        public void Remove(T pValue)
        {
            BinaryTreeNode<T> lNodeToDelete = Find(pValue);

            if (lNodeToDelete != null)
            {
                Remove(lNodeToDelete);
            }
        }

        private void Remove(BinaryTreeNode<T> pNode)
        {
            if (pNode.leftChild != null && pNode.rightChild != null)
            {
                BinaryTreeNode<T> lReplacement = pNode.rightChild;

                while (lReplacement.leftChild != null)
                {
                    lReplacement = lReplacement.leftChild;
                }
                pNode.value = lReplacement.value;
                pNode = lReplacement;
            }

            BinaryTreeNode<T> lChild = pNode.leftChild != null ? pNode.leftChild : pNode.rightChild;

            if (lChild != null)
            {
                lChild.parent = pNode.parent;

                if (pNode.parent == null)
                {
                    root = lChild;
                }
                else
                {
                    if (pNode.parent.leftChild == pNode)
                    {
                        pNode.parent.leftChild = lChild;
                    }
                    else
                    {
                        pNode.parent.rightChild = lChild;
                    }
                }
            }
            else
            {
                if (pNode.parent == null)
                {
                    root = null;
                }
                else
                {
                    if (pNode.parent.leftChild == pNode)
                    {
                        pNode.parent.leftChild = null;
                    }
                    else
                    {
                        pNode.parent.rightChild = null;
                    }
                }
            }
        }

        public void PrintTreeDFS()
        {
            PrintTreeDFS(this.root);
            Console.WriteLine();
        }

        private void PrintTreeDFS(BinaryTreeNode<T> pNode)
        {
            if (pNode != null)
            {
                PrintTreeDFS(pNode.leftChild);
                Console.Write(pNode.value + " ");
                PrintTreeDFS(pNode.rightChild);
            }
        }
    }

}
