using System;
using System.IO;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [Serializable]
    public class SceneReference : ISceneReference, IEquatable<ISceneReference>
    {
        [SerializeField] private string _name = "";
        [SerializeField] private string _path = "";
        [SerializeField] private bool _enabled = false;
        [SerializeField] private int _buildIndex = -1;

        public string name => _name;
        public string path => _path;
        public bool enabled => _enabled;
        public int buildIndex => _buildIndex;

        public SceneReference()
        {
        }

        public SceneReference(string path, bool enabled = false, int buildIndex = -1)
            : this(Path.GetFileNameWithoutExtension(path), path, enabled, buildIndex)
        {
        }

        public SceneReference(string name, string path, bool enabled = false, int buildIndex = -1)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (enabled == false && buildIndex != -1) throw new ArgumentOutOfRangeException(nameof(buildIndex));

            _name = name;
            _path = path;
            _enabled = enabled;
            _buildIndex = buildIndex;
        }

        #region IEquatable

        public bool Equals(ISceneReference other)
        {
            return _name == other.name && _path == other.path && _enabled == other.enabled && _buildIndex == other.buildIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SceneReference) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_path != null ? _path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _enabled.GetHashCode();
                hashCode = (hashCode * 397) ^ _buildIndex;
                return hashCode;
            }
        }

        #endregion
    }
}