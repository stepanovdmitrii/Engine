using System;
using Engine.Core.Collections;

namespace Engine.Core.Extensions
{
    public static class BinaryTreeExtension
    {
        public static void DeleteAll<TKey, TValue>(this BinaryTree<TKey, TValue> binaryTree, TKey key) where TKey: IComparable<TKey>
        {
            while(binaryTree.Delete(key)){
                
            }
        }
    }
}
