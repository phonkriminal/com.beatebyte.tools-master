using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatebyteToolsEditor.Utils
{
    /// <summary>
    /// TagMask allow you to display the Tags popup menu in the inspector.
    /// Will Create a list of tags like a LayerMask inspector.
    /// </summary>
    [System.Serializable]
    public class eTagMask : IList<string>
    {
        [SerializeField]
        private List<string> tags = new List<string>();

        public eTagMask()
        {
            tags = new List<string>();
        }

        public eTagMask(List<string> tags)
        {
            this.tags = tags;
        }

        public eTagMask(params string[] arg)
        {
            this.tags = new List<string>(arg);
        }

        public bool Contains(string tag)
        {
            return tags.Contains(tag);
        }

        public void Add(string tag)
        {
            if (!tags.Contains(tag)) tags.Add(tag);
        }

        public void Remove(string tag)
        {
            if (tags.Contains(tag)) tags.Remove(tag);
        }

        public void Clear()
        {
            tags.Clear();
        }

        public int Count { get { return tags.Count; } }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public string this[int index] { get { return tags[index]; } set { if (!tags.Contains(value)) tags[index] = value; } }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tags.GetEnumerator();
        }

        public int IndexOf(string item)
        {
            return tags.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            if (!tags.Contains(item))
                tags.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < tags.Count)
                tags.RemoveAt(index);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            tags.CopyTo(array, arrayIndex);
        }

        bool ICollection<string>.Remove(string item)
        {
            if (tags.Contains(item)) return tags.Remove(item);
            return false;
        }

        public static implicit operator List<string>(eTagMask t)
        {
            return t.tags;
        }

        public static implicit operator string[](eTagMask t)
        {
            return t.tags.ToArray();
        }

        public static implicit operator eTagMask(List<string> l)
        {
            return new eTagMask(l);
        }

        public static implicit operator eTagMask(string[] l)
        {
            return new eTagMask(l);
        }

        public static eTagMask operator +(eTagMask a, eTagMask b)
        {
            for (int i = 0; i < b.tags.Count; i++)
            {
                if (!a.Contains(b.tags[i]))
                    a.Add(b.tags[i]);
            }
            return a.tags;
        }

        public static eTagMask operator -(eTagMask a, eTagMask b)
        {
            for (int i = 0; i < b.tags.Count; i++)
            {
                if (a.Contains(b.tags[i]))
                    a.Remove(b.tags[i]);
            }
            return a.tags;
        }

        public static eTagMask operator +(eTagMask a, List<string> b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (!a.Contains(b[i]))
                    a.Add(b[i]);
            }
            return a.tags;
        }

        public static eTagMask operator -(eTagMask a, List<string> b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (a.Contains(b[i]))
                    a.Remove(b[i]);
            }
            return a.tags;
        }

        public static eTagMask operator +(eTagMask a, string[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (!a.Contains(b[i]))
                    a.Add(b[i]);
            }
            return a.tags;
        }

        public static eTagMask operator -(eTagMask a, string[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (a.Contains(b[i]))
                    a.Remove(b[i]);
            }
            return a.tags;
        }
    }


}